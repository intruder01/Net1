using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net1
{
	public enum SelectableObjectType
	{
		None			=0,
		Cell			=1,
		BasalSynapse	=2,
		ProximalSynapse	=3,
		ApicalSynapse	=4
	}

	/// <summary>
	/// Class used in managing objects which can be selected with the mouse on 3DViewer screen
	/// </summary>
	public class Selectable3DObject //TODO: add IWatchItem interface
	{
		#region Properties
		/// <summary>
		/// True if object has been selected with mouse
		/// </summary>
		public bool mouseSelected { get; set; }
		/// <summary>
		/// True if mouse cursor is over object
		/// </summary>
		public bool mouseOver { get; set; }
		/// <summary>
		/// True if object is visible (not used)
		/// </summary>
		public bool isVisible { get; set; }
		/// <summary>
		/// Specifies object type. Used in casting Selectable3DObject back to real entity
		/// </summary>
		public SelectableObjectType SelectableType { get; set; }

		/// <summary>
		/// Selected from 2D grid screen. Placed here for compatibility with OpenHTM
		/// </summary>
		public bool IsDataGridSelected;


		#endregion Properties


		#region Constructor

		public Selectable3DObject ()
		{
			mouseSelected = false;
			mouseOver = false;
			isVisible = false;
			SelectableType = SelectableObjectType.None;
			IsDataGridSelected = false;
		}

		#endregion Constructor


	}
}
