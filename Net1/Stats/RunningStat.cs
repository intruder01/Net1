using System;


//Code for performing running (online) statistics on data sets
//with variables coming in one at a time and without storing the 
//entire data set in memory.
//
//based on John D. Cook articles
//https://www.johndcook.com/blog/standard_deviation/
//https://www.johndcook.com/blog/skewness_kurtosis/
//https://www.johndcook.com/blog/running_regression/
//


namespace Net1.RunningStats
{
	public class RunningStat
	{
		private ulong n;		//num of data values pushed
		private double M1;		//Mean
		private double M2;
		private double M3;
		private double M4;

		private double MinValue;
		private double MaxValue;

		//added to original code
		//calling code assigns Value instead of calling Push()
		private double _Value;
		public double Value
		{
			get { return _Value; }
			set { _Value = value; push(_Value); }
		}


		public RunningStat()
		{
			Clear();
		}

		public void Clear()
		{
			n = 0;
			M1 = M2 = M3 = M4 = 0.0;
			Value = 0.0;
			MinValue = double.MaxValue;
			MaxValue = double.MinValue;
		}

		private void push(double x)
		{
			double delta, delta_n, delta_n2, term1;

			ulong n1 = n;
			if(++n == 100)		//added this condition to re-start every 100 values
				Clear();
			delta = x - M1;
			delta_n = delta / n;
			delta_n2 = delta_n * delta_n;
			term1 = delta * delta_n * n1;
			M1 += delta_n;
			M4 += term1 * delta_n2 * (n * n - 3 * n + 3) + 6 * delta_n2 * M2 - 4 * delta_n * M3;
			M3 += term1 * delta_n * (n - 2) - 3 * delta_n * M2;
			M2 += term1;

			if (x < MinValue) MinValue = x;
			if (x > MaxValue) MaxValue = x;
		}

		public ulong NumDataValues()
		{
			return n;
		}

		public double Mean()
		{
			return M1;
		}

		public double Variance()
		{
			return M2 / (n - 1.0);
		}

		public double StdDev()
		{
			return Math.Sqrt(Variance());
		}

		public double Skewness()
		{
			return Math.Sqrt(n * M3 / Math.Pow(M2, 1.5));
		}

		public double Kurtosis()
		{
			return n * M4 / (M2 * M2) - 3.0;
		}

		public double MinVal()
		{
			return MinValue;
		}

		public double MaxnVal()
		{
			return MaxValue;
		}
	}
}
