using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Win32
{
	public static class User32
	{
		[DllImport("User32.dll", SetLastError = true)]
		extern public static UInt16 GetAsyncKeyState([In] int vKey);

		public static readonly int VK_SHIFT = 0x10;
		public static readonly int VK_CONTROL = 0x11;
	}
}
