using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net1
{
	//allow notifications of engine started and stopped
	//used in ... TODO
	public delegate void SimEngineStarted_Event(object sender, EventArgs e);
	public delegate void SimEngineShutdown_Event(object sender, EventArgs e);

	/// <summary>
	/// This is the main class to start the 3D visualization.
	/// </summary>
	public class Viewer3D 
	{
		public static event SimEngineStarted_Event EngineStarted = delegate { };
		public static event SimEngineShutdown_Event EngineShutdown = delegate { };

		#region Fields

		private static Thread _thread;

		#endregion


		#region Properties

		public static bool IsActive { get; private set; }

		public static Viewer3DForm Form { get; private set; }

		public static Viewer3DEngine Engine { get; private set; }


		#endregion


		#region Methods

		/// <summary>
		/// Open the 3D viewer in a separate thread
		/// </summary>
		public static void Start()
		{
			//if thread running - kill it
			if(_thread != null && _thread.IsAlive)
			{
				_thread.Abort();
			}

			//start new thread calling StartMethod method
			_thread = new Thread(StartMethod);
			_thread.TrySetApartmentState(ApartmentState.MTA);
			_thread.Start();

			IsActive = true;
		}


		/// <summary>
		/// Close the 3D view thread
		/// </summary>
		public static void End()
		{
			EngineShutdown(Engine, EventArgs.Empty);
			if(_thread != null)
			{
				_thread.Abort();
			}
			IsActive = false;

			Engine = null;
		}

		/// <summary>
		/// Thred start method. Open the 3D viewer and start engine.
		/// </summary>
		private static void StartMethod ()
		{
			//Create the viewer form
			var form = new Viewer3DForm ();
			Form = form;

			//if (!Properties.Settings.Default.StealthMode)
			//	Form.Show();

			//Show viewer form
			Form.Show ();

			//Start the 3D engine
			Engine = new Viewer3DEngine ( Form.GetDrawSurface () );

			EngineStarted ( Engine, new EventArgs () );

			Engine.Run ();
		}

		//////////private static void StartMethod ()
		//////////{
		//////////	//Create the viewer form
		//////////	var form = new Viewer3DForm ();
		//////////	Form = form;

		//////////	//if (!Properties.Settings.Default.StealthMode)
		//////////	//	Form.Show();

		//////////	//Show viewer form
		//////////	Form.Show ();

		//////////	//Start the 3D engine
		//////////	//Engine = new Viewer3DEngine ( Form.GetDrawSurface () );

		//////////	Game = new Game1 ( Form.GetDrawSurface () );
		//////////	//EngineStarted ( Engine, new EventArgs () );

		//////////	Game.Run ();
		//////////}

		#endregion


	}
}
