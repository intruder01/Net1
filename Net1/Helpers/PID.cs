/**********************************************************************************************
 * ORIGINAL - Arduino PID Library - Version 1.2.1
 * by Brett Beauregard <br3ttb@gmail.com> brettbeauregard.com
 *
 * This Library is licensed under the MIT License
 **********************************************************************************************/

//Converted to C# by Jack Sadowski
//NOTE: For my purposes, I replaced millis() with internal call counter.
//method PID() counts number of times it was called to determine when
//to perform output update.

//Usage:
//pid = new PID(P, I, D, POnE, Direction);
//pid.SetInterval(Interval);
//pid.SetOutputLimits(OutputMin, OutputMax);
//PIDMode = PID_Constants.AUTOMATIC;
//pid.SetMode(PIDMode);

//Constants used in some of the functions below

namespace Net1
{
	public static class PID_Constants
	{
		public static int AUTOMATIC = 1;
		public static int MANUAL = 0;
		public static int DIRECT = 0;
		public static int REVERSE = 1;
		public static int P_ON_M = 0;
		public static int P_ON_E = 1;
	}
	public class PID_Control
	{
		private double dispKp;              // * we'll hold on to the tuning parameters in user-entered 
		private double dispKi;              //   format for display purposes
		private double dispKd;              //

		private double kp;                  // * (P)roportional Tuning Parameter
		private double ki;                  // * (I)ntegral Tuning Parameter
		private double kd;                  // * (D)erivative Tuning Parameter

		private int controllerDirection;
		private int pOn;

		private double Input;              // * Pointers to the Input, Output, and Setpoint variables
		private double Output;             //   This creates a hard link between the variables and the 
		private double Setpoint;           //   PID, freeing the user from having to constantly tell us
										   //   what these values are.  with pointers we'll just know.

		private double outputSum, lastInput;

		private int SampleInterval;           // Original SampleTime
		private uint SampleCount;             // Replaced with SampleCount - JS
		private double outMin, outMax;
		private bool inAuto, pOnE;
		private bool firstPass;


		/*Constructor (...)*********************************************************
	 *    The parameters specified here are those for for which we can't set up
	 *    reliable defaults, so we need to have the user set them.
	 ***************************************************************************/
		public PID_Control(double Kp, double Ki, double Kd, int POn, int ControllerDirection)
		{
			inAuto = false;

			SampleInterval = 100;                          //default cycle is 100 calls to Compute()
			SampleCount = 0;

			SetOutputLimits(0, 1);                      //default output limit 
			SetControllerDirection(ControllerDirection);
			SetTunings(Kp, Ki, Kd, POn);
			firstPass = true;
		}

		/*Constructor (...)*********************************************************
		 *    To allow backwards compatability for v1.1, or for people that just want
		 *    to use Proportional on Error without explicitly saying so
		 ***************************************************************************/

		public PID_Control(double Kp, double Ki, double Kd, int ControllerDirection)
			: this(Kp, Ki, Kd, PID_Constants.P_ON_E, ControllerDirection)
		{

		}


		/* Compute() **********************************************************************
		 *     This, as they say, is where the magic happens.  this function should be called
		 *   every time "void loop()" executes.  the function will decide for itself whether a new
		 *   pid Output needs to be computed.  returns true when the output is computed,
		 *   false when nothing has been done.
		 **********************************************************************************/
		public double Compute(double sp, double ip, double op)
		{
			Input = ip;
			Output = op;
			Setpoint = sp;

			if (!inAuto || firstPass)
			{
				outputSum = Output;
				lastInput = Input;
				firstPass = false;
				return Output;
			}

			if (++SampleCount >= SampleInterval)
			{
				/*Compute all the working error variables*/
				double input = Input;
				double error = Setpoint - input;
				double dInput = (input - lastInput);
				outputSum += (ki * error);

				/*Add Proportional on Measurement, if P_ON_M is specified*/
				if (!pOnE)
					outputSum -= kp * dInput;

				if (outputSum > outMax) outputSum = outMax;
				else if (outputSum < outMin) outputSum = outMin;

				/*Add Proportional on Error, if P_ON_E is specified*/
				double output;
				if (pOnE) output = kp * error;
				else output = 0;

				/*Compute Rest of PID Output*/
				output += outputSum - kd * dInput;

				if (output > outMax) output = outMax;
				else if (output < outMin) output = outMin;
				Output = output;

				/*Remember some variables for next time*/
				lastInput = input;

				//op = Output;

				SampleCount = 0;
			}

			return Output;
		}

