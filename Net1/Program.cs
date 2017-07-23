using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Net1
{
	static class Program
	{
		//program main form instance
		public static netForm1 netForm1 { get; private set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]

		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			netForm1 = new netForm1();
			Application.Run(netForm1);
		}
	}
}
