
namespace Net1.RunningStats
{
	public class RunningRegression
	{
		private RunningStat x_stats;
		private RunningStat y_stats;
		private double S_xy;
		private ulong n;

		public RunningRegression()
		{
			x_stats = new RunningStat();
			y_stats = new RunningStat();
			Clear();
		}

		public void Clear()
		{
			x_stats.Clear();
			y_stats.Clear();
			S_xy = 0.0;
			n = 0;
		}

		public void Push(double x, double y)
		{
			S_xy += S_xy += (x_stats.Mean() - x) * (y_stats.Mean() - y) * (double)(n) / (double)(n + 1);

			x_stats.Value = x;
			y_stats.Value = y;
			if (++n == 0)			//added this re-start just in case
				Clear();
		}

		public ulong NumDataValues()
		{
			return n;
		}

		public double Slope()
		{
			double S_xx = x_stats.Variance() * (n - 1.0);

			return S_xy / S_xx;
		}

		public double Intercept()
		{
			return y_stats.Mean() - Slope() * x_stats.Mean();
		}

		public double Correlation()
		{
			double t = x_stats.StdDev() * y_stats.StdDev();
			return S_xy / ((n - 1) * t);
		}
	}
}
