using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Win32
{
	internal class NativeMethods
	{
		[DllImport("wininet.dll", SetLastError = true)]
		public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

		[DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "InternetGetCookieEx", CallingConvention = CallingConvention.StdCall)]
		public static extern bool InternetGetCookieEx(string lpszURL, IntPtr lpszCookieName, StringBuilder lpszCookieData, ref uint lpdwSize, uint dwFlags, IntPtr lpReserved);

		[DllImport(@"wininet", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "InternetSetCookieEx", CallingConvention = CallingConvention.StdCall)]
		public static extern bool InternetSetCookieEx(string lpszURL, IntPtr lpszCookieName, string lpszCookieData, uint dwFlags, IntPtr dwReserved);

		public const uint INTERNET_COOKIE_HTTPONLY = 0x2000U;
	}
}
