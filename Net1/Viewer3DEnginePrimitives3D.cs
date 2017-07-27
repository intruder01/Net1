using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Net1
{
	public class CoordinateSysPrimitive
	{
		#region Fields

		private VertexPositionColor[] _vertices;
		private GraphicsDevice _device;
		private BasicEffect _basicEffect;

		#endregion

		#region Constructor

		public CoordinateSysPrimitive(GraphicsDevice device)
		{
			this._device = device;
			this._basicEffect = new BasicEffect(device);
			this.InitVertices();
		}

		#endregion

		#region Methods

		private void InitVertices()
		{
			this._vertices = new VertexPositionColor[30];

			//X
			this._vertices[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Red);
			this._vertices[1] = new VertexPositionColor(Vector3.Right * 5, Color.Red);
			this._vertices[2] = new VertexPositionColor(new Vector3(5, 0, 0), Color.Red);
			this._vertices[3] = new VertexPositionColor(new Vector3(4.5f, 0.5f, 0), Color.Red);
			//this._vertices[4] = new VertexPositionColor(new Vector3(5, 0, 0), Color.Red);
			//this._vertices[5] = new VertexPositionColor(new Vector3(4.5f, -0.5f, 0), Color.Red);

			//Y
			this._vertices[6] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Green);
			this._vertices[7] = new VertexPositionColor(Vector3.Up * 5, Color.Green);
			//this._vertices[8] = new VertexPositionColor(new Vector3(0, 5, 0), Color.Green);
			//this._vertices[9] = new VertexPositionColor(new Vector3(0.5f, 4.5f, 0), Color.Green);
			//this._vertices[10] = new VertexPositionColor(new Vector3(0, 5, 0), Color.Green);
			//this._vertices[11] = new VertexPositionColor(new Vector3(-0.5f, 4.5f, 0), Color.Green);

			//Z
			this._vertices[12] = new VertexPositionColor ( new Vector3 ( 0, 0, 0 ), Color.Blue );
			this._vertices[13] = new VertexPositionColor ( Vector3.Forward * 5, Color.Blue );
			//this._vertices[14] = new VertexPositionColor ( new Vector3 ( 0, 0, -5 ), Color.Blue );
			//this._vertices[15] = new VertexPositionColor ( new Vector3 ( 0, 0.5f, -4.5f ), Color.Blue );
			//this._vertices[16] = new VertexPositionColor ( new Vector3 ( 0, 0, -5 ), Color.Blue );
			//this._vertices[17] = new VertexPositionColor ( new Vector3 ( 0, -0.5f, -4.5f ), Color.Blue );
		}

		public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
		{
			this._basicEffect.World = Matrix.Identity;
			this._basicEffect.View = viewMatrix;
			this._basicEffect.Projection = projectionMatrix;
			this._basicEffect.VertexColorEnabled = true;
			foreach (var pass in this._basicEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				this._device.DrawUserPrimitives(PrimitiveType.LineList, this._vertices, 0, 9);
			}
		}

		public void DrawUsingPresetEffect()
		{
			this._device.DrawUserPrimitives(PrimitiveType.LineList, this._vertices, 0, 9);
		}

		#endregion
	}

	/// <summary>
	/// Custom vertex type for vertices that have just a
	/// position and a normal, without any texture coordinates.
	/// </summary>
	public struct VertexPositionNormal : IVertexType
	{
		#region Fields

		public Vector3 Position;
		public Vector3 Normal;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public VertexPositionNormal(Vector3 position, Vector3 normal)
		{
			this.Position = position;
			this.Normal = normal;
		}

		#endregion

		#region Properties

		/// <summary>
		/// A VertexDeclaration object, which contains information about the vertex
		/// elements contained within this struct.
		/// </summary>
		VertexDeclaration IVertexType.VertexDeclaration
		{
			get
			{
				return VertexDeclaration;
			}
		}

		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0));

		#endregion
	}

	/// <summary>
	/// Base class for simple geometric primitive models. This provides a vertex
	/// buffer, an index buffer, plus methods for drawing the model. Classes for
	/// specific types of primitive (CubePrimitive, SpherePrimitive, etc.) are
	/// derived from this common base, and use the AddVertex and AddIndex methods
	/// to specify their geometry.
	/// </summary>
	public abstract class GeometricPrimitive : IDisposable
	{
		#region Fields

		// During the process of constructing a primitive model, vertex
		// and index data is stored on the CPU in these managed lists.
		private List<VertexPositionNormal> _vertices = new List<VertexPositionNormal>();
		private List<ushort> _indices = new List<ushort>();

		// Once all the geometry has been specified, the InitializePrimitive
		// method copies the vertex and index data into these buffers, which
		// store it on the GPU ready for efficient rendering.
		private VertexBuffer _vertexBuffer;
		private IndexBuffer _indexBuffer;
		private BasicEffect _basicEffect;


		#endregion

		#region Properties

		//I added these for storing mouse activity over the primitives
		public bool mouseOver { get; set; }
		public bool mouseSelected { get; set; }

		/// <summary>
		/// Queries the index of the current vertex. This starts at
		/// zero, and increments every time AddVertex is called.
		/// </summary>
		protected int CurrentVertex
		{
			get
			{
				return this._vertices.Count;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a new vertex to the primitive model. This should only be called
		/// during the initialization process, before InitializePrimitive.
		/// </summary>
		protected void AddVertex(Vector3 position, Vector3 normal)
		{
			this._vertices.Add(new VertexPositionNormal(position, normal));
		}

		/// <summary>
		/// Adds a new index to the primitive model. This should only be called
		/// during the initialization process, before InitializePrimitive.
		/// </summary>
		protected void AddIndex(int index)
		{
			if (index > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			this._indices.Add((ushort)index);
		}

		/// <summary>
		/// Once all the geometry has been specified by calling AddVertex and AddIndex,
		/// this method copies the vertex and index data into GPU format buffers, ready
		/// for efficient rendering.
		/// </summary>
		protected void InitializePrimitive(GraphicsDevice graphicsDevice)
		{
			// Create a vertex declaration, describing the format of our vertex data.

			// Create a vertex buffer, and copy our vertex data into it.
			this._vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormal), this._vertices.Count, BufferUsage.None);

			this._vertexBuffer.SetData(this._vertices.ToArray());

			// Create an index buffer, and copy our index data into it.
			this._indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), this._indices.Count, BufferUsage.None);

			this._indexBuffer.SetData(this._indices.ToArray());

			// Create a BasicEffect, which will be used to render the primitive.
			this._basicEffect = new BasicEffect(graphicsDevice);

			this._basicEffect.EnableDefaultLighting();
		}

		/// <summary>
		/// Draws the primitive model, using the specified effect. Unlike the other
		/// Draw overload where you just specify the world/view/projection matrices
		/// and color, this method does not set any renderstates, so you must make
		/// sure all states are set to sensible values before you call it.
		/// </summary>
		public void Draw(Effect effect)
		{
			GraphicsDevice graphicsDevice = effect.GraphicsDevice;

			var rs = new RasterizerState()
			{
				CullMode = CullMode.None
			};
			graphicsDevice.RasterizerState = rs;

			// Set our vertex declaration, vertex buffer, and index buffer.
			graphicsDevice.SetVertexBuffer(this._vertexBuffer);

			graphicsDevice.Indices = this._indexBuffer;

			foreach (var effectPass in effect.CurrentTechnique.Passes)
			{
				effectPass.Apply();

				int primitiveCount = this._indices.Count / 3;

				//graphicsDevice.DrawIndexedPrimitives ( PrimitiveType.TriangleList, 0, 0, this._vertices.Count, 0, primitiveCount );
				graphicsDevice.DrawIndexedPrimitives ( PrimitiveType.TriangleList, 0, 0, primitiveCount );
			}
		}

		/// <summary>
		/// Draws the primitive model, using a BasicEffect shader with default
		/// lighting. Unlike the other Draw overload where you specify a custom
		/// effect, this method sets important renderstates to sensible values
		/// for 3D model rendering, so you do not need to set these states before
		/// you call it.
		/// </summary>
		public void Draw(Matrix world, Matrix view, Matrix projection, Color color, float alphaValue)
		{
			// Set BasicEffect parameters.
			this._basicEffect.World = world;
			this._basicEffect.View = view;
			this._basicEffect.Projection = projection;
			this._basicEffect.DiffuseColor = color.ToVector3();
			// basicEffect.Alpha = color.A / 255.0f;
			this._basicEffect.Alpha = alphaValue;

			GraphicsDevice device = this._basicEffect.GraphicsDevice;
			device.DepthStencilState = DepthStencilState.Default;
			device.BlendState = BlendState.AlphaBlend;

			// Draw the model, using BasicEffect.
			this.Draw(this._basicEffect);
		}

		public BoundingBox GetBoundingBox(Matrix world)
		{
			//Create variables to hold min and max xyz values for the mesh. Initialise them to extremes
			Vector3 vertexMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Vector3 vertexMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			foreach (var v in this._vertices)
			{
				Vector3 vertexPos = v.Position;
				Vector3 transPos = Vector3.Transform(vertexPos, world);
				vertexMax = Vector3.Max(vertexMax, transPos);
				vertexMin = Vector3.Min(vertexMin, transPos);
			}

			BoundingBox box = new BoundingBox(vertexMin, vertexMax);
			//box.Max = vertexMax;
			//box.Min = vertexMin;
			return box;
		}

		#endregion

		#region Constructor

		public GeometricPrimitive()
		{
			mouseOver = false;
			mouseSelected = false;
		}
		/// <summary>
		/// Finalizer.
		/// </summary>
		~GeometricPrimitive()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Frees resources used by this object.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Frees resources used by this object.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this._vertexBuffer != null)
				{
					this._vertexBuffer.Dispose();
				}

				if (this._indexBuffer != null)
				{
					this._indexBuffer.Dispose();
				}

				if (this._basicEffect != null)
				{
					this._basicEffect.Dispose();
				}
			}
		}

		#endregion
	}

	public class LinePrimitive
	{
		#region Fields

		private VertexPositionColor[] _vertices;
		private GraphicsDevice _device;
		private BasicEffect _basicEffect;

		#endregion

		#region Constructor

		public LinePrimitive(GraphicsDevice graphicsDevice)
		{
			this._device = graphicsDevice;
			this._basicEffect = new BasicEffect(this._device);
			//mouseOver = false;
			//mouseSelected = false;
		}

		#endregion

		#region Methods

		//public void SetUpVertices(Vector3 v1, Vector3 v2, Color color)
		//{
		//	var v3 = new Vector3(v2.X - 0.1f, v2.Y - 0.1f, v2.Z - 0.1f);

		//	this._vertices = new VertexPositionColor[3];

		//	this._vertices[0] = new VertexPositionColor(v1, color);
		//	this._vertices[1] = new VertexPositionColor(v2, color);
		//	this._vertices[2] = new VertexPositionColor(v3, color);
		//}

		public void SetUpVertices(Vector3 v1, Vector3 v2, Color color)
		{
			var v3 = new Vector3(v2.X - 0.1f, v2.Y - 0.1f, v2.Z - 0.1f);

			this._vertices = new VertexPositionColor[3];

			this._vertices[0] = new VertexPositionColor(v1, color);
			this._vertices[1] = new VertexPositionColor(v2, color);
			this._vertices[2] = new VertexPositionColor(v3, color);


		}

		public void Draw(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
		{
			this._basicEffect.World = worldMatrix;
			this._basicEffect.View = viewMatrix;
			this._basicEffect.Projection = projectionMatrix;
			this._basicEffect.VertexColorEnabled = true;
			foreach (var pass in this._basicEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				this._device.DrawUserPrimitives(PrimitiveType.LineList, this._vertices, 0, 1);
			}
		}

		#endregion
	}

	public class SquarePrimitve : GeometricPrimitive
	{
		#region Constructor

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		public SquarePrimitve(GraphicsDevice device)
		{
			// Square does have 4 vertices

			// Coordinates in world-space origin
			var v1 = new Vector3(1, 0, 1);
			var v2 = new Vector3(-1, 0, 1);
			var v3 = new Vector3(-1, 0, -1);
			var v4 = new Vector3(1, 0, -1);

			// Add Indices for index buffe
			this.AddIndex(0);
			this.AddIndex(1);
			this.AddIndex(2);
			this.AddIndex(2);
			this.AddIndex(3);
			this.AddIndex(0);

			// Normal for each vertice: Up-Vector
			var normal = new Vector3(0, 1, 0);
			this.AddVertex(v1, normal);
			this.AddVertex(v2, normal);
			this.AddVertex(v3, normal);
			this.AddVertex(v4, normal);

			// Initialize Buffers
			this.InitializePrimitive(device);
		}

		#endregion
	}

	/// <summary>
	/// Geometric primitive class for drawing cubes.
	/// </summary>
	public class CubePrimitive : GeometricPrimitive
	{

		#region Constructor

		/// <summary>
		/// Constructs a new cube primitive, using default settings.
		/// </summary>
		public CubePrimitive(GraphicsDevice graphicsDevice)
			: this(graphicsDevice, 1)
		{
		}

		/// <summary>
		/// Constructs a new cube primitive, with the specified size.
		/// </summary>
		public CubePrimitive(GraphicsDevice graphicsDevice, float size)
		{
			// A cube has six faces, each one pointing in a different direction.
			Vector3[] normals = { new Vector3(0, 0, 1), new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, -1, 0) };

			// Create each face in turn.
			foreach (var normal in normals)
			{
				// Get two vectors perpendicular to the face normal and to each other.
				var side1 = new Vector3(normal.Y, normal.Z, normal.X);
				Vector3 side2 = Vector3.Cross(normal, side1);

				// Six indices (two triangles) per face.
				this.AddIndex(this.CurrentVertex + 0);
				this.AddIndex(this.CurrentVertex + 1);
				this.AddIndex(this.CurrentVertex + 2);

				this.AddIndex(this.CurrentVertex + 0);
				this.AddIndex(this.CurrentVertex + 2);
				this.AddIndex(this.CurrentVertex + 3);

				// Four vertices per face.
				Vector3 v1 = (normal - side1 - side2) * size / 2;
				Vector3 v2 = (normal - side1 + side2) * size / 2;
				Vector3 v3 = (normal + side1 + side2) * size / 2;
				Vector3 v4 = (normal + side1 - side2) * size / 2;

				this.AddVertex(v1, normal);
				this.AddVertex(v2, normal);
				this.AddVertex(v3, normal);
				this.AddVertex(v4, normal);
			}

			this.InitializePrimitive(graphicsDevice);


		}

		#endregion
	}
}
