using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using Grabacr07.KanColleWrapper.Win32;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public partial class KanColleProxy
	{
		private readonly IConnectableObservable<Session> connectableSessionSource;
		private readonly IConnectableObservable<Session> apiSource;
		private readonly LivetCompositeDisposable compositeDisposable;

		public IObservable<Session> SessionSource
		{
			get { return this.connectableSessionSource.AsObservable(); }
		}

		public IObservable<Session> ApiSessionSource
		{
			get { return this.apiSource.AsObservable(); }
		}

		public IProxySettings UpstreamProxySettings { get; set; }


		public KanColleProxy()
		{
			this.compositeDisposable = new LivetCompositeDisposable();

			this.connectableSessionSource = Observable
				.FromEvent<SessionStateHandler, Session>(
					action => new SessionStateHandler(action),
					h => FiddlerApplication.AfterSessionComplete += h,
					h => FiddlerApplication.AfterSessionComplete -= h)
				.Publish();

			this.apiSource = this.connectableSessionSource
				.Where(s => s.PathAndQuery.StartsWith("/kcsapi"))
				.Where(s => s.oResponse.MIMEType.Equals("text/plain"))
				#region .Do(debug)
#if DEBUG
.Do(session =>
				{
					Debug.WriteLine("==================================================");
					Debug.WriteLine("Fiddler session: ");
					Debug.WriteLine(session);
					Debug.WriteLine("");
				})
#endif
			#endregion
				.Publish();
		}


		public void Startup(int proxy = 37564)
		{
			FiddlerApplication.Startup(proxy, false, true);
			FiddlerApplication.BeforeRequest += this.SetUpstreamProxyHandler;
			FiddlerApplication.BeforeRequest += this.SetCookieIfNoCookie;

			SetIESettings("localhost:" + proxy);

			this.compositeDisposable.Add(this.connectableSessionSource.Connect());
			this.compositeDisposable.Add(this.apiSource.Connect());
		}

		public void Shutdown()
		{
			this.compositeDisposable.Dispose();

			FiddlerApplication.BeforeRequest -= this.SetCookieIfNoCookie;
			FiddlerApplication.BeforeRequest -= this.SetUpstreamProxyHandler;
			FiddlerApplication.Shutdown();
		}


		private static void SetIESettings(string proxyUri)
		{
			// ReSharper disable InconsistentNaming
			const int INTERNET_OPTION_PROXY = 38;
			const int INTERNET_OPEN_TYPE_PROXY = 3;
			// ReSharper restore InconsistentNaming

			INTERNET_PROXY_INFO proxyInfo;
			proxyInfo.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
			proxyInfo.proxy = Marshal.StringToHGlobalAnsi(proxyUri);
			proxyInfo.proxyBypass = Marshal.StringToHGlobalAnsi("local");

			var proxyInfoSize = Marshal.SizeOf(proxyInfo);
			var proxyInfoPtr = Marshal.AllocCoTaskMem(proxyInfoSize);
			Marshal.StructureToPtr(proxyInfo, proxyInfoPtr, true);

			NativeMethods.InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, proxyInfoPtr, proxyInfoSize);
		}

		/// <summary>
		/// Fiddler からのリクエスト発行時にプロキシを挟む設定を行います。
		/// </summary>
		/// <param name="requestingSession">通信を行おうとしているセッション。</param>
		private void SetUpstreamProxyHandler(Session requestingSession)
		{
			var settings = this.UpstreamProxySettings;
			if (settings == null) return;

			var useGateway = !string.IsNullOrEmpty(settings.Host) && settings.IsEnabled;
			if (!useGateway || (IsSessionSSL(requestingSession) && !settings.IsEnabledOnSSL)) return;

			var gateway = settings.Host.Contains(":")
				// IPv6 アドレスをプロキシホストにした場合はホストアドレス部分を [] で囲う形式にする。
				? string.Format("[{0}]:{1}", settings.Host, settings.Port)
				: string.Format("{0}:{1}", settings.Host, settings.Port);

			requestingSession["X-OverrideGateway"] = gateway;
		}

		/// <summary>
		/// セッションが SSL 接続を使用しているかどうかを返します。
		/// </summary>
		/// <param name="session">セッション。</param>
		/// <returns>セッションが SSL 接続を使用する場合は true、そうでない場合は false。</returns>
		internal static bool IsSessionSSL(Session session)
		{
			// 「http://www.dmm.com:433/」の場合もあり、これは Session.isHTTPS では判定できない
			return session.isHTTPS || session.fullUrl.StartsWith("https:") || session.fullUrl.Contains(":443");
		}

		/// <summary>
		/// Cookie が設定されていない場合、Internet Explorer の Cookie キャッシュから Cookie を読み込んで設定します。
		/// </summary>
		/// <param name="requestingSession">通信を行おうとしているセッション。</param>
		private void SetCookieIfNoCookie(Session requestingSession)
		{
			if (!requestingSession.url.StartsWith("www.dmm.com/"))
			{
				// DMM.COM 以外の URL だった場合は何もしない
				return;
			}

			var headers = requestingSession.oRequest.headers;
			bool isSessionSet = false;

			// Cookie にセッション ID がある場合は何もしないように、フラグを立てる
			if (headers.Exists("Cookie"))
			{
				var cookies = headers["Cookie"];
				var cookieList = cookies.Split(' ');
				foreach (var cookie in cookieList)
				{
					if (cookie.StartsWith("INT_SESID"))
					{
						isSessionSet = true;
						break;
					}
				}
			}

			// フラグが立たなかった場合、Internet Explorer の Cookie で置き換える
			if(!isSessionSet)
			{
				uint cookieStringSize = 4096;
				var cookieString = new StringBuilder((int)cookieStringSize);
				if(NativeMethods.InternetGetCookieEx(requestingSession.fullUrl, IntPtr.Zero, cookieString, ref cookieStringSize, 0, IntPtr.Zero))
				{
					if(headers.Exists("Cookie"))
					{
						headers["Cookie"] = cookieString.ToString();
					}
					else
					{
						headers.Add("Cookie", cookieString.ToString());
					}
				}
			}
		}
	}
}
