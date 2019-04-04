using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;

namespace ChickenIngot.Console
{
	/// <summary>
	/// Creates a console window that actually works in Unity
	/// You should add a script that redirects output using Console.Write to write to it.
	/// </summary>
	public class WindowsConsole
	{
		TextWriter oldOutput;

		public void Initialize()
		{
			//
			// Attach to any existing consoles we have
			// failing that, create a new one.
			//
			if (!AttachConsole(0x0ffffffff))
			{
				AllocConsole();
			}

			oldOutput = System.Console.Out;

			try
			{
				IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
				Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
				FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
				System.Text.Encoding encoding = System.Text.Encoding.Default;
				StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
				standardOutput.AutoFlush = true;
				System.Console.SetOut(standardOutput);
			}
			catch (Exception e)
			{
				Debug.Log("Couldn't redirect output: " + e.Message);
			}
		}

		public void Shutdown()
		{
			System.Console.SetOut(oldOutput);
			FreeConsole();
		}

		public void SetTitle(string strName)
		{
			SetConsoleTitle(strName);
		}

		private const int STD_OUTPUT_HANDLE = -11;

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AttachConsole(uint dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool FreeConsole();

		[DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll")]
		static extern bool SetConsoleTitle(string lpConsoleTitle);
	}
}