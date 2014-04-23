using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Win32
{
	public static class Kernel32
	{
		/// <summary>
		/// スレッドが優先的に使用するプロセッサ番号を取得します。
		/// </summary>
		/// <param name="hThread">取得対象のスレッドへのハンドル</param>
		/// <param name="lpIdealProcessor">プロセッサ番号を指す PROCESSOR_NUMBER 構造体</param>
		/// <returns>GetThreadIdealProcessorEx 関数の成否</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		extern public static bool GetThreadIdealProcessorEx(IntPtr hThread, [In, Out] ref PROCESSOR_NUMBER lpIdealProcessor);

		/// <summary>
		/// [内部] スレッドが優先的に使用するプロセッサ番号を変更します。
		/// </summary>
		/// <param name="hThread">変更対象のスレッドへのハンドル</param>
		/// <param name="lpIdealProcessor">プロセッサを指定する PROCESSOR_NUMBER 構造体</param>
		/// <param name="lpPreviousIdealProcessor">[省略可能] 以前に設定したプロセッサの PROCESSOR_NUMBER 構造体</param>
		/// <returns>SetThreadIdealProcessorEx 関数の成否</returns>
		[DllImport("Kernel32.dll", SetLastError = true, EntryPoint = "SetThreadIdealProcessorEx")]
		extern private static bool _SetThreadIdealProcessorEx([In] IntPtr hThread, [In] IntPtr lpIdealProcessor, [In] IntPtr lpPreviousIdealProcessor);

		/// <summary>
		/// スレッドが優先的に使用するプロセッサ番号を変更します。
		/// </summary>
		/// <param name="hThread">変更対象のスレッドへのハンドル</param>
		/// <param name="lpIdealProcessor">プロセッサを指定する PROCESSOR_NUMBER 構造体</param>
		/// <param name="lpPreviousIdealProcessor">以前に設定したプロセッサの PROCESSOR_NUMBER 構造体</param>
		/// <returns>SetThreadIdealProcessorEx 関数の成否</returns>
		public static bool SetThreadIdealProcessorEx([In] IntPtr hThread, [In] ref PROCESSOR_NUMBER lpIdealProcessor, [In, Out] ref PROCESSOR_NUMBER lpPreviousIdealProcessor)
		{
			var _lpIdealProcessor = GCHandle.Alloc(lpIdealProcessor, GCHandleType.Pinned);
			var _lpPreviousIdealProcessor = GCHandle.Alloc(lpPreviousIdealProcessor, GCHandleType.Pinned);

			var _b = _SetThreadIdealProcessorEx(hThread, _lpIdealProcessor.AddrOfPinnedObject(), _lpPreviousIdealProcessor.AddrOfPinnedObject());

			_lpPreviousIdealProcessor.Free();
			_lpIdealProcessor.Free();

			return _b;
		}

		/// <summary>
		/// スレッドが優先的に使用するプロセッサ番号を変更します。
		/// </summary>
		/// <param name="hThread">変更対象のスレッドへのハンドル</param>
		/// <param name="lpIdealProcessor">プロセッサを指定する PROCESSOR_NUMBER 構造体</param>
		/// <returns>SetThreadIdealProcessorEx 関数の成否</returns>
		public static bool SetThreadIdealProcessorEx([In] IntPtr hThread, [In] ref PROCESSOR_NUMBER lpIdealProcessor)
		{
			var _lpIdealProcessor = GCHandle.Alloc(lpIdealProcessor, GCHandleType.Pinned);
			var _b = _SetThreadIdealProcessorEx(hThread, _lpIdealProcessor.AddrOfPinnedObject(), IntPtr.Zero);
			_lpIdealProcessor.Free();

			return _b;
		}

		/// <summary>
		/// [内部] スレッドが使用できるプロセッサを指定します。
		/// </summary>
		/// <param name="hThread">変更対象のスレッドへのハンドル</param>
		/// <param name="GroupAffinity">プロセッサ グループにおけるプロセッサのビットマップ</param>
		/// <param name="PreviousGroupAffinity">以前のプロセッサのビットマップ</param>
		/// <returns>SetThreadGroupAffinity 関数の成否</returns>
		[DllImport("Kernel32.dll", SetLastError = true, EntryPoint = "SetThreadGroupAffinity")]
		extern private static Boolean _SetThreadGroupAffinity([In] IntPtr hThread, [In] ref GROUP_AFFINITY GroupAffinity, [In] IntPtr PreviousGroupAffinity);

		/// <summary>
		/// スレッドが使用できるプロセッサを指定します。
		/// </summary>
		/// <param name="hThread">変更対象のスレッドへのハンドル</param>
		/// <param name="GroupAffinity">プロセッサ グループにおけるプロセッサのビットマップ</param>
		/// <returns>SetThreadGroupAffinity 関数の成否</returns>
		public static Boolean SetThreadGroupAffinity([In] IntPtr hThread, [In] ref GROUP_AFFINITY GroupAffinity)
		{
			return _SetThreadGroupAffinity(hThread, ref GroupAffinity, IntPtr.Zero);
		}

		/// <summary>
		/// スレッドが使用できるプロセッサを指定します。
		/// </summary>
		/// <param name="hThread">変更対象のスレッドへのハンドル</param>
		/// <param name="GroupAffinity">プロセッサ グループにおけるプロセッサのビットマップ</param>
		/// <param name="PreviousGroupAffinity">以前のプロセッサのビットマップ</param>
		/// <returns>SetThreadGroupAffinity 関数の成否</returns>
		public static Boolean SetThreadGroupAffinity([In] IntPtr hThread, [In] ref GROUP_AFFINITY GroupAffinity, [In, Out] ref GROUP_AFFINITY PreviousGroupAffinity)
		{
			var _PreviousGroupAffinity = GCHandle.Alloc(PreviousGroupAffinity, GCHandleType.Pinned);
			var _b = _SetThreadGroupAffinity(hThread, ref GroupAffinity, _PreviousGroupAffinity.AddrOfPinnedObject());
			_PreviousGroupAffinity.Free();

			return _b;
		}

		/// <summary>
		/// 呼び出し元のスレッドを指す擬似ハンドルを返します。
		/// </summary>
		/// <returns>呼び出し元のスレッドを指すスレッド ハンドル</returns>
		[DllImport("Kernel32.dll")]
		extern public static IntPtr GetCurrentThread();
	}

	/// <summary>
	/// 特定のプロセッサを指す構造体。
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct PROCESSOR_NUMBER
	{
		/// <summary>
		/// プロセッサ グループ
		/// </summary>
		public ushort Group;
		/// <summary>
		/// プロセッサ番号
		/// </summary>
		public byte Number;
		/// <summary>
		/// パディング用に予約されています。
		/// </summary>
		private byte Reserved;

		/// <summary>
		/// PROCESSOR_NUMBER 構造体を生成します。
		/// </summary>
		/// <param name="Group">プロセッサ グループ</param>
		/// <param name="Number">プロセッサ番号</param>
		public PROCESSOR_NUMBER(ushort Group, byte Number)
		{
			this.Group = Group;
			this.Number = Number;
			this.Reserved = 0;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct GROUP_AFFINITY
	{
		/// <summary>
		/// 使用するプロセッサのビットマップ
		/// </summary>
		public UIntPtr Mask;
		/// <summary>
		/// プロセッサ グループ
		/// </summary>
		public ushort Group;
		/// <summary>
		/// パディング用に予約されています。
		/// </summary>
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 3)]
		private ushort[] Reserved;

		/// <summary>
		/// GROUP_AFFINITY 構造体を生成します。
		/// </summary>
		/// <param name="Group">プロセッサ グループ</param>
		/// <param name="Mask">使用するプロセッサのビットマップ</param>
		public GROUP_AFFINITY(ushort Group, UIntPtr Mask)
		{
			this.Mask = Mask;
			this.Group = Group;
			this.Reserved = new ushort[3];
		}
	}
}
