using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net1
{
	public static class Global
	{
		public static Random rnd = new Random();

		//initial values
		public static double SYNAPSE_INITIAL_PERMANENCE_MIN = 0.45;     //Synapse initialisation value minimum
		public static double SYNAPSE_INITIAL_PERMANENCE_MAX = 0.55;     //Synapse initialisation value minimum
		public static int DENDRITE_INITIAL_ACTIVATION_THRESHOLD = 1;	//Dendrite minimum connected synapses to activate
		public static double COLUMN_INITIAL_BOOST_VALUE = 1.0;			//Initial (default) boost value for Column
		public static double COLUMN_BOOST_ADJ_FACTOR = 0.1;				//Column Boost adaptation speed 

		//constants
		public static int DEFAULT_TIME_AVERAGE_PERIOD = 10;				// default time period for time average function
		public static int OVERRIDE_DEPTH = 2;

		//display tabs parameters
		public static int DISPLAY_X_OFFSET = 5;         //X offset of pattern in display tabs
		public static int DISPLAY_Y_OFFSET = 5;         //Y offset of pattern in display tabs
		public static int DISPLAY_X_PITCH = 10;         //X pitch between columns in display tabs
		public static int DISPLAY_Y_PITCH = 10;         //Y pitch between columns in display tabs

		//chart parameters
		public static int CHART_NUM_POINTS = 100;		//number of points to display on chart

		

#if TESTING

		public static class Tests
		{
			public static int TestNumLoops = 10;		//Number of loops in Tests
		}

#endif

	}
}
