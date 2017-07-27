using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Net1
{

	public delegate void SimEngineSelectionChanged_Event (object sender, EventArgs e, object obj);
	
	
	/// <summary>
	/// Main class for 3D viewer implementation
	/// </summary>
	public class Viewer3DEngine : Game
	{
		public event SimEngineSelectionChanged_Event SelectionChangedEvent = delegate { };
		#region Fields

		#region HTM

		/// <summary>
		/// 2 dimensional Array to grab prediction information
		/// </summary>
		private float[,] predictions;

		/// <summary>
		/// 2 dimensional Array to grab active columns information
		/// </summary>
		private float[,] activeCOlumns;

		#endregion


		#region Graphics

		private GraphicsDeviceManager graphics;
		private IntPtr drawSurface;

		// Global Matrices
		private Matrix viewMatrix;
		private Matrix projectionMatrix;
		
		// Coordinates constants
		private const float zHtmRegion = -0.0f;    //was-5.0f
		private const float zHtmPlane = -0f; // Shift to the side //was -5.0f 
		private const float yHtmPlane = -5f; // Shift down 

		private bool contentLoaded; //used in LoadContent()

		private SpriteBatch spriteBatch;
		private SpriteFont spriteFont;
		private Color clearColor = Color.CornflowerBlue;

		// Colors
		private Dictionary<HtmCellColors, HtmColorInformation> dictionaryCellColors;
		private Dictionary<HtmProximalSynapseColors, HtmColorInformation> dictionaryProximalSynapseColors;
		private Dictionary<HtmDistalSynapseColors, HtmColorInformation> dictionaryDistalSynapseColors;

		// Right Legend-Elements
		private HtmOverViewInformation rightLegend;

		// Primitives
		private CubePrimitive cube;
		private CoordinateSysPrimitive coordinateSystem;
		private SquarePrimitve bit;
		private LinePrimitive connectionLine;

		//input state varialbles
		KeyboardState keyState, prevKeyState;
		MouseState mouseState, prevMouseState;

		public Microsoft.Xna.Framework.Point mouseLocation;
		public Microsoft.Xna.Framework.Point mouseLocationClick;
		bool mouseLClick, mouseRClick;

		Color mouseOverColor = new Color ( 0, 0, 0 );
		Color selectedColor = Color.Red;
		Color color1 = new Color ( 0, 0, 0 );
		Color color2 = new Color ( 255, 255, 255 );
		int colorChangeMilliseconds = 0;


		#endregion

		#region Grid

		private Texture2D gridTexture;
		private Texture2D whiteTexture;
		private Vector2 gridSize = new Vector2(14f);

		#endregion


		#endregion

		#region Camera

		private Vector3 posCamera = new Vector3(0, 0, 0);
		private Vector3 lookAt = new Vector3(0, 0, 0);

		// A yaw and pitch applied to the second viewport based on input
		private float yawHtm;
		private float pitchHtm;

		private float yawCamera;
		private float pitchCamera;

		private float zoomCamera;

		private float aspectRatio;
		private float sensitivity = 0.2f;
		private float moveSpeedCamera = 1f;
		private float rotateSpeedCamera = .002f;

		FPSCamera camera;

		#endregion


		#region Properties

		/// <summary>
		/// Reference to list of columns from Layer for traversing
		/// </summary>
		public List<List<Column>> HtmRegionColumns { get; private set; }

		public Layer Region { get; set; } //TODO rename this later


		#endregion


		#region FPS Camera

		public class FPSCamera
		{
			#region Fields

			// We don't need an update method because the camera only needs updating
			// when we change one of it's parameters.
			// We keep track if one of our matrices is dirty
			// and reacalculate that matrix when it is accesed.
			private bool viewMatrixDirty = true;
			private bool projectionMatrixDirty = true;

			public float MinZoom = 1;
			public float MaxZoom = float.MaxValue;

			public float MinPitch = -MathHelper.PiOver2 + 0.3f;
			public float MaxPitch = MathHelper.PiOver2 - 0.3f;

			public float MinPosition = -1000f;
			public float MaxPosition = 1000f;

			#endregion

			#region Properties

			public float Pitch
			{
				get
				{
					return this.pitch;
				}
				set
				{
					this.viewMatrixDirty = true;
					this.pitch = MathHelper.Clamp(value, this.MinPitch, this.MaxPitch);
				}
			}
			private float pitch;

			public float Yaw
			{
				get
				{
					return this.yaw;
				}
				set
				{
					this.viewMatrixDirty = true;
					this.yaw = value;
				}
			}
			private float yaw;

			public float FieldOfView
			{
				get
				{
					return this.fieldOfView;
				}
				set
				{
					this.projectionMatrixDirty = true;
					this.fieldOfView = value;
				}
			}
			private float fieldOfView;

			public float AspectRatio
			{
				get
				{
					return this.aspectRatio;
				}
				set
				{
					this.projectionMatrixDirty = true;
					this.aspectRatio = value;
				}
			}
			private float aspectRatio;

			public float NearPlane
			{
				get
				{
					return this.nearPlane;
				}
				set
				{
					this.projectionMatrixDirty = true;
					this.nearPlane = value;
				}
			}
			private float nearPlane;

			public float FarPlane
			{
				get
				{
					return this.farPlane;
				}
				set
				{
					this.projectionMatrixDirty = true;
					this.farPlane = value;
				}
			}
			private float farPlane;

			public float Zoom
			{
				get
				{
					return this.zoom;
				}
				set
				{
					this.viewMatrixDirty = true;
					this.zoom = MathHelper.Clamp(value, this.MinZoom, this.MaxZoom);
				}
			}
			private float zoom = 1;

			public Vector3 Position
			{
				get
				{
					if (this.viewMatrixDirty)
					{
						this.ReCreateViewMatrix();
					}
					return this.position;
				}
				set
				{
					this.viewMatrixDirty = true;
					float valueX = MathHelper.Clamp(value.X, this.MinPosition, this.MaxPosition);
					float valueY = MathHelper.Clamp(value.Y, this.MinPosition, this.MaxPosition);
					float valueZ = MathHelper.Clamp(value.Z, this.MinPosition, this.MaxPosition);
					this.position = new Vector3(valueX, valueY, valueZ);
				}
			}
			private Vector3 position;

			public float MoveSpeed
			{
				get
				{
					return this.moveSpeed;
				}
				set
				{
					this.moveSpeed = value;
				}
			}
			private float moveSpeed = 1f;


			public Vector3 LookAt
			{
				get
				{
					return this.lookAt;
				}
				set
				{
					this.viewMatrixDirty = true;
					this.lookAt = value;
				}
			}
			private Vector3 lookAt;

			public Matrix ViewProjectionMatrix
			{
				get
				{
					return this.getViewMatrix * this.getProjectionMatrix;
				}
			}

			public Matrix getViewMatrix
			{
				get
				{
					if (this.viewMatrixDirty)
					{
						this.ReCreateViewMatrix();
					}
					return this.viewMatrix;
				}
			}
			private Matrix viewMatrix;

			public Matrix getProjectionMatrix
			{
				get
				{
					if (this.projectionMatrixDirty)
					{
						this.ReCreateProjectionMatrix();
					}
					return this.projectionMatrix;
				}
			}
			private Matrix projectionMatrix;

			#endregion

			#region Constructor

			public FPSCamera(float aspectRatio, Vector3 lookAt)
				: this(aspectRatio, MathHelper.PiOver4, lookAt, Vector3.Up, 0.1f, float.MaxValue)
			{
			}

			public FPSCamera(float aspectRatio, float fieldOfView, Vector3 lookAt, Vector3 up, float nearPlane, float farPlane)
			{
				this.aspectRatio = aspectRatio;
				this.fieldOfView = fieldOfView;
				this.lookAt = lookAt;
				this.nearPlane = nearPlane;
				this.farPlane = farPlane;
			}

			#endregion

			#region Methods

			//http://ploobs.com.br/?p=1507

			private void ReCreateViewMatrix()
			{
				Matrix cameraRotation = Matrix.CreateRotationX(this.pitch) * Matrix.CreateRotationY(this.yaw);
				Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
				Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
				Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
				Vector3 target = this.position + cameraRotatedTarget;
				Vector3 up = Vector3.Transform(cameraOriginalUpVector, cameraRotation);
				this.viewMatrix = Matrix.CreateLookAt(this.position, target, up);
			}

			/// <summary>
			/// Recreates our view matrix, then signals that the view matrix
			/// is clean.
			/// </summary>
			private void ReCreateViewMatrix_old()
			{
				// Calculate the relative position of the camera                       
				this.position = Vector3.Transform(Vector3.Backward, Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, 0));

				// Convert the relative position to the absolute position
				this.position *= this.zoom;
				this.position += this.lookAt;

				// Calculate a new viewmatrix
				this.viewMatrix = Matrix.CreateLookAt(this.position, this.lookAt, Vector3.Up);
				this.viewMatrixDirty = false;
			}

			/// <summary>
			/// Recreates our projection matrix, then signals that the projection
			/// matrix is clean.
			/// </summary>
			private void ReCreateProjectionMatrix()
			{
				this.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.AspectRatio, this.nearPlane, this.farPlane);
				this.projectionMatrixDirty = false;
			}

			/// <summary>
			/// Moves the camera and lookAt at to the right,
			/// as seen from the camera, while keeping the same height
			/// </summary>       
			public void MoveCameraRight(float amount)
			{
				Vector3 right = Vector3.Normalize(this.LookAt - this.Position); //calculate forward
				right = Vector3.Cross(right, Vector3.Up); //calculate the real right
				right.Y = 0;
				right.Normalize();
				this.LookAt += right * amount;
			}

			/// <summary>
			/// Moves the camera and lookAt forward,
			/// as seen from the camera, while keeping the same height
			/// </summary>       
			public void MoveCameraForward(float amount)
			{
				Vector3 forward = Vector3.Normalize(this.LookAt - this.Position);
				forward.Y = 0;
				forward.Normalize();
				this.LookAt += forward * amount;
			}

			#endregion
		}

		//end of FPS Cam


		#endregion


		#region Constructors

		public Viewer3DEngine(IntPtr drawSurface)
		{
			this.graphics = new GraphicsDeviceManager(this);
			this.drawSurface = drawSurface;

			this.graphics.PreparingDeviceSettings += this.graphics_PreparingDeviceSettings;
			Control.FromHandle(this.Window.Handle).VisibleChanged += this.Game1_VisibleChanged;
		}
		#endregion

		#region Methods

		#region Form

		/// <summary>
		/// Occurs when original windows' visibility changs and makes sure it stays invisible 
		/// </summary>
		private void Game1_VisibleChanged(object sender, EventArgs e)
		{
			if (Control.FromHandle(this.Window.Handle).Visible)
			{
				Control.FromHandle(this.Window.Handle).Visible = false;
			}
		}
		#endregion

		#region Graphics Device

		public void ResetGraphicsDevice ()
		{
			if(this.GraphicsDevice != null && Viewer3D.Form.pictureBoxSurface.Width != 0 && Viewer3D.Form.pictureBoxSurface.Height != 0 )
			{
				//Change back buffer size
				this.graphics.PreferredBackBufferHeight = Viewer3D.Form.pictureBoxSurface.Height;
				this.graphics.PreferredBackBufferWidth = Viewer3D.Form.pictureBoxSurface.Width;
				this.graphics.ApplyChanges ();
			}
		}

		#endregion


		#endregion


		#region Content

		protected override void LoadContent()
		{
			if(!this.contentLoaded)
			{
				//Create new SpriteBatch which can be used to draw textures
				this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

				//load the sprite font
				this.Content.RootDirectory = @"Content";
				this.spriteFont = this.Content.Load<SpriteFont>("Simulation3DFont");

				//load the grid texture
				this.gridTexture = LoadTexture2D(this.GraphicsDevice, "./Content/Simulation3DTileGrid.png");
				this.whiteTexture = LoadTexture2D(this.GraphicsDevice, "./Content/Simulation3DWhiteTexture.png");

				//Load cell colors for net
				this.dictionaryCellColors = new Dictionary<HtmCellColors, HtmColorInformation>();
				this.dictionaryCellColors.Add(HtmCellColors.Active, new HtmColorInformation(Color.Black, "Cell is activated"));
				this.dictionaryCellColors.Add(HtmCellColors.Inactive, new HtmColorInformation(Color.White, "Cell is inactive"));
				this.dictionaryCellColors.Add(HtmCellColors.Learning, new HtmColorInformation(Color.DarkGray, "Cell is learning"));
				this.dictionaryCellColors.Add(HtmCellColors.Predicting, new HtmColorInformation(Color.Orange, "Cell is predicting (t+2...)"));
				this.dictionaryCellColors.Add(HtmCellColors.SequencePredicting, new HtmColorInformation(Color.Aqua, "Cell is sequence predicting"));
				this.dictionaryCellColors.Add(HtmCellColors.RightPrediction, new HtmColorInformation(Color.LimeGreen, "Cell correctly predicted"));
				this.dictionaryCellColors.Add(HtmCellColors.FalsePrediction, new HtmColorInformation(Color.Red, "Cell is falsely predicted"));
				this.dictionaryCellColors.Add(HtmCellColors.Selected, new HtmColorInformation(Color.Brown, "Cell prediction is lost"));
				this.dictionaryCellColors.Add(HtmCellColors.Inhibited, new HtmColorInformation(Color.Black, "Cell is inhibited"));

				this.dictionaryProximalSynapseColors = new Dictionary<HtmProximalSynapseColors, HtmColorInformation>();
				this.dictionaryProximalSynapseColors.Add(HtmProximalSynapseColors.Default, new HtmColorInformation(Color.White, "Proximal synapse not active, not connected"));
				this.dictionaryProximalSynapseColors.Add(HtmProximalSynapseColors.Active, new HtmColorInformation(Color.White, "Proximal synapse active"));
				this.dictionaryProximalSynapseColors.Add(HtmProximalSynapseColors.ActiveConnected, new HtmColorInformation(Color.White, "Proximal synapse is active and connected"));

				this.dictionaryDistalSynapseColors = new Dictionary<HtmDistalSynapseColors, HtmColorInformation>();
				this.dictionaryDistalSynapseColors.Add(HtmDistalSynapseColors.Default, new HtmColorInformation(Color.White, "Basal synapse not active"));
				this.dictionaryDistalSynapseColors.Add(HtmDistalSynapseColors.Active, new HtmColorInformation(Color.Black, "Basal synapse active"));

				//Create OverviewElement TODO
				this.rightLegend = new HtmOverViewInformation();

				//Get references for traversing regions
				this.Region = Program.netForm1.Net.Lr;
				this.HtmRegionColumns = Program.netForm1.Net.Lr.Columns;

				//Prepare Arrays for 2-dim-content
				this.predictions = new float[this.Region.NumColumnsX, this.Region.NumColumnsY];
				this.activeCOlumns = new float[this.Region.NumColumnsX, this.Region.NumColumnsY];

				//prepare Cube
				this.cube = new CubePrimitive(this.GraphicsDevice);
				this.coordinateSystem = new CoordinateSysPrimitive(this.GraphicsDevice);
				this.bit = new SquarePrimitve(this.GraphicsDevice);
				this.connectionLine = new LinePrimitive(this.GraphicsDevice);

				this.ResetCamera();

				this.contentLoaded = true;

			}
		}

		public static Texture2D LoadTexture2D(GraphicsDevice graphicsDevice, string path)
		{
			return Texture2D.FromStream(graphicsDevice, new StreamReader(path).BaseStream);
		}


			#endregion


		#region Nested Classes

		private struct HtmColorInformation
		{
			#region Fields

			public Color HtmColor;
			public string HtmInformation;

			#endregion

			#region Constructor

			public HtmColorInformation(Color color, string info)
			{
				this.HtmColor = color;
				this.HtmInformation = info;
			}

			#endregion
		}

		private enum HtmCellColors
		{
			Learning,
			FalsePrediction,
			RightPrediction,
			Predicting,
			SequencePredicting,
			Active,
			Inactive,
			Selected,
			Inhibited
		}

		private enum HtmProximalSynapseColors
		{
			Default,
			Active,
			ActiveConnected
		}

		private enum HtmDistalSynapseColors
		{
			Default,
			Active
		}

		private class HtmOverViewInformation
		{
			#region Fields

			public string StepCount;
			public string ChosenHtmElement;
			public string PositionElement;
			public string ActivityRate;
			public string PrecisionRate;

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of the <see cref="HtmOverViewInformation"/> class.
			/// </summary>
			public HtmOverViewInformation ()
			{
				this.StepCount = "";
				this.ChosenHtmElement = "Region";
				this.PositionElement = "";
				this.ActivityRate = "";
				this.PrecisionRate = "";
			}

			#endregion
		}


		#endregion


		#region Draw

		protected override void Draw(GameTime gameTime)
		{
			this.GraphicsDevice.Clear(this.clearColor);

			//Draw Legend
			//this.DrawLegend(); 

			//Draw HTM
			this.DrawHtmInputPlane();
			this.DrawTest ();

			//this.DrawHtmRegion(false);	//TOCONTINUE
			//this.DrawHtmRegion(true);

			//Draw Prediction Plane
			//this.DrawHtmRegionPredictionPlane();
			//this.DrawHtmRegionPredictionReconstructionPlane();  //20160109 - 1

			//Draw Active Columns Plane
			//this.DrawHtmActiveColsPlane();

			//Draw CoordinateSystem
			if ( Viewer3D.Form.ShowCoordinateSystem)
			{
				this.coordinateSystem.Draw(this.viewMatrix, this.projectionMatrix);
			}


			base.Draw(gameTime);		
		}


		private void DrawTest ()
		{
			try
			{
				//Get input data from fileSensor. Attention: Draw rythm happens very often!
				//TODO they use the Global.T concept to access data according to T steps back
				//I did not implement it here as I see no need for it 
				InputPlane ip = Program.netForm1.Net.Ip;
				List<List<Column>> inputData = Program.netForm1.Net.Ip.Columns;
				//int[,] inputData = Program.netForm1.Net.Ip.Columns;

				if ( inputData != null )
				{
					const float alphaValue = 0.9f;

					int regionWidth = ip.NumColumnsX;
					int regionHeight = ip.NumColumnsY;

					Matrix worldTranslationInitial = Matrix.CreateTranslation ( new Vector3 ( 5, 0, 0 ) );
					float bitSquareScale = 0.3f;
					Matrix worldScale = Matrix.CreateScale ( new Vector3 ( bitSquareScale, bitSquareScale, bitSquareScale ) );
					Matrix worldRotate = Matrix.CreateRotationX ( this.pitchHtm ) * Matrix.CreateRotationY ( this.yawHtm );

					for ( int x = 0; x < 5; x++ )
					{
						//All variables are on the method level
						float cf = 1f;
						Matrix worldTranslation = worldTranslationInitial * Matrix.CreateTranslation ( new Vector3 ( x * cf, 0, 0 * cf ) );
						Matrix world = worldScale * worldTranslation * worldRotate;
						
						
						//Draw input bit square
						this.cube.Draw ( world, this.viewMatrix, this.projectionMatrix, Color.White, alphaValue );
					}

				}

			}
			catch ( Exception )
			{
				//occasionally data is in transition and causes exception, abort draw when this is the case
				return;
			}


		}

		/// <summary>
		/// Draws lenend for HTM-Algorithm on the left and right sied of the animation.
		/// </summary>
		private void DrawLegend()
		{
			var gridWidth = (int)this.gridSize.X;
			var gridHeight = (int)this.gridSize.Y;

			const int gridHeightBuffer = 5;
			const int gridWidthBuffer = 30;

			var startVectorLeft = new Vector2 ( 20, 25 );
			var startVectorRight = new Vector2 ( this.GraphicsDevice.PresentationParameters.BackBufferWidth - 250, 25 );
			var startVectorRightTab = new Vector2 ( startVectorRight.X + 120.0f, startVectorRight.Y );

			//Draw left legend
			this.spriteBatch.Begin ( SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, null, null );

			this.spriteBatch.DrawString ( this.spriteFont, "Legend", startVectorLeft, Color.Black );
			foreach ( var item in this.dictionaryCellColors )
			{
				startVectorLeft.Y += gridHeight + gridHeightBuffer;

				this.spriteBatch.Draw ( this.gridTexture, new Rectangle ( (int)startVectorLeft.X, (int)startVectorLeft.Y, gridWidth, gridHeight ), item.Value.HtmColor );

				this.spriteBatch.DrawString ( this.spriteFont, item.Value.HtmInformation, new Vector2 ( startVectorLeft.X + gridWidthBuffer, startVectorLeft.Y ), Color.White );
			}

			//Draw right legend
			string str;
			this.spriteBatch.DrawString ( this.spriteFont, "HTM Information", startVectorRight, Color.Black );
			startVectorRight.Y += gridHeight + gridHeightBuffer + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, "Steps", startVectorRight, Color.White );
			this.spriteBatch.DrawString ( this.spriteFont, this.rightLegend.StepCount, startVectorRightTab, Color.White );
			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, "Chosen", startVectorRight, Color.White );
			this.spriteBatch.DrawString ( this.spriteFont, this.rightLegend.ChosenHtmElement, startVectorRightTab, Color.White );
			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, "Position", startVectorRight, Color.White );
			this.spriteBatch.DrawString ( this.spriteFont, this.rightLegend.PositionElement, startVectorRightTab, Color.White );
			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, "Activity Rate", startVectorRight, Color.White );
			this.spriteBatch.DrawString ( this.spriteFont, this.rightLegend.ActivityRate, startVectorRightTab, Color.White );
			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, "Precision", startVectorRight, Color.White );
			this.spriteBatch.DrawString ( this.spriteFont, this.rightLegend.PrecisionRate, startVectorRightTab, Color.White );

			//debug js
			//startVectorRight.Y += gridHeight + gridHeightBuffer;
			//startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			//this.spriteBatch.DrawString ( this.spriteFont, "MouseGetStateLoc: ", startVectorRight, Color.White );
			//string sMouseLoc = string.Format ( "X {0}  Y {1}", mouseGetStateLocation.X, mouseGetStateLocation.Y );
			//this.spriteBatch.DrawString ( this.spriteFont, sMouseLoc, startVectorRightTab, Color.White );

			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, "MouseLoc: ", startVectorRight, Color.White );
			str = string.Format ( "X {0}  Y {1}", mouseLocation.X, mouseLocation.Y );
			this.spriteBatch.DrawString ( this.spriteFont, str, startVectorRightTab, Color.White );

			//startVectorRight.Y += gridHeight + gridHeightBuffer;
			//startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			//this.spriteBatch.DrawString ( this.spriteFont, "MouseLocClick: ", startVectorRight, Color.White );
			//str = string.Format ( "X {0}  Y {1}", mouseLocationClick.X, mouseLocationClick.Y );
			//this.spriteBatch.DrawString ( this.spriteFont, str, startVectorRightTab, Color.White );

			//startVectorRight.Y += gridHeight + gridHeightBuffer;
			//startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			//this.spriteBatch.DrawString ( this.spriteFont, "minDistProximal: ", startVectorRight, Color.White );
			//str = string.Format ( "{0:0.000}", minDistanceProximal );
			//this.spriteBatch.DrawString ( this.spriteFont, str, startVectorRightTab, Color.White );

			//startVectorRight.Y += gridHeight + gridHeightBuffer;
			//startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			//this.spriteBatch.DrawString ( this.spriteFont, "pickDistProximal: ", startVectorRight, Color.White );
			//str = string.Format ( "{0:0.000}", pickDistanceProximal );
			//this.spriteBatch.DrawString ( this.spriteFont, str, startVectorRightTab, Color.White );

			//startVectorRight.Y += gridHeight + gridHeightBuffer;
			//startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			//this.spriteBatch.DrawString ( this.spriteFont, "minDistDistal: ", startVectorRight, Color.White );
			//str = string.Format ( "{0:0.000}", minDistanceDistal );
			//this.spriteBatch.DrawString ( this.spriteFont, str, startVectorRightTab, Color.White );

			//startVectorRight.Y += gridHeight + gridHeightBuffer;
			//startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			//this.spriteBatch.DrawString ( this.spriteFont, "pickDistDistal: ", startVectorRight, Color.White );
			//str = string.Format ( "{0:0.000}", pickDistanceDistal );
			//this.spriteBatch.DrawString ( this.spriteFont, str, startVectorRightTab, Color.White );

			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, "aspectRatio: ", startVectorRight, Color.White );
			str = string.Format ( "{0:0.00}", aspectRatio );
			this.spriteBatch.DrawString ( this.spriteFont, str, startVectorRightTab, Color.White );

			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, "pitch yaw: ", startVectorRight, Color.White );
			str = string.Format ( "{0:0.00} {1:0.00}", pitchCamera, yawCamera );
			this.spriteBatch.DrawString ( this.spriteFont, str, startVectorRightTab, Color.White );

			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, "posCam: ", startVectorRight, Color.White );
			str = string.Format ( "X={0:0.0} Y={1:0.0} Z={2:0.0} ", posCamera.X, posCamera.Y, posCamera.Z );
			//str = string.Format ( "{0:0.00}", posCamera );
			this.spriteBatch.DrawString ( this.spriteFont, str, startVectorRightTab, Color.White );

			startVectorRight.Y += gridHeight + gridHeightBuffer;
			startVectorRightTab.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, "lookAt: ", startVectorRight, Color.White );
			//str = string.Format ( "{0:0.00}", lookAt );
			str = string.Format ( "X={0:0.0} Y={1:0.0} Z={2:0.0} ", lookAt.X, lookAt.Y, lookAt.Z );
			this.spriteBatch.DrawString ( this.spriteFont, str, startVectorRightTab, Color.White );


			this.spriteBatch.End ();

		}


		private void DrawHtmInputPlane()
		{
			try
			{
				//Get input data from fileSensor. Attention: Draw rythm happens very often!
				//TODO they use the Global.T concept to access data according to T steps back
				//I did not implement it here as I see no need for it 
				InputPlane ip = Program.netForm1.Net.Ip;
				List<List<Column>> inputData = Program.netForm1.Net.Ip.Columns;
				//int[,] inputData = Program.netForm1.Net.Ip.Columns;

				if(inputData != null)
				{
					const float alphaValue = 0.9f;

					int regionWidth = ip.NumColumnsX;
					int regionHeight = ip.NumColumnsY;

					//int regionWidth = inputData.Count;			//X TOCHECK - check if dimensions can be obtained from lists directly without referencing Ip
					//int regionHeight = inputData[0].Count;		//Y TOCHECK

					Matrix worldTranslationBehindDown = Matrix.CreateTranslation ( new Vector3 ( 0, yHtmPlane, 0 ) ) * Matrix.CreateTranslation ( new Vector3 ( 0, 0, zHtmPlane ) );
					float bitSquareScale = 0.3f;
					Matrix worldScale = Matrix.CreateScale (new Vector3 (bitSquareScale, bitSquareScale, bitSquareScale));
					Matrix worldRotate = Matrix.CreateRotationX ( this.pitchHtm ) * Matrix.CreateRotationY ( this.yawHtm );

					for ( int x = 0; x < regionWidth; x++ )
					{
						for ( int z = 0; z < regionHeight; z++ )
						{
							//All variables are on the method level
							float cf = 1f;
							Matrix worldTranslation = Matrix.CreateTranslation ( new Vector3 ( x * cf, 0, z * cf ) ) * worldTranslationBehindDown;
							Matrix world = worldScale * worldTranslation * worldRotate;
							Column column = inputData[x][z];
							Color color = column.IsActive ? Color.White : Color.Black;

							//Draw input bit square
							this.bit.Draw ( world, this.viewMatrix, this.projectionMatrix, color, alphaValue );
						}
					}

				}

			}
			catch (Exception)
			{
				//occasionally data is in transition and causes exception, abort draw when this is the case
				return;
			}


		}




		#endregion



		#region Update

		/// <summary>
		/// Rotation angle for htm objects in world space
		/// </summary>
		/// <param name="diffX"></param>
		/// <param name="diffY"></param>
		public void RotateWorldSpaceHtmObjects(float diffX, float diffY)
		{
			this.yawHtm += diffX * rotateSpeedCamera;
			this.pitchHtm += diffY * rotateSpeedCamera;
		}

		#endregion



		#region Events

		/// <summary>
		/// Event capturing the consturction of a draw surgace and makes sure this gets redirected to
		/// a predesignated drawsurface marked by pointer drawSurface
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = this.drawSurface;

			e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = Viewer3D.Form.pictureBoxSurface.Width;
			e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = Viewer3D.Form.pictureBoxSurface.Height;
		}

		/// <summary>
		/// Reset camera to default position, rotation angle and zoom factor values
		/// </summary>
		internal void ResetCamera()
		{
			//position camera relative to region size
			//Y=4 slightly raised
			//pitch = -10 look slightly down
			//Z=size/3 + 3 - shift to right to give angled view (+3 provides shift for small regions)
			////this.posCamera = new Vector3 ( -25, 4, GetSize ().X / 3 + 3 );
			this.posCamera = new Vector3 ( 30, 4, GetSize ().X / 3 + 3 );

			//Reset rotation angle for camera
			////this.yawCamera = (float)MathHelper.ToRadians ( -90 );
			this.yawCamera = (float)MathHelper.ToRadians ( 90 );
			this.pitchCamera = (float)MathHelper.ToRadians(-10);

			//Reset rotation angle for htm objects
			this.yawHtm = 0f;
			this.pitchHtm = 0f;

			//Reset zoom
			this.zoomCamera = 35f;

			this.UpdateCamera();


		}

		/// <summary>
		/// Move camera according to rotation angle and zoom factor
		/// </summary>
		private void UpdateCamera()
		{
			this.aspectRatio =
				(float)this.GraphicsDevice.PresentationParameters.BackBufferWidth /
				this.GraphicsDevice.PresentationParameters.BackBufferHeight;

			camera = new FPSCamera(this.aspectRatio, this.lookAt)
			{
				Position = this.posCamera,
				Zoom = this.zoomCamera,
				Pitch = this.pitchCamera,
				Yaw = this.yawCamera
			};
			this.viewMatrix = camera.getViewMatrix;
			this.projectionMatrix = camera.getProjectionMatrix;
		}

		/// <summary>
		/// Calculate size of the region in world coordinates
		/// </summary>
		/// <returns>Vector3 size</returns>
		private Vector3 GetSize()
		{
			Matrix worldTranslationZ = Matrix.CreateTranslation(new Vector3(0, 0, zHtmRegion));
			Matrix worldTranslation;
			Matrix worldRotate = Matrix.CreateRotationX( this.pitchHtm ) * Matrix.CreateRotationY( this.yawHtm );
			Vector3 size = new Vector3();

			foreach (List<Column> colList in this.HtmRegionColumns)
			{
				foreach (Column column in colList)
				{
					foreach (Cell cell in column.Cells)
					{
						//calculate cell world coordinates
						var translationVector = new Vector3(column.X, cell.Index, column.Y);
						worldTranslation = Matrix.CreateTranslation(translationVector) * worldTranslationZ;

						//Z extent for initial camera position
						size.X = Math.Max(translationVector.X, size.X);
						size.Y = Math.Max(translationVector.Y, size.Y);
						size.Z = Math.Max(translationVector.Z, size.Z);
					}
				}
			}
			return size;
		}

		#endregion



		#region Picking

		public void Pick(System.Drawing.Point location, bool bSelectionEnable)
		{
			//this.GraphicsDevice.Clear ( this._clearColor );

			////////////debug js
			////////////ray = Simulation3D.Engine.getPickingRay ( new System.Drawing.Point(440, 448 ));
			////////////ray = Simulation3D.Engine.getPickingRay ( new System.Drawing.Point ( 428, 444 ) );
			////////////ray = Simulation3D.Engine.getPickingRay ( new System.Drawing.Point ( 211, 332 ) );
			Ray ray = Viewer3D.Engine.getPickingRay ( location );

			//if (Properties.Settings.Default.StealthMode)
			//	ray = Simulation3D.Engine.getPickingRay ( new System.Drawing.Point ( 331, 301 ) );


			//// Draw Legend:
			//this.DrawLegend ();

			//// Draw HTM
			//this.PickHtmPlane ();

			this.PickHtmRegion ( ray, bSelectionEnable );
			//this.PickHtmRegion ( true );

			//// Draw Prediction Plane
			//this.PickHtmRegionPrediction ();

			//// Draw Active Columns Plane
			//this.PickHtmActiveCols ();

		}

		public Ray getPickingRay(System.Drawing.Point mousePosition)
		{
			mouseLocationClick = new Point ( mousePosition.X, mousePosition.Y );

			Vector3 nearPoint = new Vector3 ( mousePosition.X, mousePosition.Y, 0.0f );
			Vector3 farPoint = new Vector3 ( mousePosition.X, mousePosition.Y, 0.999999f );

			Matrix worldRotate = Matrix.CreateRotationX ( this.pitchHtm ) * Matrix.CreateRotationY ( this.yawHtm );

			Vector3 nearPointWorld = this.GraphicsDevice.Viewport.Unproject ( nearPoint, this.projectionMatrix, this.viewMatrix, worldRotate );
			Vector3 farPointWorld = this.GraphicsDevice.Viewport.Unproject ( farPoint, this.projectionMatrix, this.viewMatrix, worldRotate );

			Vector3 direction = farPointWorld - nearPointWorld;
			direction.Normalize ();

			Ray ray = new Ray ( nearPointWorld, direction );
			return ray;
		}

		public void PickHtmRegion (Ray ray, bool bSelectionEnable)
		{
			Matrix worldTranslationZ = Matrix.CreateTranslation ( new Vector3 ( 0, 0, zHtmRegion ) );
			Matrix worldTranslation;
			Matrix worldScale;
			Matrix worldRotate = Matrix.CreateRotationX ( this.pitchHtm ) * Matrix.CreateRotationY ( this.yawHtm );
			float distance;
			float nearestDistance = float.MaxValue;
			Cell returnedCell = null;
			Cell nearestCell = null;
			SynapseProximal returnedProximalSynapse = null;
			SynapseProximal nearestProximalSynapse = null;
			SynapseBasal returnedBasalSynapse = null;
			SynapseBasal nearestBasalSynapse = null;
			SynapseApical returnedApicalSynapse = null;
			SynapseApical nearestApicalSynapse = null;

			//this is a two-step process
			//Each Pick..() function retuns the "returned..." object which is the nearest object found for this call (eg for this cell).
			//Then, back in this function, the ultimate nearest object is determined (from all the object types) to be the ultimately picked object.

			foreach ( List<Column> colList in this.HtmRegionColumns )
			{
				foreach ( Column column in colList )
				{
					returnedProximalSynapse = null;

					foreach ( Cell cell in column.Cells )
					{
						returnedCell = null;
						returnedBasalSynapse = null;

						Vector3 translationVector = new Vector3 ( column.X, cell.Index, column.Y );

						worldScale = Matrix.CreateScale ( new Vector3 ( 0.2f, 0.2f, 0.2f ) );
						worldTranslation = Matrix.CreateTranslation ( translationVector ) * worldTranslationZ;
						//Matrix world = worldScale * worldTranslation * worldRotate;
						Matrix world = worldScale * worldTranslation;

						distance = PickCell ( ray, world, column, cell, ref returnedCell );
						TrackNearestObject ( distance, ref nearestDistance,
											returnedCell, ref nearestCell,
											returnedBasalSynapse, ref nearestBasalSynapse,
											returnedProximalSynapse, ref nearestProximalSynapse,
											returnedApicalSynapse, ref nearestApicalSynapse );

						//Pick synapse connections
						distance = PickBasalSynapseConnections ( ray, column, cell, ref returnedBasalSynapse );
						TrackNearestObject ( distance, ref nearestDistance,
											returnedCell, ref nearestCell,
											returnedBasalSynapse, ref nearestBasalSynapse,
											returnedProximalSynapse, ref nearestProximalSynapse,
											returnedApicalSynapse, ref nearestApicalSynapse );
					}

					//Pick proximal synapse connections
					distance = PickProximalSynapseConnections ( ray, column, ref returnedProximalSynapse );
					TrackNearestObject ( distance, ref nearestDistance,
										returnedCell, ref nearestCell,
										returnedBasalSynapse, ref nearestBasalSynapse,
										returnedProximalSynapse, ref nearestProximalSynapse,
										returnedApicalSynapse, ref nearestApicalSynapse );

					//Pick apical synapse connections
					distance = PickApicalSynapseConnections ( ray, column, ref returnedApicalSynapse );
					TrackNearestObject ( distance, ref nearestDistance,
										returnedCell, ref nearestCell,
										returnedBasalSynapse, ref nearestBasalSynapse,
										returnedProximalSynapse, ref nearestProximalSynapse,
										returnedApicalSynapse, ref nearestApicalSynapse );
				}

				//Find and process nearest object picked
				Selectable3DObject selObject = null;
				if ( nearestDistance < float.MaxValue )
				{
					if ( nearestCell != null )
					{
						selObject = nearestCell;
						selObject.mouseOver = true;
					}
					if ( nearestBasalSynapse != null )
					{
						selObject = nearestBasalSynapse;
						selObject.mouseOver = true;
					}
					if ( nearestProximalSynapse != null )
					{
						selObject = nearestProximalSynapse;
						selObject.mouseOver = true;
					}
					if ( nearestApicalSynapse != null )
					{
						selObject = nearestApicalSynapse;
						selObject.mouseOver = true;
					}

					if ( bSelectionEnable && selObject != null )
					{
						if ( !selObject.mouseSelected )
							selObject.mouseSelected = true;
						else
							selObject.mouseSelected = false;
					}

					////SHIFT key - deselect
					//if (keyState.IsKeyDown ( Microsoft.Xna.Framework.Input.Keys.LeftShift ) || keyState.IsKeyDown ( Microsoft.Xna.Framework.Input.Keys.RightShift ))
					//	selObject.mouseSelected = false;

					//update selected object list
					//UpdateSelectedObjectList ( selObject, selObject.mouseSelected ); start here
					UpdateSelectedObjectList ( this.Region, selObject.mouseSelected );
				}
			}

			//if (selObject != null)
			//{

			//	//test only
			//	DataSet ds = ViewListToDataset_Cells ( );
			//}
		}

		private float PickCell(Ray ray, Matrix worldTranslation, Column column, Cell cell, ref Cell returnedCell)
		{
			cell.mouseOver = false;

			BoundingBox box = this.cube.GetBoundingBox ( worldTranslation );

			float? intersectDistance = ray.Intersects ( box );

			if(intersectDistance != null )
			{
				returnedCell = cell;
				return (float)intersectDistance;
			}

			return float.MaxValue;
		}

		private float PickProximalSynapseConnections (Ray ray, Column column, ref SynapseProximal returnedProximalSynapse)
		{
			float intersectDistance = float.MaxValue;
			float minDistance = float.MaxValue;

			returnedProximalSynapse = null;

			//Draw Connections if existing
			if ( column.IsDataGridSelected || ( Viewer3D.Form.ShowSpatialLearning && column.IsActive ) )
			{
				Vector3 rayP1 = ray.Position;
				Vector3 rayP2 = rayP1 + ray.Direction;

				foreach ( SynapseProximal synapse in column.ProximalDendrite.Synapses )
				{
					synapse.mouseOver = false;

					//TODO: implement Statictics concept for my elements...
					//if ( column.Statistics.StepCounter > 0 )
					//{
						//Get the two vectors to draw line between
						var startPosition = new Vector3 ( column.X, 0, column.Y + zHtmRegion );

						//Get input plane position   TODO: straighten out xyz here....
						int x = synapse.ColumnConnected.X;
						int y = (int)yHtmPlane;
						int z = synapse.ColumnConnected.Y;
						var endPosition = new Vector3 ( x, y, z + zHtmPlane );

						bool intersect;
						Vector3 Line1ClosestPt = new Vector3 ();
						Vector3 Line2ClosestPt = new Vector3 ();
						intersect = Math3D.ClosestPointsLineSegmentToLine ( out Line1ClosestPt, out Line2ClosestPt, startPosition, endPosition, rayP1, rayP2, 0.1f, out intersectDistance );

						if (intersect && intersectDistance < minDistance)
						{
							minDistance = intersectDistance;
							returnedProximalSynapse = synapse;
						}
					//}
				}
			}

			return minDistance;
		}

		private float PickBasalSynapseConnections (Ray ray, Column column, Cell cell, ref SynapseBasal returnedBasalSynapse)
		{
			float intersectDistance = float.MaxValue;
			float minDistance = float.MaxValue;

			returnedBasalSynapse = null;

			//Draw Connections if existing
			if ( cell.IsDataGridSelected || ( Viewer3D.Form.ShowSpatialLearning && cell.IsPredicting ) )
			{
				Vector3 rayP1 = ray.Position;
				Vector3 rayP2 = rayP1 + ray.Direction;

				foreach ( SynapseBasal synapse in cell.BasalDendrite.Synapses )
				{
					synapse.mouseOver = false;

					var basalSynapse = synapse as SynapseBasal;

					//Get the two vectors to draw line between
					var startPosition = new Vector3 ( column.X, cell.Index, column.Y + zHtmRegion );

					//Get input plane position   TODO: straighten out xyz here....
					int x = synapse.ColumnConnected.X;
					int y = synapse.ColumnConnected.Cells[0].Index; //TODO: - this just = 0. It point to first cell in connected column. May need a better solution to represent connection to entire column
					int z = synapse.ColumnConnected.Y;
					var endPosition = new Vector3 ( x, y, x + zHtmPlane );

					bool intersect;
					Vector3 Line1ClosestPt = new Vector3 ();
					Vector3 Line2ClosestPt = new Vector3 ();
					intersect = Math3D.ClosestPointsLineSegmentToLine ( out Line1ClosestPt, out Line2ClosestPt, startPosition, endPosition, rayP1, rayP2, 0.1f, out intersectDistance );

					if ( intersect && intersectDistance < minDistance )
					{
						minDistance = intersectDistance;
						returnedBasalSynapse = synapse;
					}

				}
			}

			return minDistance;
		}

		private float PickApicalSynapseConnections (Ray ray, Column column, ref SynapseApical returnedApicalSynapse)
		{
			float intersectDistance = float.MaxValue;
			float minDistance = float.MaxValue;

			//TODO: implement later

			//returnedApicalSynapse = null;

			////Draw Connections if existing
			//if ( column.IsDataGridSelected || ( Viewer3D.Form.ShowSpatialLearning && column.IsActive ) )
			//{
			//	Vector3 rayP1 = ray.Position;
			//	Vector3 rayP2 = rayP1 + ray.Direction;

			//	foreach ( SynapseApical synapse in column.ApicalDendrite.Synapses )
			//	{
			//		synapse.mouseOver = false;

			//		if ( column.Statistics.StepCounter > 0 )
			//		{
			//			//Get the two vectors to draw line between
			//			var startPosition = new Vector3 ( column.X, 0, column.Y + zHtmRegion );

			//			//Get input plane position   TODO: straighten out xyz here....
			//			int x = synapse.ColumnConnected.X;
			//			int y = (int)yHtmPlane; //TODO: need Y of the other layer here....
			//			int z = synapse.ColumnConnected.Y;
			//			var endPosition = new Vector3 ( x, y, x + zHtmPlane );

			//			bool intersect;
			//			Vector3 Line1ClosestPt = new Vector3 ();
			//			Vector3 Line2ClosestPt = new Vector3 ();
			//			intersect = Math3D.ClosestPointLineSegmentToLine ( out Line1ClosestPt, out Line2ClosestPt, startPosition, endPosition, rayP1, rayP2, out intersectDistance );

			//			if ( intersect && intersectDistance < minDistance )
			//			{
			//				minDistance = intersectDistance;
			//				returnedApicalSynapse = synapse;
			//			}
			//		}
			//	}
			//}

			return minDistance;
		}

		//Keep track of nearest object
		private void TrackNearestObject(float distance, ref float nearestDistance, 
			Cell returnedCell, ref Cell nearestCell,
			SynapseBasal returnedBasalSynapse, ref SynapseBasal nearestBasalSynapse, 
			SynapseProximal returnedProximalSynapse, ref SynapseProximal nearestProximalSynapse,
			SynapseApical returnedApicalSynapse, ref SynapseApical nearestApicalSynapse)
		{
			if(distance < nearestDistance )
			{
				if(returnedCell != null)
				{
					nearestCell = returnedCell;
					nearestBasalSynapse = null;
					nearestProximalSynapse = null;
					nearestApicalSynapse = null;
				}
				if ( returnedBasalSynapse != null )
				{
					nearestCell = null;
					nearestBasalSynapse = returnedBasalSynapse;
					nearestProximalSynapse = null;
					nearestApicalSynapse = null;
				}
				if ( returnedProximalSynapse != null )
				{
					nearestCell = null;
					nearestBasalSynapse = null;
					nearestProximalSynapse = returnedProximalSynapse;
					nearestApicalSynapse = null;
				}
				if ( returnedApicalSynapse != null )
				{
					nearestCell = null;
					nearestBasalSynapse = null;
					nearestProximalSynapse = null;
					nearestApicalSynapse = returnedApicalSynapse;
				}
			}
		}

		public void UpdateSelectedObjectList(object obj, bool add)
		{
			//Trigger selection changed event
			SelectionChangedEvent ( this, EventArgs.Empty, Program.netForm1.Net.Lr );
		}


		#endregion Picking


	}
}
