using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



//search control value CV domain and record levels of present value PV
//to determine optimal CV setting for maximum process stability
//after response profiile has been built, select CV
//in the middle of range where PV is displaying most stability
//


namespace Net1
{
	//Search parameter mode
	//Specifies whether exact or closest lower of higher PV are sought
	public enum ParamSearchMode
	{
		None,
		Exact,
		Higher,
		Lower
	}

	public class ParamSearch
	{
		public double PV_Target { get; private set; }		//PV desired value
		public double CVMinLimit { get; private set; }		//control value limits define search space
		public double CVMaxLimit { get; private set; }		//control value limits define search space
		public double CVStepSize { get; private set; }		//search step size (change in CV value)
		public int CVNumSamplesPerStep { get; private set; }//number of samples to average with each Step
		private int CVNumSamplesCtr;						//number of samples counter for this Step
		public ParamSearchMode Mode { get; private set; }	//if exact range not found - pick closest value

		public double CV { get; private set; }				//control value CV, output to the system
		public double PV { get; private set; }				//present value PV, feedback from system


		//storage for curve data
		private List<double> listSamplesCV;			//list of CV sample values
		private List<double> listSamplesPV;			//list of PV sample values
		private int numSamples;						//number of samples to store = range / step size

		//search status information
		public bool SearchCompleted { get; private set; }   //indicated search CV scan is completed
		public double CV_Found { get; private set; }        //store CV value found during serach
		public double PV_Found { get; private set; }        //store CV value found during serach

		//enable/disable control
		public bool Enabled { get; set; }


		public ParamSearch(double PV_Target, double CVMinLimit, double CVMaxLimit, double CVStepSize, int CVNumSamplesPerStep, ParamSearchMode Mode, bool Enabled)
		{
			this.PV_Target = PV_Target;
			this.CVMinLimit = CVMinLimit;
			this.CVMaxLimit = CVMaxLimit;
			this.CVStepSize = CVStepSize;
			this.CVNumSamplesPerStep = CVNumSamplesPerStep;
			this.Mode = Mode;
			this.Enabled = Enabled;

			listSamplesCV = new List<double>();
			listSamplesPV = new List<double>();
			numSamples = (int)((CVMaxLimit - this.CVMinLimit) / this.CVStepSize);
			
			//initialize search status information
			SearchCompleted = false;
			CV_Found = Double.MinValue;
			PV_Found = Double.MinValue;
			
			//initialize internal variables
			CV = CVMinLimit;
			CVNumSamplesCtr = 0;
		}

		public double Update(double pv)
		{
			//if disabled just return 
			//CV_Found will be Double.MinValue until search is completed successfuly
			if (!Enabled) return CV_Found;

			//nothing to do if search already completed - just return found CV 
			if (SearchCompleted) return CV_Found;

			//while counting number of samples per step - just return current CV
			if (++CVNumSamplesCtr < CVNumSamplesPerStep) return CV;
			CVNumSamplesCtr = 0;

			PV = pv;

			//until number of samples reached - keep building response curve
			//and return slowly incrementing CV back to the system
			if (listSamplesCV.Count < numSamples)
			{
				SearchCompleted = false;
				//build response curve
				listSamplesCV.Add(CV);
				listSamplesPV.Add(PV);
				//increment CV
				CV += CVStepSize;
				return CV;
			}

			//once response curve complete - find stable region
			(CV_Found, PV_Found) = FindOptimalCV(listSamplesCV, listSamplesPV, PV_Target, Mode);

			//update status information
			SearchCompleted = true;

			//return optimal CV
			return CV_Found;
		}

