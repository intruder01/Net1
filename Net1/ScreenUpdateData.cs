using System;
using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts.WinForms;

namespace Net1
{
	public delegate void ScreenUpdateChangedEventHandler();
	public static class ScreenUpdateData
	{
		//declare event handler for data changes
		public static ScreenUpdateChangedEventHandler DataChanged;
		public static void OnDataChanged()
		{
			DataChanged?.Invoke();
		}

		//items which trigger automatic update event
		private static string _Filename; public static string Filename { get { return _Filename; } set { _Filename = value; OnDataChanged(); } }
		private static int _FileNumColumnsX; public static int FileNumColumnsX { get { return _FileNumColumnsX; } set { _FileNumColumnsX = value; OnDataChanged(); } }
		private static int _FileNumColumnsY; public static int FileNumColumnsY { get { return _FileNumColumnsY; } set { _FileNumColumnsY = value; OnDataChanged(); } }



		//items which require manual update by client

		public static LineSeries ChartSparsenessLineSeries { get; set; }    //Sparseness chart data series
		public static LineSeries ChartEntropyLineSeries { get; set; }       //Entropy chart data series
		public static double EntropyVal { get; set; }
		public static double SparsenessVal { get; set; }


		//parameter search chackboxes - need manual update
		public static bool SparsenessParamSearchEnable { get; set; }

		static ScreenUpdateData()
		{
			Filename = "";
			ChartSparsenessLineSeries = new LineSeries();    //Sparsness chart data series
			ChartEntropyLineSeries = new LineSeries();       //Entropy chart data series
		}

	}
}
