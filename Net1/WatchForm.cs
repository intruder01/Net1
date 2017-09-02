using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Threading;
using System.Windows.Forms;
//using System.Data;
//using OpenHTM.CLA;
//using OpenHTM.IDE.UIControls;
using WeifenLuo.WinFormsUI.Docking;
//using Region = OpenHTM.CLA.Region;

namespace Net1
{
	public partial class WatchForm : DockContent
	{

		#region Fields

		// Private singleton instance
		private static WatchForm _instance;

		private List<WatchWindow> watchWindowList;


		#endregion

		#region Properties

		/// <summary>
		/// Singleton
		/// </summary>
		public static WatchForm Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new WatchForm ();
				}
				return _instance;
			}
		}


		#endregion

		#region Nested classes



		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="WatchForm"/> class.
		/// </summary>
		public WatchForm ()
		{
			this.InitializeComponent ();

			// Set UI properties
			this.MdiParent = Program.netFormMain;

			watchWindowList = new List<WatchWindow>();

			WatchWindow.WatchWindowClosed += this.Handler_WatchWindowClosed;
		}

		#endregion

		#region Methods

		public void InitializeParams ()
		{
			
		}
		#endregion

		#region Events



		#endregion

		#region Custom Events


		// event hendler - wait for Engine created
		public void Handler_SimEngineStarted ( object sender, EventArgs e )
		{
			Viewer3DEngine engine = (Viewer3DEngine)sender;
			//subscribe to Engine SelectionChanged event 
			engine.SelectionChangedEvent += Handler_SimSelectionChanged;
		}
		// event handler - wait for Engine shutdown
		public void Handler_SimEngineShutdown ( object sender, EventArgs e )
		{
			Viewer3DEngine engine = (Viewer3DEngine)sender;
			//un-subscribe from Engine SelectionChanged event 
			engine.SelectionChangedEvent -= Handler_SimSelectionChanged;
		}

		/// <summary>
		/// Event handler. Handles Watch selection change notifications.
		/// </summary>
		/// <param name="sender">List of Selectable3DObject - WatchList from Viewer3DEngine
		/// TEST: entire network passed by NetControllerForm.Instance.TopNode.Region</param>
		/// <param name="e"></param>
		public void Handler_SimSelectionChanged ( object sender, EventArgs e, object region )
		{
			SetPropertyGridDataSource ( region );
		}


		/// <summary>
		/// Event Handler. Handles object clicked on StateInformationPanel.
		/// Displays object in WatchForm (Tab).
		/// </summary>
		/// <param name="sender">StateInformationPanel</param>
		/// <param name="e"></param>
		/// <param name="obj">Clicked object. Region, Column, Cell, Segment, Synapse.</param>
		public void Handler_StateInfoPanelObjectClicked ( object sender, EventArgs e, object obj )
		{
			SetPropertyGridDataSource ( obj );
		}

		/// <summary>
		/// Event Handler. Handles object selected from StateInformationPanel.
		/// Creates new WatchWindow to display object and adds the window to watchWindowList.
		/// </summary>
		/// <param name="sender">StateInformationPanel</param>
		/// <param name="e"></param>
		/// <param name="obj">Selected object. Region, Column, Cell, Segment, Synapse.</param>
		public void Handler_StateInfoPanelObjectSelected ( object sender, EventArgs e, object obj )
		{
			// only open new watch window if not already open
			foreach (WatchWindow w in this.watchWindowList)
			{
				// If object already on list - bring object's WatchWondow to front
				if (w.objectDisplayed == obj)
				{
					w.TopLevel = true;
					return;
				}
					
			}

			WatchWindow ww = new WatchWindow ( obj, "" );
			if (ww != null)
			{
				ww.Show ();
				watchWindowList.Add ( ww );
			}
		}

		/// <summary>
		/// Event Handler. Handles object De-selected from StateInformationPanel.
		/// Closes watchWindow for object and removes window from watchWindowList.
		/// </summary>
		/// <param name="sender">StateInformationPanel</param>
		/// <param name="e"></param>
		/// <param name="obj">De-Selected object. Region, Column, Cell, Segment, Synapse.</param>
		public void Handler_StateInfoPanelObjectDeSelected ( object sender, EventArgs e, object obj )
		{
			WatchWindow winToClose = null;
			foreach (WatchWindow w in this.watchWindowList)
			{
				if (w.objectDisplayed == obj)
				{
					winToClose = w;
					break;
				}
			}

			if (winToClose != null)
			{
				winToClose.Close ();
				this.watchWindowList.Remove ( winToClose );
			}
		}
		#endregion



		//Thread safe way of setting the datasource
		//in WatchGrid
		delegate void SetPropertyGridDatasource ( object region ); 

		private void SetPropertyGridDataSource ( object region )
		{
			// InvokeRequired required compares the thread ID of the 
			// calling thread to the thread ID of the creating thread. 
			// If these threads are different, it returns true. 
			if (this.watchPropertyGrid.InvokeRequired)
			{
				SetPropertyGridDatasource d = new SetPropertyGridDatasource ( SetPropertyGridDataSource );
				this.Invoke ( d, new object[] { region } );
			}
			else
			{
				this.watchPropertyGrid.SelectObject ( region, false, 500 );
				this.watchPropertyGrid.Refresh ();
				Application.DoEvents ();
			}
		}

		//when watch window closed - remove that object from list
		private void Handler_WatchWindowClosed ( object sender, EventArgs e, object obj )
		{
			this.watchWindowList.Remove ( (WatchWindow)sender );
		}

	}
}