		/// <summary>
		/// Once curve data contains all points required
		/// scan the curve for optimal CV range that gives stable desired PV. 
		/// If such ideal range can't be located, find range that gives closest PV
		/// to the desired one. (may need params for HIGHER/LOWER)
		/// bool closestHigher = if PV range not found - will return closest value. THis parameter allows to specify 
		/// whether closest higher or lower is preferred.
		/// </summary>
		private (double cvFound, double pvFound) FindOptimalCV(List<double> lstCV, List<double> lstPV, double pvTarget, ParamSearchMode mode)
		{


			//keep track of ranges found
			List<int> listExactRangeStart = new List<int>();//Exact is preferred. Keep track of exact ranges regardless of mode
			List<int> listExactRangeLength = new List<int>();
			List<int> listClosestRangeStart = new List<int>();//If exact not found. Keep track of closest ranges according to mode.
			List<int> listClosestRangeLength = new List<int>();

			int sampleIdx = 0;                  //index to sample being processed
			bool rngProcessing = false;         //true if currently processing a range
			int rngIdxExact = -1;               //index of found exact range being processed
			int rngIdxClosest = -1;             //index of found closest range being processed (acc to mode)

			int longestRngStartExact = 0;           //index of longest found Exact range
			int longestRngLengthExact = 0;      //length of longest found Ecxact range
			int longestRngStartClosest = 0;     //index of longest found Closest range
			int longestRngLengthClosest = 0;    //length of longest found Closest range

			//FIND RANGES EQUAL TO PV TARGET
			foreach (double pv in lstPV)
			{
				bool sampleInRange = false;

				//determine if sample is within search parameters and 
				//a candidate to store for analysis
				if (mode == ParamSearchMode.Exact && pv == pvTarget)
					sampleInRange = true;
				if (mode == ParamSearchMode.Higher && pv >= pvTarget)
					sampleInRange = true;
				if (mode == ParamSearchMode.Lower && pv <= pvTarget)
					sampleInRange = true;

				//start of range found
				if (sampleInRange && rngProcessing == false)
				{
					rngProcessing = true;
					if (mode == ParamSearchMode.Exact)
					{
						rngIdxExact++;
						listExactRangeStart.Add(sampleIdx);
						listExactRangeLength.Add(0);
					}
					if (mode == ParamSearchMode.Higher || mode == ParamSearchMode.Lower)
					{
						rngIdxClosest++;
						listClosestRangeStart.Add(sampleIdx);
						listClosestRangeLength.Add(0);
					}
				}

				//track end of range index
				if (sampleInRange && rngProcessing == true)
				{
					if (mode == ParamSearchMode.Exact)
						listExactRangeLength[rngIdxExact] = sampleIdx - listExactRangeStart[rngIdxExact] + 1;
					if (mode == ParamSearchMode.Higher || mode == ParamSearchMode.Lower)
						listClosestRangeLength[rngIdxClosest] = sampleIdx - listClosestRangeStart[rngIdxClosest] + 1;
				}

				//mark of range
				if (!sampleInRange)
				{
					rngProcessing = false;
				}

				sampleIdx++;
			}

			//FIND LONGEST EXACT RANGE
			for (int idx = 0; idx < listExactRangeStart.Count; idx++)
			{
				if (listExactRangeLength[idx] > longestRngLengthExact)
				{
					longestRngStartExact = listExactRangeStart[idx];
					longestRngLengthExact = listExactRangeLength[idx];
				}
			}

			//FIND LONGEST CLOSEST RANGE
			for (int idx = 0; idx < listClosestRangeStart.Count; idx++)
			{
				if (listClosestRangeLength[idx] > longestRngLengthClosest)
				{
					longestRngStartClosest = listClosestRangeStart[idx];
					longestRngLengthClosest = listClosestRangeLength[idx];
				}
			}

			//RETURN LONGEST EXACT RANGE IF FOUND (REGARDLESS OF MODE)
			if (longestRngLengthExact > 0)
			{
				int rangeMiddle = longestRngStartExact + (longestRngLengthExact / 2);
				double optimalCV = lstCV[rangeMiddle];
				return (optimalCV, lstPV[rangeMiddle]);
			}

			//RETURN LONGEST CLOSEST RANGE IF FOUND (ACC TO MODE)
			if (mode == ParamSearchMode.Higher || mode == ParamSearchMode.Lower)
			{
				if (longestRngLengthClosest > 0)
				{
					int rangeMiddle = longestRngStartClosest + (longestRngLengthClosest / 2);
					double optimalCV = lstCV[rangeMiddle];
					return (optimalCV, lstPV[rangeMiddle]);
				}
			}

			return (Double.MinValue, Double.MinValue);
		}

		/// <summary>
		/// Reset search process
		/// Clear curve data and restart search alogrithm.
		/// </summary>
		public void Reset()
		{
			listSamplesCV.Clear();
			listSamplesPV.Clear();
			CV = CVMinLimit;
		}
	}
}