		/* SetTunings(...)*************************************************************
		 * This function allows the controller's dynamic performance to be adjusted.
		 * it's called automatically from the constructor, but tunings can also
		 * be adjusted on the fly during normal operation
		 ******************************************************************************/
		public void SetTunings(double Kp, double Ki, double Kd, int POn)
		{
			if (Kp < 0 || Ki < 0 || Kd < 0) return;

			pOn = POn;
			pOnE = POn == PID_Constants.P_ON_E;

			dispKp = Kp; dispKi = Ki; dispKd = Kd;

			double SampleTimeInSec = ((double)SampleInterval);
			kp = Kp;
			ki = Ki * SampleTimeInSec;
			kd = Kd / SampleTimeInSec;

			if (controllerDirection == PID_Constants.REVERSE)
			{
				kp = (0 - kp);
				ki = (0 - ki);
				kd = (0 - kd);
			}
		}

		/* SetTunings(...)*************************************************************
		 * Set Tunings using the last-rembered POn setting
		 ******************************************************************************/
		public void SetTunings(double Kp, double Ki, double Kd)
		{
			SetTunings(Kp, Ki, Kd, pOn);
		}

		/* SetSampleTime(...) *********************************************************
		 * sets the period, in Milliseconds, at which the calculation is performed
		 ******************************************************************************/
		public void SetInterval(int NewInterval)
		{
			if (NewInterval > 0)
			{
				double ratio = (double)NewInterval
								/ (double)SampleInterval;
				ki *= ratio;
				kd /= ratio;
				SampleInterval = NewInterval;
			}
		}

		/* SetOutputLimits(...)****************************************************
		 *     This function will be used far more often than SetInputLimits.  while
		 *  the input to the controller will generally be in the 0-1023 range (which is
		 *  the default already,)  the output will be a little different.  maybe they'll
		 *  be doing a time window and will need 0-8000 or something.  or maybe they'll
		 *  want to clamp it from 0-125.  who knows.  at any rate, that can all be done
		 *  here.
		 **************************************************************************/
		public void SetOutputLimits(double Min, double Max)
		{
			if (Min >= Max) return;
			outMin = Min;
			outMax = Max;

			if (inAuto)
			{
				if (Output > outMax) Output = outMax;
				else if (Output < outMin) Output = outMin;

				if (outputSum > outMax) outputSum = outMax;
				else if (outputSum < outMin) outputSum = outMin;
			}
		}

		/* SetMode(...)****************************************************************
		 * Allows the controller Mode to be set to manual (0) or Automatic (non-zero)
		 * when the transition from manual to auto occurs, the controller is
		 * automatically initialized
		 ******************************************************************************/
		public void SetMode(int Mode)
		{
			bool newAuto = (Mode == PID_Constants.AUTOMATIC);
			if (newAuto && !inAuto)
			{  /*we just went from manual to auto*/
				initialize();
			}
			inAuto = newAuto;
		}

		/* Initialize()****************************************************************
		 *	does all the things that need to happen to ensure a bumpless transfer
		 *  from manual to automatic mode.
		 ******************************************************************************/
		private void initialize()
		{
			outputSum = Output;
			lastInput = Input;
			if (outputSum > outMax) outputSum = outMax;
			else if (outputSum < outMin) outputSum = outMin;
		}

		/* SetControllerDirection(...)*************************************************
		 * The PID will either be connected to a DIRECT acting process (+Output leads
		 * to +Input) or a REVERSE acting process(+Output leads to -Input.)  we need to
		 * know which one, because otherwise we may increase the output when we should
		 * be decreasing.  This is called from the constructor.
		 ******************************************************************************/
		public void SetControllerDirection(int Direction)
		{
			if (Direction == PID_Constants.DIRECT || Direction == PID_Constants.REVERSE)
			{
				if (inAuto && Direction != controllerDirection)
				{
					kp = (0 - kp);
					ki = (0 - ki);
					kd = (0 - kd);
				}
				controllerDirection = Direction;
			}
		}

		/* Status Funcions*************************************************************
		 * Just because you set the Kp=-1 doesn't mean it actually happened.  these
		 * functions query the internal state of the PID.  they're here for display
		 * purposes.  this are the functions the PID Front-end uses for example
		 ******************************************************************************/
		public double GetKp() { return dispKp; }
		public double GetKi() { return dispKi; }
		public double GetKd() { return dispKd; }
		public int GetMode() { return inAuto ? PID_Constants.AUTOMATIC : PID_Constants.MANUAL; }
		public int GetDirection() { return controllerDirection; }
		public uint GetSampleCount() { return SampleCount; }
		public void SetOutput(double op) { Output = op; }
		public double GetInput() { return Input; }
		public double GetOutput() { return Output; }
		public double GetSetpoint() { return Setpoint; }

	}
}

