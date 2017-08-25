using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;

//Trainer class
//links to InputPlane
//handles file operations and case presentation 
//to the InputPlane


namespace Net1
{
	public class Trainer
	{
		//training set width - X dimension of InputPlane
		public int NumColumnsX { get; private set; }
		//training set height - Y dimansion of InputPlane
		public int NumColumnsY { get; private set; }
		//Trainer data string[case][X][Y]
		public List<List<List<int>>> FileData { get; private set; }	
		public string Filename { get; private set; }
		public int NumCases{ get; private set; }
		public int CurrCaseNum { get; private set; }
		public int LastCaseNum { get; private set; }
		public int CurrEpochNum { get; private set; }
		public bool DataReady { get; private set; } //true after file is opened


		//file properties
		TextReader reader;
		CsvParser parser;


		public Trainer()
		{
			NumColumnsX = 0;
			NumColumnsY = 0;
			Filename = "";
			NumCases = 0;
			CurrCaseNum = 0;
			CurrEpochNum = 1;
			DataReady = false;
			Filename = "";
		}

		//test Trainer with just dimenstions, allowing to create InputPlane
		public Trainer(int numInputsX, int numInputsY) : this()
		{
			NumColumnsX = numInputsX;
			NumColumnsY = numInputsY;
		}

		public Trainer(string filename) : this()
		{
			Filename = filename;
			DataReady = ReadFile(Filename);
		}

		public bool ReadFile(string filename)
		{
			if (Path.GetExtension(filename) == ".csv")
			{
				//determine file structure
				ParseFile(filename);

				//clear data
				FileData = new List<List<List<int>>>();
				List<List<int>> CaseData = new List<List<int>>();
				List<int> LineData = new List<int> ();

				//read data
				using ( reader = File.OpenText ( Filename ) )
				{
					parser = new CsvParser ( reader );
					SkipHeader ( parser );

					while ( true )
					{
						string[] row = parser.Read ();

						//end of file - add current case and break
						if ( row == null )
						{
							//if Casedata populated - add it to FileData
							if ( CaseData.Count > 0 )
								FileData.Add ( CaseData );
							break;
						}

						//"Case" line - add current case and clear it
						if ( row[0].Trim () == "Case" )
						{
							//if Casedata populated - add it to FileData
							if ( CaseData.Count > 0 )
								FileData.Add ( CaseData );
							CaseData = new List<List<int>> ();
						}

						//read case data
						if ( row[0].Trim () != "Case" )
						{
							//LineData = Array.ConvertAll(row, s => int.Parse(s)).ToList();
							//LineData = Array.ConvertAll(row, int.Parse).ToList(); //lambda
							LineData = row.Select ( int.Parse ).ToList (); //linq
							CaseData.Add ( LineData );
						}
					}
				}

				Filename = filename;
				ScreenUpdateData.Filename = filename;
				ScreenUpdateData.FileNumColumnsX = NumColumnsX;
				ScreenUpdateData.FileNumColumnsY = NumColumnsY;
				return true;
			}
			else
			{
				Filename = "";
				ScreenUpdateData.Filename = "";
				ScreenUpdateData.FileNumColumnsX = 0;
				ScreenUpdateData.FileNumColumnsY = 0;
				return false;
			}
		}

		//determine file structure and field/case counts
		public bool ParseFile(string filename)
		{
			NumColumnsX = 0;
			NumColumnsY = 0;
			NumCases = 0;

			bool firstCase = true;

			using ( reader = File.OpenText ( Filename ) )
			{
				parser = new CsvParser ( reader );
				SkipHeader ( parser );

				while ( true )
				{
					string[] row = parser.Read ();
					if ( row == null ) break;

					//"Case" - count cases in file
					if ( row[0].Trim () == "Case" )
					{
						if ( NumColumnsX > 0 )
							firstCase = false;  //first case done
						NumCases++;         //count cases
					}

					//use first case to count fields
					if ( row[0].Trim () != "Case" )
					{
						if ( firstCase )
						{
							//find maximum X in first case
							if ( row.Length > NumColumnsX )
								NumColumnsX = row.Length;
							//count number of lines (Y) in first case
							NumColumnsY++;
						}
					}
				}
			}
			
			return true;
		}


		/// <summary>
		/// Skip initial lines in file that form the header of the file.
		/// </summary>
		/// <param name="parser"></param>
		private void SkipHeader (CsvParser parser)
		{
			parser.Read (); //num Columns X
			parser.Read (); //num Columns Y
			parser.Read (); //num Cells in Column
		}


		//present next training case to InputPlane
		public bool LoadNextCase(InputPlane ip)
		{
			//store previous case number
			LastCaseNum = CurrCaseNum;

			if (NumColumnsX == ip.NumColumnsX && NumColumnsY == ip.NumColumnsY)
			{
				//increment current case number
				if (++CurrCaseNum >= NumCases)
				{
					CurrCaseNum = 0;
					CurrEpochNum++;
				}
				copyCaseToInputPlane(ip);
				//ScreenUpdateData.OnDataChanged();
				NetConfigData.OnDataChanged();
			}
			else
				return false;

			return true; 
		}

		//present requested training case to InputPlane
		public bool LoadCase(InputPlane ip, int caseNum)
		{
			if (NumColumnsX == ip.NumColumnsX && NumColumnsY == ip.NumColumnsY && caseNum >= 0 && caseNum < NumCases)
			{
				//set current case number to requested case
				CurrCaseNum = caseNum;
				copyCaseToInputPlane(ip);
			}
			else
				return false;

			return true;
		}

		private void copyCaseToInputPlane (InputPlane ip)
		{
			//present case to InputPlane
			List<List<int>> CaseData = FileData[CurrCaseNum];

			//Reverse xy here to transfer data row-wise
			for ( int x = 0; x < ip.NumColumnsX; x++ )
			{
				for ( int y = 0; y < ip.NumColumnsY; y++ )
				{
					ip.Columns[y][x].SetActive ( CaseData[y][x] > 0 );
				}
			}
		}
	}
}
