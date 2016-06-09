using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyboardApi;

namespace KeyboardDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			var p = new Process {StartInfo = {FileName = "notepad.exe"}};

			p.Start();

			try
			{
				List<VKey> keys = "hello world".Select(c => new VKey(c)).ToList();
				var procId = p.Id;
				Console.WriteLine("ID: " + procId);
				Console.WriteLine("Sending background keypresses to write \"hello world\"");
				p.WaitForInputIdle();
				foreach (var key in keys)
				{
					key.PressForeground(p.MainWindowHandle);
				}

			}
			catch (InvalidOperationException)
			{
			}
		}
	}
}
