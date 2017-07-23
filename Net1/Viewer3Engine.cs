using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Net1
{

	/// <summary>
	/// Main class for 3D viewer implementation
	/// </summary>
	public class Viewer3DEngine : Game
	{
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

		// Colors
		private Dictionary<HtmCellColors, HtmColorInformation> dictionaryCellColors;
		private Dictionary<HtmProximalSynapseColors, HtmColorInformation> dictionaryProximalSynapseColors;
		private Dictionary<HtmDistalSynapseColors, HtmColorInformation> dictionaryDistalSynapseColors;

		// Primitives
		private CubePrimitive cube;
		private CoordinateSysPrimitive coordinateSystem;
		private SquarePrimitve bit;
		private LinePrimitive connectionLine;

		#endregion

		#region Grid

		private Texture2D gridTexture;
		private Texture2D whiteTexture;

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
				//this.rightLegend = new HtmOverviewInformation();

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
			this.posCamera = new Vector3(-25, 4, GetSize().X / 3 + 3);

			//Reset rotation angle for camera
			this.yawCamera = (float)MathHelper.ToRadians(-90);
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






	}
}
