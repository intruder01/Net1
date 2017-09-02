using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using OpenHTM.CLA;

namespace Net1
{
	public delegate void WatchWindowClosed_Event ( object sender, EventArgs e, object obj );
	public partial class WatchWindow : Form
	{
		public static event WatchWindowClosed_Event WatchWindowClosed = delegate { };

		public WatchWindow ()
		{
			InitializeComponent ();
		}

		private object _objectDisplayed;
		public object objectDisplayed
		{
			get { return _objectDisplayed; }
			protected set { 
							_objectDisplayed = value; 
							propertyGrid1.SelectObject ( _objectDisplayed, false, 1000 ); 
						}
		}

		public WatchWindow ( object obj, string titlePrefix = "")
		{
			InitializeComponent ();
			objectDisplayed = obj;

			//set window title 
			//TODO: add these functions
			string text = "";
			//if (objectDisplayed is Cell)
			//	text = ((Cell)objectDisplayed).ToString ();
			//if (objectDisplayed is Column)
			//	text = ((Column)objectDisplayed).ToString ();
			//if (objectDisplayed is Dendrite)
			//	text = ((Dendrite)objectDisplayed).ToString ();
			//if (objectDisplayed is Synapse)
			//	text = ((Synapse)objectDisplayed).ToString ();

			if (titlePrefix.Length > 0 )
				this.Text = titlePrefix + " ";
			this.Text = text;

		}

		//notify listeners when user closes WatchWindow
		private void WatchWindow_FormClosing ( object sender, FormClosingEventArgs e )
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				WatchWindowClosed ( this, e, objectDisplayed );
				Application.DoEvents ();
			}
		}
	}
}
