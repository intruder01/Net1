using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//InputPlane
//Layer with Columns with 1 Cell each
//
//Input file:
//
//

namespace Net1
{
	public class InputPlane : Layer
	{

		#region Constructors

		public InputPlane(int numColumnsX, int numColumnsY) 
			: base(numColumnsX, numColumnsY, 1)
		{

		}


		#endregion





		#region Public Methods

		public new void Update()
		{

		}




		#endregion


	}
}
