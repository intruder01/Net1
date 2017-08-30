using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Net1
{
	public delegate void SimEngineStarted_Event (object sender, EventArgs e);
	public delegate void SimEngineShutdown_Event (object sender, EventArgs e);
	public delegate void SimEngineSelectionChanged_Event (object sender, EventArgs e, object obj);
	
	
	/// <summary>
	/// Main class for 3D viewer implementation
	/// </summary>
	public class Viewer3DEngine : Game
	{
		public static event SimEngineStarted_Event EngineStarted = delegate { };
		public static event SimEngineShutdown_Event EngineShutdown = delegate { };
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
		private float[,] activeColumns;

		#endregion


		#region Graphics

		private GraphicsDeviceManager graphics;
		private IntPtr drawSurface;

		// Global Matrices
		private Matrix viewMatrix;
		private Matrix projectionMatrix;
		
		// Coordinates constants
		private const float zHtmRegion = 0.0f;    //was-5.0f
		private const float zHtmPlane = 0f; // Shift to the side //was -5.0f 
		private const float yHtmPlane = -5f; // Shift down  was -5f

		private bool contentLoaded; //used in LoadContent()

		private SpriteBatch spriteBatch;
		private SpriteFont spriteFont;
		private Color clearColor = Color.CornflowerBlue;

		// Colors
		public Dictionary<HtmCellColors, HtmColorInformation> dictionaryCellColors;
		private Dictionary<HtmProximalSynapseColors, HtmColorInformation> dictionaryProximalSynapseColors;
		private Dictionary<HtmDistalSynapseColors, HtmColorInformation> dictionaryDistalSynapseColors;

		// Right Legend-Elements
		private HtmOverViewInformation rightLegend;

		// Primitives
		private CubePrimitive cube;
		private CoordinateSysPrimitive coordinateSystem;
		private SquarePrimitve bit;
		private LinePrimitive connectionLine;
		private PyramidPrimitive pyramid;
		private OctahedronPrimitive octahedron;


		//input state varialbles
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

		private const int gridHeight = 7;
		private const int gridWidth = 7;

		//Buffering space dimansions
		private const int gridHeightBuffer = 5;
		private const int gridWidthBuffer = 10;


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
		public List<List<Column>> InputPlaneColumns { get; private set; }

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
			Control.FromHandle(this.Window.Handle).VisibleChanged += this.Viever3D_VisibleChanged;

			IsFixedTimeStep = false;

			//listen to Form events
			Viewer3DForm.Viewer3DFormKeyPressedEvent += Handler_Viewer3DKeyForm_KeyPressed_Event;
			Viewer3DForm.Viewer3DFormClosingEvent += Handler_Viewer3DKeyForm_Closing_Event;
		}

		~Viewer3DEngine ()
		{
			//Stop monitoring Form key events
			Viewer3DForm.Viewer3DFormKeyPressedEvent -= Handler_Viewer3DKeyForm_KeyPressed_Event;
		}

		#endregion

		#region Methods

		#endregion



		#region Form

		/// <summary>
		/// Occurs when original windows' visibility changs and makes sure it stays invisible 
		/// </summary>
		private void Viever3D_VisibleChanged(object sender, EventArgs e)
		{
			Control.FromHandle ( this.Window.Handle ).Visible = false;
		}

		/// <summary>
		/// Respond to key down events from the Form.
		/// Move camera.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Handler_Viewer3DKeyForm_KeyPressed_Event(object sender, KeyEventArgs e)
		{
			//store input variables for this scan
			System.Windows.Forms.Keys key = e.KeyCode;

			// X = left/right
			if ( key == System.Windows.Forms.Keys.Right || key == System.Windows.Forms.Keys.D )
				AddToCameraPosition ( new Vector3 ( -sensitivity, 0, 0 ) );
			if ( key == System.Windows.Forms.Keys.Left || key == System.Windows.Forms.Keys.A )
				AddToCameraPosition ( new Vector3 ( sensitivity, 0, 0 ) );

			// Y = up/dn
			if ( key == System.Windows.Forms.Keys.Up || key == System.Windows.Forms.Keys.W )
				AddToCameraPosition ( new Vector3 ( 0, -sensitivity, 0 ) );
			if ( key == System.Windows.Forms.Keys.Down || key == System.Windows.Forms.Keys.S )
				AddToCameraPosition ( new Vector3 ( 0, sensitivity, 0 ) );

			// Z = in/out
			if ( key == System.Windows.Forms.Keys.Q )
				AddToCameraPosition ( new Vector3 ( 0, 0, -sensitivity ) );
			if ( key == System.Windows.Forms.Keys.E )
				AddToCameraPosition ( new Vector3 ( 0, 0, sensitivity ) );
		}

		/// <summary>
		/// Stop monitoring Form key events when form closing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Handler_Viewer3DKeyForm_Closing_Event (object sender, FormClosingEventArgs e)
		{
			//Stop monitoring Form key events
			Viewer3DForm.Viewer3DFormKeyPressedEvent -= Handler_Viewer3DKeyForm_KeyPressed_Event;
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
				this.dictionaryCellColors.Add(HtmCellColors.Inhibited, new HtmColorInformation(Color.Yellow, "Cell is inhibited"));

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
				this.InputPlaneColumns = Program.netForm1.Net.Ip.Columns;

				//Prepare Arrays for 2-dim-content
				this.predictions = new float[this.Region.NumColumnsX, this.Region.NumColumnsY];
				this.activeColumns = new float[this.Region.NumColumnsX, this.Region.NumColumnsY];

				//prepare Cube
				this.cube = new CubePrimitive(this.GraphicsDevice);
				this.coordinateSystem = new CoordinateSysPrimitive(this.GraphicsDevice);
				this.bit = new SquarePrimitve(this.GraphicsDevice);
				this.connectionLine = new LinePrimitive(this.GraphicsDevice);
				this.pyramid = new PyramidPrimitive ( this.GraphicsDevice );
				this.octahedron = new OctahedronPrimitive ( this.GraphicsDevice );

				this.ResetCamera();

				this.contentLoaded = true;

				//send notification Engine started
				EngineStarted?.Invoke ( this, new EventArgs () );
			}
		}


		public static Texture2D LoadTexture2D(GraphicsDevice graphicsDevice, string path)
		{
			return Texture2D.FromStream(graphicsDevice, new StreamReader(path).BaseStream);
		}


		#endregion Content





		#region Draw

		protected override void Draw(GameTime gameTime)
		{
			this.GraphicsDevice.Clear(this.clearColor);

			////TEST only
			////this.DrawHtmInputPlane ();
			//this.DrawTest ();

			//Draw Legend
			this.DrawLegend ();

			//Draw HTM
			this.DrawHtmInputPlane ();
			//this.DrawHtmRegion ( false );
			this.DrawHtmRegion ( true );

			//Draw Prediction Plane
			this.DrawHtmRegionPredictionPlane ();
			this.DrawHtmRegionPredictionReconstructionPlane ();  //20160109 - 1

			//TOCONTINUE
			//Draw Active Columns Plane
			this.DrawHtmActiveColsPlane ();

			//Draw CoordinateSystem
			if ( Viewer3D.Form.ShowCoordinateSystem)
			{
				this.coordinateSystem.Draw(this.viewMatrix, this.projectionMatrix);
			}


			base.Draw(gameTime);		
		}

		private void FillHtmOverview(object element)
		{
			//TOCONTINUE
			//var statistics = Statistics ();


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

		private void DrawHtmActiveColsPlane ()
		{
			if ( !Viewer3D.Form.ShowActiveColumnGrid )
				return;

			//Compute starting point
			int x = this.GraphicsDevice.PresentationParameters.BackBufferWidth -
					this.activeColumns.GetLength ( 1 ) * ( gridWidth + gridWidthBuffer );
			int y = this.GraphicsDevice.PresentationParameters.BackBufferHeight -
					this.activeColumns.GetLength ( 0 ) * ( gridHeight + gridHeightBuffer ) - 50;
			var startVectorLeft = new Vector2 ( x, y );

			this.DrawHtmActivationMap ( startVectorLeft, this.activeColumns, "Active Columns:" );
		}


		private void DrawHtmInputPlane()
		{
			try
			{
				//Get input data from fileSensor. Attention: Draw rythm happens very often!
				//TODO: they use the Global.T concept to access data according to T steps back
				//I did not implement it here as I see no need for it 
				InputPlane ip = Program.netForm1.Net.Ip;
				List<List<Column>> inputData = Program.netForm1.Net.Ip.Columns;

				if(inputData != null)
				{
					const float alphaValue = 0.9f;

					int regionWidth = ip.NumColumnsX;
					int regionHeight = ip.NumColumnsY;

					Matrix worldTranslationBehindDown = Matrix.CreateTranslation ( new Vector3 ( 0, yHtmPlane, 0 ) ) 
													* Matrix.CreateTranslation ( new Vector3 ( 0, 0, zHtmPlane ) );
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
							Column column = inputData[z][x];
							Color color = column.IsActive ? Color.Black : Color.White;

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

		private void DrawTest ()
		{
			try
			{
				//Get input data from fileSensor. Attention: Draw rythm happens very often!
				//TODO they use the Global.T concept to access data according to T steps back
				//I did not implement it here as I see no need for it 
				////////InputPlane ip = Program.netForm1.Net.Ip;
				////////List<List<Column>> inputData = Program.netForm1.Net.Ip.Columns;
				//int[,] inputData = Program.netForm1.Net.Ip.Columns;

				float alphaValue = 0.9f;

				//initial positin in world
				//Matrix worldTranslationInitial = Matrix.CreateTranslation ( new Vector3 ( 0, 0, 0 ) ) * Matrix.CreateTranslation ( new Vector3 ( 0, 5, 0 ) ) * Matrix.CreateTranslation ( new Vector3 ( 0, 0, 0 ) );
				Matrix worldTranslationInitial = Matrix.CreateTranslation ( new Vector3 ( 0, 0, 0 ) );
				Matrix worldTranslation;
				Matrix world;
				float Scale = 0.8f;
				Matrix worldScale = Matrix.CreateScale ( new Vector3 ( Scale, Scale, Scale ) );
				Matrix worldRotate = Matrix.CreateRotationX ( this.pitchHtm ) * Matrix.CreateRotationY ( this.yawHtm );
				float cf = 1.0f;

				//////if ( inputData != null )
				//////{
					for ( int x = 0; x < 10; x++ )
					{
						for ( int y = 0; y < 10; y++ )
						{
							for ( int z = 0; z < 10; z++ )
							{
								worldTranslation = worldTranslationInitial * Matrix.CreateTranslation ( new Vector3 ( x * cf, y * cf, z * cf ) );
								world = worldScale * worldTranslation * worldRotate;

								//Draw input bit square
								if ( x == 0 )
									this.cube.Draw ( world, this.viewMatrix, this.projectionMatrix, Color.White, alphaValue );
								else
									this.cube.Draw ( world, this.viewMatrix, this.projectionMatrix, Color.Brown, alphaValue );
							}
						}
					}
				//////}

				//float Scale = 5.3f;
				//Matrix worldScale = Matrix.CreateScale ( new Vector3 ( Scale, Scale, Scale ) );
				//Matrix worldRotate = Matrix.CreateRotationX ( this.pitchHtm ) * Matrix.CreateRotationY ( this.yawHtm );

				//Matrix worldTranslationInitial = Matrix.CreateTranslation ( new Vector3 ( 2, 2, 0 ) );
				//float cf = 1.0f;
				//Matrix worldTranslation = worldTranslationInitial * Matrix.CreateTranslation ( new Vector3 ( 0, 0, 0 ) );
				//Matrix world = worldScale * worldTranslation * worldRotate;

				//const float alphaValue = 0.99f;
				//this.octahedron.Draw ( world, this.viewMatrix, this.projectionMatrix, Color.TransparentBlack, alphaValue );

				Scale = 0.99f;
				worldScale = Matrix.CreateScale ( new Vector3 ( Scale, Scale, Scale ) );
				worldRotate = Matrix.CreateRotationX ( this.pitchHtm ) * Matrix.CreateRotationY ( this.yawHtm );

				worldTranslationInitial = Matrix.CreateTranslation ( new Vector3 ( 5, 5, 0 ) );
				worldTranslation = worldTranslationInitial * Matrix.CreateTranslation ( new Vector3 ( 0, 0, 0 ) );
				world = worldScale * worldTranslation * worldRotate;

				alphaValue = 0.99f;
				this.octahedron.Draw ( world, this.viewMatrix, this.projectionMatrix, Color.TransparentBlack, alphaValue );

			}
			catch ( Exception )
			{
				//occasionally data is in transition and causes exception, abort draw when this is the case
				return;
			}


		}

		private void DrawHtmRegion(bool inactiveCells)
		{
			Matrix worldTranslationZ = Matrix.CreateTranslation ( new Vector3 ( 0, 0, zHtmRegion ) );
			Matrix worldTranslation;
			Matrix worldScale;
			Matrix worldRotate = Matrix.CreateRotationX ( this.pitchHtm ) * Matrix.CreateRotationY ( this.yawHtm );
			var sumOf3DCoordinates = new Vector3 ();
			var regionCenter = new Vector3 ();
			int numberPointsToCalculateAverageOfCenter = 0;

			try
			{
				this.FillHtmOverview (this.Region);

				foreach ( List<Column> colY in this.HtmRegionColumns )
				{
					foreach ( Column column in colY )
					{
						int predictionCounter = 0;
						column.isVisible = false;

						foreach ( Cell cell in column.Cells )
						{
							if ( column.IsDataGridSelected )
							{
								this.FillHtmOverview ( column );
							}

							if ( cell.IsDataGridSelected )
							{
								this.FillHtmOverview ( cell );
							}

							//calculate cell world coordinates
							var translationVector = new Vector3 ( column.X, cell.Index, column.Y );
							worldTranslation = Matrix.CreateTranslation ( translationVector ) * worldTranslationZ;


							if ( inactiveCells )
							{
								//Calculate region center
								sumOf3DCoordinates += translationVector;
								numberPointsToCalculateAverageOfCenter ++;
							}

							Color color;
							float alphaValue;
							this.GetColorFromCell ( cell, column, out color, out alphaValue );

							//if ( ( !inactiveCells && alphaValue < 1.0f ) || ( inactiveCells && alphaValue == 1.0f ) )
							//{
							//	continue;
							//}

							//Check for cell selection
							if ( column.IsDataGridSelected )
							{
								alphaValue = 1.0f;
								color = this.dictionaryCellColors[HtmCellColors.Selected].HtmColor;
								worldScale = Matrix.CreateScale ( new Vector3 ( 0.5f, 0.5f, 0.5f ) );
							}
							
							if(cell.IsDataGridSelected)
							{
								alphaValue = 1.0f;
								worldScale = Matrix.CreateScale ( new Vector3 ( 0.5f, 0.5f, 0.5f ) );
							}
							else
							{
								worldScale = Matrix.CreateScale ( new Vector3 ( 0.3f, 0.3f, 0.3f ) );
							}

							if(cell.IsPredicting)
							{
								//Closed for region prediction visualization 
								predictionCounter++;
							}

							if ( cell.mouseSelected )
								color = Color.Red;

							if(cell.mouseOver)
							{
								color = mouseOverColor;
							}

							//apply scale factor
							Matrix world = worldScale * worldTranslation * worldRotate;

							//Draw cube 
							this.cube.Draw ( world, this.viewMatrix, this.projectionMatrix, color, alphaValue );

							//Draw synapse connection
							this.DrawBasalSynapseConnections ( ref worldTranslation, ref worldRotate, column, cell );

							
							//set isVisible property if cell is displayed
							if ( alphaValue > 0.11 )
							{
								cell.isVisible = true;
								column.isVisible = true;
							}
							else
							{
								cell.isVisible = false;
							}
						}

						// 20160090-1
						if(!inactiveCells)
						{
							//Send column indices with actual proecition value in 2-dim Array
							float result = predictionCounter / (float)Program.netForm1.Net.Lr.NumCellsInColumn;
							this.predictions[column.X, column.Y] = result;
						}

						//Draw proximal synapse connections
						this.DrawProximalSynapseConnections ( ref worldRotate, column );

						//Define value to draw ColumnMap
						{
							if ( column.IsActive )
								this.activeColumns[column.X, column.Y] = 1;
							else
								this.activeColumns[column.X, column.Y] = 0;
						}
					}
				}
			}
			catch(Exception)
			{

			}

			if(inactiveCells)
			{
				//Calculate region center to focus camera on
				regionCenter = new Vector3 (
					sumOf3DCoordinates.X / numberPointsToCalculateAverageOfCenter,
					sumOf3DCoordinates.Y / numberPointsToCalculateAverageOfCenter,
					sumOf3DCoordinates.Z / numberPointsToCalculateAverageOfCenter );
				regionCenter.Z += zHtmRegion;
				this.lookAt = regionCenter;
			}
		}


		private void DrawHtmRegionPredictionPlane()
		{
			if ( !Viewer3D.Form.ShowPredictedGrid )
				return;

			int x = 10;
			int y = this.GraphicsDevice.PresentationParameters.BackBufferHeight - this.activeColumns.GetLength ( 0 ) *
				( gridHeight + gridHeightBuffer ) - 50;

			Vector2 startVectorLeft = new Vector2 ( x, y );
			//20160109-1
			this.DrawHtmActivationMap ( startVectorLeft, this.predictions, "Region Prediction" );
		}

		public void DrawHtmRegionPredictionReconstructionPlane() // 20160109-1
		{
			if(!Viewer3D.Form.ShowPredictionReconstructiondGrid)
			{
				return;
			}

			//prediction reconstruction commented out in this commit
			//float[,] inputPredictionReconstruction = this.Region.PredictionReconstruction;//20170818-1
			float[,] inputPredictionReconstruction = new float[5,5]; // dummy

			int x = 10;
			int y = this.GraphicsDevice.PresentationParameters.BackBufferHeight -
					this.activeColumns.GetLength ( 0 ) * ( gridHeight + gridHeightBuffer ) - 50;
			var startVectorLeft = new Vector2 ( x, y );
			//20160109-1
			this.DrawHtmInputPredictionReconstructionMap ( startVectorLeft, inputPredictionReconstruction, "Input Prediction" );
		}

		private void DrawBasalSynapseConnections(ref Matrix worldTranslation,
			ref Matrix worldRotate, Column column, Cell cell)
		{
			try
			{
				//Draw connections if existing
				foreach ( SynapseBasal synapse in cell.BasalDendrite.Synapses )
				{
					//if(cell.IsDataGridSelected 
					//|| (Viewer3D.Form.ShowTemporalLearning && cell.IsPredicting))
				if ( cell.IsDataGridSelected
					|| ( Viewer3D.Form.ShowTemporalLearning && cell.isVisible ) )
					{
						//Get the two vectors to draw line between
						Vector3 startPosition = new Vector3 ( column.X,
							cell.Index, column.Y );

						//Get input source position
						int x = synapse.ColumnConnected.X;
						int y = 0;
						int z = synapse.ColumnConnected.Y;
						Vector3 endPosition = new Vector3 ( x, y, z );

						//Color color = synapse.IsActive ? Color.Black : Color.White

						Color color;
						float alphaValue;
						GetColorFromBasalSynapse ( synapse, out color, out alphaValue );
						this.connectionLine.SetUpVertices ( startPosition, endPosition, color );

						//Draw line
						this.connectionLine.Draw ( worldTranslation * worldRotate, this.viewMatrix, this.projectionMatrix );

						synapse.isVisible = true;
						column.isVisible = true;
					}
					else
					{
						synapse.isVisible = false;
					}
				}
			}
			catch ( Exception )
			{
				// Is sometimes raised because of collections modification by another thread.
			}
		}

		private void DrawProximalSynapseConnections(ref Matrix worldRotate, Column column)
		{
			try
			{
				//Draw connections if existing
				//if ( column.IsDataGridSelected || ( Viewer3D.Form.ShowSpatialLearning && column.IsActive ) )
				foreach ( SynapseProximal synapse in column.ProximalDendrite.Synapses)
				{
					//if ( column.IsDataGridSelected 
					//|| ( Viewer3D.Form.ShowSpatialLearning && Viewer3D.Form.ShowActiveCells && column.IsActive )
					//|| ( Viewer3D.Form.ShowSpatialLearning && Viewer3D.Form.ShowInhibitedColumns && column.IsInhibited ) )
				if ( column.IsDataGridSelected
				|| ( Viewer3D.Form.ShowSpatialLearning && column.isVisible ) )
					{
						//Get the two vectors to draw line between
						Vector3 startPosition = new Vector3 ( column.X, 0, column.Y + zHtmRegion );

						//Get input source position
						int x = synapse.ColumnConnected.X;
						int y = (int)yHtmPlane;
						int z = synapse.ColumnConnected.Y;
						Vector3 endPosition = new Vector3 ( x, y, z );

						Color color;
						float alphaValue;
						GetColorFromProximalSynapse ( synapse, out color, out alphaValue );
						this.connectionLine.SetUpVertices ( startPosition, endPosition, color );

						//Draw line
						this.connectionLine.Draw ( worldRotate, this.viewMatrix, this.projectionMatrix );

						synapse.isVisible = true;
						column.isVisible = true;
					}
					else
					{
						synapse.isVisible = false;
					}
				}

			}
			catch ( Exception )
			{

			}

		}

		/// <summary>
		/// Draws 2d map on screen at wanted position
		/// </summary>
		/// <param name="startVectorLeft"></param>
		/// <param name="mapData"></param>
		/// <param name="title"></param>
		private void DrawHtmActivationMap(Vector2 startVectorLeft, float[,] mapData, string title)
		{
			//Count active elements
			int activeCounter = 0;
			var gridLeftStart = (int)startVectorLeft.X;

			//Draw Prediction Legend
			this.spriteBatch.Begin ( SpriteSortMode.Deferred, BlendState.NonPremultiplied,
									SamplerState.AnisotropicClamp, null, null );
			this.spriteBatch.DrawString ( this.spriteFont, title, startVectorLeft, Color.Black );
			//Go one more line down
			startVectorLeft.Y += gridHeight + gridHeightBuffer;

			for ( int i = mapData.GetLength(0) - 1; i>= 0;  i-- )
			{
				//Go one more line down
				startVectorLeft.Y += gridHeight + gridHeightBuffer;
				for ( int j = 0; j < mapData.GetLength(0); j++ )
				{
					if(mapData[i, j] == 1)
					{
						activeCounter++;
					}

					//Adapt color
					float component = ( 1 - mapData[i, j] ) * 255;
					Color newColor = new Color ( (int)component, (int)component, (int)component );

					this.spriteBatch.Draw ( this.gridTexture,
										new Rectangle ( (int)startVectorLeft.X, (int)startVectorLeft.Y,
														gridWidth, gridHeight ), newColor );
				}
				startVectorLeft.X = gridLeftStart;
			}

			string activeColumnString = "Active Colums per step:" + activeCounter.ToString ();
			startVectorLeft.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, activeColumnString, startVectorLeft, Color.Black );

			this.spriteBatch.End ();

		}

		private void DrawHtmInputPredictionReconstructionMap (Vector2 startVectorLeft, float[,]mapData, string title)
		{
			// Count active elements
			int activeCounter = 0;
			var gridLeftStart = (int)startVectorLeft.X;

			float minPrediction = 0, maxPrediction = 0;

			//Finx max.
			foreach ( var amount in mapData )
			{
				if(amount > maxPrediction)
				{
					maxPrediction = amount;
				}
			}

			// The min feedforward input must be bigger thatn 0 (ignore the inputs who received
			// zero
			minPrediction = maxPrediction;

			//Find min
			foreach ( var amount in mapData )
			{ 
				if(amount < minPrediction && amount > 0)
				{
					minPrediction = amount;
				}
			}

			// Draw prediction legend
			this.spriteBatch.Begin ( SpriteSortMode.Deferred, BlendState.NonPremultiplied,
									SamplerState.AnisotropicClamp, null, null );
			this.spriteBatch.DrawString ( this.spriteFont, title, startVectorLeft, Color.Black );
			// Go one more line down
			startVectorLeft.Y += gridHeight + gridHeightBuffer;

			for ( int i = mapData.GetLength(0); i < 0;	 i-- )
			{
				// Go one nore line down
				startVectorLeft.Y += gridHeight + gridHeightBuffer;
				for ( int j = 0; j < mapData.GetLength(0); j++ )
				{
					if(mapData[i, j] > 0)
					{
						activeCounter++;	//this is arbitrary
					}

					float reconstructionStrength = mapData[i, j];

					if(reconstructionStrength > 0)
					{
						// Adapt color
						// float component = (1 - mapData[i, j] * 255
						float percentageFromReconstructionRange =
							( reconstructionStrength - minPrediction ) /
							( maxPrediction - minPrediction );

						var colorByte = (byte)( ( 1 - percentageFromReconstructionRange ) * byte.MaxValue );
						Color newColor = new Color ( (int)colorByte, (int)colorByte, (int)colorByte );

						this.spriteBatch.Draw ( this.whiteTexture,
									new Rectangle ( (int)startVectorLeft.X, (int)startVectorLeft.Y,
									gridWidth, gridHeight ), newColor );
					}
					else
					{
						//this._spriteBatch.Draw ( this._whiteTexture,
						//					   new Rectangle ( (int)startVectorLeft.X, (int)startVectorLeft.Y,
						//									 _gridWidth, _gridHeight ), Color.White );
					}

					startVectorLeft.X += gridWidth + gridWidthBuffer;
				}
				startVectorLeft.X = gridLeftStart;
			}

			string activeColumnString = "Predicted Inputs: " + activeCounter.ToString ();
			startVectorLeft.Y += gridHeight + gridHeightBuffer;
			this.spriteBatch.DrawString ( this.spriteFont, activeColumnString, startVectorLeft, Color.Black );
			this.spriteBatch.End ();
		}


		private void GetColorFromProximalSynapse(SynapseProximal proximalSynapse, out Color color, out float alphaValue)
		{
			color = this.dictionaryProximalSynapseColors[HtmProximalSynapseColors.Default].HtmColor;
			alphaValue = 0.1f;  //All conditions can be false
			Viewer3DForm visualiserForm = Viewer3D.Form;

			try
			{
				if ( proximalSynapse.IsActive )
				{
					if ( proximalSynapse.IsConnected )
					{
						//this._connectionLine.SetUpVertices ( startPosition, endPosition, Color.Green );
						{
							alphaValue = 1.0f;
							color = this.dictionaryProximalSynapseColors[HtmProximalSynapseColors.ActiveConnected].HtmColor;
						}
					}
					else    //Active
					{
						//this._connectionLine.SetUpVertices ( startPosition, endPosition, Color.Orange );
						alphaValue = 1.0f;
						color = this.dictionaryProximalSynapseColors[HtmProximalSynapseColors.Active].HtmColor;
					}
				}
				else    //Not active
				{
					//this._connectionLine.SetUpVertices ( startPosition, endPosition, Color.White );
					alphaValue = 1.0f;
					color = this.dictionaryProximalSynapseColors[HtmProximalSynapseColors.Default].HtmColor;
				}

				//Selected
				if(proximalSynapse.mouseSelected)
				{
					alphaValue = 1.0f;
					color = selectedColor;
					//color = this._dictionaryProximalSynapseColors[HtmProximalSynapseColors.MouseSelected].HtmColor; //TODO add this color to dictionary
				}

				//mouseOver
				if ( proximalSynapse.mouseOver )
				{
					alphaValue = 1.0f;
					color = mouseOverColor;
					//color = this._dictionaryProximalSynapseColors[HtmProximalSynapseColors.MouseOver].HtmColor; //TODO add this color to dictionary
				}
			}
			catch ( Exception )
			{

			}
		}

		private void GetColorFromBasalSynapse(SynapseBasal basalSynapse, out Color color, out float alphaValue)
		{
			color = this.dictionaryDistalSynapseColors[HtmDistalSynapseColors.Default].HtmColor;
			alphaValue = 1.0f;
			Viewer3DForm visualizerForm = Viewer3D.Form;

			//Color color = basalSynapse.IsActive ? Color.Black : Color.White;
			try
			{
				if(basalSynapse.IsActive)
				{
					alphaValue = 1.0f;
					color = this.dictionaryDistalSynapseColors[HtmDistalSynapseColors.Active].HtmColor;
				}
				else    //Not active
				{
					alphaValue = 1.0f;
					color = this.dictionaryDistalSynapseColors[HtmDistalSynapseColors.Default].HtmColor;
				}

				//Selected
				if(basalSynapse.mouseSelected)
				{
					color = selectedColor;
					alphaValue = 1.0f;
				}

				//mouseOver
				if(basalSynapse.mouseOver)
				{
					color = mouseOverColor;
					alphaValue = 1.0f;
				}

				//set isVisible property if synapse is displayed
				if ( alphaValue > 0.1 )
				{
					basalSynapse.isVisible = true;
				}
				else
				{
					basalSynapse.isVisible = false;
				}

			}
			catch ( Exception )
			{

			}

		}

		/// <summary>
		/// Helper method to get color from cell activity
		/// </summary>
		/// <param name="cell"></param>
		/// <param name="color"></param>
		/// <param name="alphaValue"></param>
		private void GetColorFromCell (Cell cell, Column column, out Color color, out float alphaValue)
		{
			color = this.dictionaryCellColors[HtmCellColors.Inactive].HtmColor;
			alphaValue = 0.1f;  //All conditions can be false
			Viewer3DForm visualizerForm = Viewer3D.Form;

			try
			{
				//Currently predicting cells
				if ( visualizerForm.ShowPredictingCells && cell.IsPredicting )
				{
					if ( cell.IsPredicting )
					{
						//Sequence predicting cells (t+1)
						if ( visualizerForm.ShowSeqPredictingCells )
						{
							alphaValue = 1f;
							color = this.dictionaryCellColors[HtmCellColors.SequencePredicting].HtmColor;

						}
					}
					else
					{
						//Lost predicting cells for t+k
						alphaValue = 1f;
						color = this.dictionaryCellColors[HtmCellColors.Selected].HtmColor;

						//TODO: - original had logic for multiple dendrite segments
						//-replaced that with single BasalDendrite logic

						//New predicting cells for t+k
						if ( cell.BasalDendrite.IsActive )
						{
							color = this.dictionaryCellColors[HtmCellColors.Predicting].HtmColor;
						}

					}
				}

				//Inhibited in t+0
				if ( visualizerForm.ShowInhibitedColumns && column.IsInhibited )
				{
					alphaValue = 1f;
					color = this.dictionaryCellColors[HtmCellColors.Inhibited].HtmColor;
				}
				
				//Learning in t+0
				if ( visualizerForm.ShowLearningCells && cell.IsLearning )  
				{
					alphaValue = 1f;
					color = this.dictionaryCellColors[HtmCellColors.Learning].HtmColor;
				}
				else //Learning cells are all active
				{
					if ( visualizerForm.ShowActiveCells && cell.IsActive )
					{
						alphaValue = 1f;
						color = this.dictionaryCellColors[HtmCellColors.Active].HtmColor;
					}
				}

				//Sequence predicted cells
				if ( cell.GetSequencePredictingBasalSegment () != null )//TODO: see if HTM theory uses multiple segments 
				{
					//False predicted cells
					if ( visualizerForm.ShowFalsePredictedCells && !cell.IsActive )
					{
						alphaValue = 1f;
						color = this.dictionaryCellColors[HtmCellColors.FalsePrediction].HtmColor;
					}

					//Correctly predicting in t+0
					if ( visualizerForm.ShowCorrectPredictedCells && cell.IsActive )
					{
						alphaValue = 1f;
						color = this.dictionaryCellColors[HtmCellColors.RightPrediction].HtmColor;
					}
				}
			}
			catch ( Exception ex)
			{
				MessageBox.Show ( "Exception occured in GetColorFromCell() : " + Environment.NewLine + ex.Message );
			}
		}

		

		#endregion



		#region Update

		protected override void Update (GameTime gameTime)
		{
			this.UpdateCamera ();

			//Mouse is handled here.
			//Keys are handled by the Form since PictureBox control does not support keyboard events. 
			//Keys are detected in the Form (keyPreview = True) and passed to 
			//Handler_Viewer3DKeyForm_KeyPressed_Event() method 
			mouseState = Mouse.GetState ();
			prevMouseState = mouseState;

			mouseLClick = ( mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
				&& ( prevMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released );
			mouseRClick = ( mouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
				&& ( prevMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released );

			//mouseOver color change
			colorChangeMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
			if ( colorChangeMilliseconds >= 30 )
			{
				if ( mouseOverColor == color1 )
					mouseOverColor = color2;
				else
					mouseOverColor = color1;

				colorChangeMilliseconds = 0;
			}

			base.Update ( gameTime );
		}
		/// <summary>
		/// Rotation angle for camera in world space
		/// </summary>
		public void RotateWorldSpaceCamera(float diffX, float diffY)
		{
			this.yawCamera += diffX * rotateSpeedCamera;
			this.pitchCamera += diffY * rotateSpeedCamera;
		}

		/// <summary>
		/// Rotation angle for htm objects in world space
		/// </summary>
		public void RotateWorldSpaceHtmObjects(float diffX, float diffY)
		{
			this.yawHtm += diffX * rotateSpeedCamera;
			this.pitchHtm += diffY * rotateSpeedCamera;
		}

		public void AddToCameraPosition(Vector3 vectorToAdd)
		{
			Matrix cameraRotation = Matrix.CreateRotationX ( this.pitchCamera ) * Matrix.CreateRotationY ( this.yawCamera );
			Vector3 rotatedVector = Vector3.Transform ( vectorToAdd, cameraRotation );
			this.posCamera += this.moveSpeedCamera * rotatedVector;
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
				

		internal void ResetCamera ()
		{
			Vector3 sizeRegion = GetSize_Region ();
			Vector3 sizeInputPlane = GetSize_InputPlane ();

			float maxX = Math.Max ( sizeRegion.X, sizeInputPlane.X );
			float maxY = Math.Max ( sizeRegion.Y, sizeInputPlane.Y );
			float maxZ = Math.Max ( sizeRegion.Z, sizeInputPlane.Z );

			//position camera relative to region size
			//Y=4 slightly raised
			//pitch = -10 look slightly down
			//Z=size/3 + 3 - shift to right to give angled view (+3 provides shift for small regions)
			
			//this.posCamera = new Vector3 ( 5, 5, GetSize ().Y * 3 + 20 );
			//this.posCamera = new Vector3 ( 5, 15, GetSize_Region ().Y * 1 + 20 );
			this.posCamera = new Vector3 ( 5, maxY * 2 + 10, maxZ * 2 );

			//Reset rotation angle for camera
			this.yawCamera = (float)MathHelper.ToRadians ( 0 );
			this.pitchCamera = (float)MathHelper.ToRadians ( -50 );

			//Reset rotation angle for htm objects
			this.yawHtm = 0f;
			this.pitchHtm = 0f;

			//Reset zoom
			this.zoomCamera = 35f;

			this.UpdateCamera ();


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
		private Vector3 GetSize_Region()
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

		/// <summary>
		/// Calculate size of the region in world coordinates
		/// </summary>
		/// <returns>Vector3 size</returns>
		private Vector3 GetSize_InputPlane ()
		{
			Matrix worldTranslationZ = Matrix.CreateTranslation ( new Vector3 ( 0, 0, zHtmRegion ) );
			Matrix worldTranslation;
			Matrix worldRotate = Matrix.CreateRotationX ( this.pitchHtm ) * Matrix.CreateRotationY ( this.yawHtm );
			Vector3 size = new Vector3 ();

			foreach ( List<Column> colList in this.InputPlaneColumns )
			{
				foreach ( Column column in colList )
				{
					foreach ( Cell cell in column.Cells )
					{
						//calculate cell world coordinates
						var translationVector = new Vector3 ( column.X, cell.Index, column.Y );
						worldTranslation = Matrix.CreateTranslation ( translationVector ) * worldTranslationZ;

						//Z extent for initial camera position
						size.X = Math.Max ( translationVector.X, size.X );
						size.Y = Math.Max ( translationVector.Y, size.Y );
						size.Z = Math.Max ( translationVector.Z, size.Z );
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
				else if ( nearestBasalSynapse != null )
				{
					selObject = nearestBasalSynapse;
					selObject.mouseOver = true;
				}
				else if ( nearestProximalSynapse != null )
				{
					selObject = nearestProximalSynapse;
					selObject.mouseOver = true;
				}
				else if ( nearestApicalSynapse != null )
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

					////SHIFT key - deselect
					//if (keyState.IsKeyDown ( Microsoft.Xna.Framework.Input.Keys.LeftShift ) || keyState.IsKeyDown ( Microsoft.Xna.Framework.Input.Keys.RightShift ))
					//	selObject.mouseSelected = false;

					//update selected object list
					//UpdateSelectedObjectList ( selObject, selObject.mouseSelected );
					UpdateSelectedObjectList ( this.Region, selObject.mouseSelected );
				}
			}
		}
		
		public Ray getPickingRay (System.Drawing.Point mousePosition)
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
			//if ( column.IsDataGridSelected || ( Viewer3D.Form.ShowSpatialLearning && column.IsActive ) )
			//{
				Vector3 rayP1 = ray.Position;
				Vector3 rayP2 = rayP1 + ray.Direction;

				foreach ( SynapseProximal synapse in column.ProximalDendrite.Synapses )
				{
					synapse.mouseOver = false;

					if ( synapse.isVisible )
					{
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

						if ( intersect && intersectDistance < minDistance )
						{
							minDistance = intersectDistance;
							returnedProximalSynapse = synapse;
						}
						//}
					}
				}
			//}

			return minDistance;
		}

		private float PickBasalSynapseConnections (Ray ray, Column column, Cell cell, ref SynapseBasal returnedBasalSynapse)
		{
			float intersectDistance = float.MaxValue;
			float minDistance = float.MaxValue;

			returnedBasalSynapse = null;

			//Draw Connections if existing
			//if ( cell.IsDataGridSelected || ( Viewer3D.Form.ShowSpatialLearning && cell.IsPredicting ) )
			//{
				Vector3 rayP1 = ray.Position;
				Vector3 rayP2 = rayP1 + ray.Direction;

				foreach ( SynapseBasal synapse in cell.BasalDendrite.Synapses )
				{
					synapse.mouseOver = false;

					if ( synapse.isVisible )
					{
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
			//}
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

			//if ( synapse.isVisible )
			//{
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
				//}
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
				nearestDistance = distance;

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



	#region Helper Color Classes

	public struct HtmColorInformation
	{
		#region Fields

		public Color HtmColor;
		public string HtmInformation;

		#endregion

		#region Constructor

		public HtmColorInformation (Color color, string info)
		{
			this.HtmColor = color;
			this.HtmInformation = info;
		}

		#endregion
	}

	public enum HtmCellColors
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

	public enum HtmProximalSynapseColors
	{
		Default,
		Active,
		ActiveConnected
	}

	public enum HtmDistalSynapseColors
	{
		Default,
		Active
	}

	public class HtmOverViewInformation
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
}
