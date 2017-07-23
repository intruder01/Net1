using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Math;

namespace Net1
{
	public class GradientDescent
	{
		private double x, prevX;        //x values 
		private double newX;
		private double y, prevY;        //y values 
		private double err, prevErr;	//error to minimize
		//private double slope, prevSlope;//slope (used to control momentum)
		private bool firstPass;			//return 0.0 on first pass, adjust afterwards
		private double adjustValue;		//current adjustment value (for momentum calc)
		private double prevAdjustValue; //previous adjustment value (for momentum calc)
		private double direction;       //search direction +1.0 or -1.0

		private double momAmt;			//amount added due to momentum while slope continuing
		public GradientDescent()
		{
			firstPass = true;
			//slope = 0.1;                //starts adjustment process in random direction 
			//prevSlope = 0.1;            //starts adjustment process in random direction 
			prevAdjustValue = 0.0;     //starts adjustment process in random direction  
			direction = 1.0;
		}

		/// <summary>
		/// Adjust control value by calculating gradient descent rate from previus call.
		/// Use previous X, Y values to calculate the slope of the function.
		/// </summary>
		/// <param name="currX">X value to be adjusted</param>
		/// <param name="currErr">Y value - current</param>
		/// <param name="desiredY">Y value to achieve</param>
		/// <param name="errorTolerance">stop updating when error below this level</param>
		/// <param name="learnRate">Adjustment rate eg. 0.001</param>
		/// <param name="momentum">Fraction of previous adjustment. eg. 0.5</param>
		/// <returns></returns>
		public double AdjustX(double currX, double currY, double desiredY, double errTolernace, double learnRate, double momentum)
		{
			//save previous state
			prevX = x;
			prevY = y;
			prevErr = err;
			//prevSlope = slope;
			prevAdjustValue = adjustValue;

			//save current values
			x = currX;
			y = currY;

			//calculate error
			//err = currY - desiredY;
			err = desiredY - currY;

			//exit if error within tolerance
			if (Math.Abs(err) <= errTolernace)
				return currX;

			//calculate momentum term
			momAmt += prevAdjustValue * momentum;

			//slope changed direction - reset momentum term
			if ((prevErr > 0 && err < 0) || (prevErr < 0 && err > 0))
			{
				momAmt = 0.0;
			}

			//X range min limit reached - reverse direction
			if (currX < 0.0 && prevX >= 0.0)
			{
				direction = -direction;
				momAmt = 0.0;
				currX = 0.0;
			}

			//X range max limit reached - reverse direction
			if (currX > 1.0 && prevX <= 1.0)
			{
				direction = -direction;
				momAmt = 0.0;
				currX = 1.0;
			}

			//calc adjustment amount
			adjustValue = err * learnRate * direction;// + momAmt;

			//adjust X value
			newX = currX + adjustValue;


			double result = currX;

			if (!firstPass && Math.Abs(err) > errTolernace)
				result = newX;

			firstPass = false;

			Debug.WriteLine($"{currX:N4} {desiredY:N4} {currY:N4}   {err:N4} {momAmt:N12} {adjustValue:N12}   {newX:N6}");

			return result;
		}

	}
}
