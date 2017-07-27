using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


//Unity3D math function library from: http://wiki.unity3d.com/index.php/3d_Math_functions
//Note: not all functions have been ported from original library.

namespace Net1
{
	public class Math3D
	{

	#region Functions used
		//Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
		//Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
		//same plane, use ClosestPointsOnTwoLines() instead.
		//Note: modified from original for 4 points.
		public static bool LineLineIntersection (  Vector3 line1P1, Vector3 line1P2, Vector3 line2P1, Vector3 line2P2, out Vector3 intersection )
		{
			
			Vector3 lineVec1 = line1P2 - line1P1;
			Vector3 lineVec2 = line2P2 - line2P1;
			intersection = new Vector3 ( 0, 0, 0 );

			Vector3 lineVec3 = line2P1 - line1P1;
			Vector3 crossVec1and2 = Vector3.Cross ( lineVec1, lineVec2 );
			Vector3 crossVec3and2 = Vector3.Cross ( lineVec3, lineVec2 );

			float planarFactor = Vector3.Dot ( lineVec3, crossVec1and2 );

			//Lines are not coplanar. Take into account rounding errors.
			if ((planarFactor >= 0.0001f) || (planarFactor <= -0.0001f))
			{

				return false;
			}

			//Note: sqrMagnitude does x*x+y*y+z*z on the input vector.
			float s = Vector3.Dot ( crossVec3and2, crossVec1and2 ) / ((crossVec1and2.X * crossVec1and2.X) + (crossVec1and2.Y * crossVec1and2.Y) + (crossVec1and2.Z * crossVec1and2.Z));

			if ((s >= 0.0f) && (s <= 1.0f))
			{
				intersection = line1P1 + (lineVec1 * s);
				return true;
			}
			else
			{
				return false;
			}
		}

		//Two non-parallel lines which may or may not touch each other have a point on each line which are closest
		//to each other. This function finds those two points. If the lines are not parallel, the function 
		//outputs true, otherwise false.
		public static bool ClosestPointsOnTwoLines ( out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 line1P1, Vector3 line1P2, Vector3 line2P1, Vector3 line2P2 )
		{
			Vector3 lineVec1 = line1P2 - line1P1;
			Vector3 lineVec2 = line2P2 - line2P1;

			closestPointLine1 = new Vector3 ( 0, 0, 0 );
			closestPointLine2 = new Vector3 ( 0, 0, 0 );

			float a = Vector3.Dot ( lineVec1, lineVec1 );
			float b = Vector3.Dot ( lineVec1, lineVec2 );
			float e = Vector3.Dot ( lineVec2, lineVec2 );

			float d = a * e - b * b;

			//lines are not parallel
			if (d != 0.0f)
			{
				Vector3 r = line1P1 - line2P1;
				float c = Vector3.Dot ( lineVec1, r );
				float f = Vector3.Dot ( lineVec2, r );

				float s = (b * f - c * e) / d;
				float t = (a * f - c * b) / d;

				closestPointLine1 = line1P1 + lineVec1 * s;
				closestPointLine2 = line2P1 + lineVec2 * t;

				return true;
			}

			else
			{
				return false;
			}
		}

		//look here
		//http://stackoverflow.com/questions/627563/calculating-the-shortest-distance-between-two-lines-line-segments-in-3d

		public static bool ClosestPointsLineSegmentToLine ( out Vector3 closestPointLineSeg, out Vector3 closestPointRay, Vector3 lineSegP1, Vector3 lineSegP2, Vector3 rayP1, Vector3 rayP2, float tolerance, out float intersectDistance)
		{
			intersectDistance = float.MaxValue;

			bool notParallel = ClosestPointsOnTwoLines ( out closestPointRay, out closestPointLineSeg, rayP1, rayP2, lineSegP1, lineSegP2 );

			if (!notParallel)
				return false;

			intersectDistance = Vector3.Distance ( closestPointRay, closestPointLineSeg );

			//find segment endpoints min/max values
			float lineSegXMin = lineSegP1.X >= lineSegP2.X ? lineSegP2.X : lineSegP1.X;
			float lineSegXMax = lineSegP1.X >= lineSegP2.X ? lineSegP1.X : lineSegP2.X;
			float lineSegYMin = lineSegP1.Y >= lineSegP2.Y ? lineSegP2.Y : lineSegP1.Y;
			float lineSegYMax = lineSegP1.Y >= lineSegP2.Y ? lineSegP1.Y : lineSegP2.Y;
			float lineSegZMin = lineSegP1.Z >= lineSegP2.Z ? lineSegP2.Z : lineSegP1.Z;
			float lineSegZMax = lineSegP1.Z >= lineSegP2.Z ? lineSegP1.Z : lineSegP2.Z;

			//move endpoints "in" by tolerance amount. The idea is to ignore actual endpoints. Closest point will be at endpoints if 
			//mouse is outside of line segment rectangle.
			//lineSegXMax -= tolerance;
			//lineSegXMin += tolerance;
			//lineSegYMax -= tolerance;
			//lineSegYMin += tolerance;
			//lineSegZMax -= tolerance;
			//lineSegZMin += tolerance;

			if (closestPointLineSeg.X >= lineSegXMin && closestPointLineSeg.X <= lineSegXMax &&
				closestPointLineSeg.Y >= lineSegYMin && closestPointLineSeg.Y <= lineSegYMax &&
				closestPointLineSeg.Z >= lineSegZMin && closestPointLineSeg.Z <= lineSegZMax)
			{
				if (intersectDistance <= tolerance)
					return true;
			}

			return false;
		}


		/////////////////////////////////////////////////////////////////////////////////
		//class point
		//{
		//	public double x;
		//	public double y;
		//	public double z;
		//}

		//class line
		//{
		//	public double x1;
		//	public double y1;
		//	public double z1;
		//	public double x2;
		//	public double y2;
		//	public double z2;
		//}

		//double getShortestDistance ( line line1, line line2 )
		//{
		//	double EPS = 0.00000001;

		//	point delta21 = new point ();
		//	delta21.x = line1.x2 - line1.x1;
		//	delta21.y = line1.y2 - line1.y1;
		//	delta21.z = line1.z2 - line1.z1;

		//	point delta41 = new point ();
		//	delta41.x = line2.x2 - line2.x1;
		//	delta41.y = line2.y2 - line2.y1;
		//	delta41.z = line2.z2 - line2.z1;

		//	point delta13 = new point ();
		//	delta13.x = line1.x1 - line2.x1;
		//	delta13.y = line1.y1 - line2.y1;
		//	delta13.z = line1.z1 - line2.z1;

		//	double a = dot ( delta21, delta21 );
		//	double b = dot ( delta21, delta41 );
		//	double c = dot ( delta41, delta41 );
		//	double d = dot ( delta21, delta13 );
		//	double e = dot ( delta41, delta13 );
		//	double D = a * c - b * b;

		//	double sc, sN, sD = D;
		//	double tc, tN, tD = D;

		//	if (D < EPS)
		//	{
		//		sN = 0.0;
		//		sD = 1.0;
		//		tN = e;
		//		tD = c;
		//	}
		//	else
		//	{
		//		sN = (b * e - c * d);
		//		tN = (a * e - b * d);
		//		if (sN < 0.0)
		//		{
		//			sN = 0.0;
		//			tN = e;
		//			tD = c;
		//		}
		//		else if (sN > sD)
		//		{
		//			sN = sD;
		//			tN = e + b;
		//			tD = c;
		//		}
		//	}

		//	if (tN < 0.0)
		//	{
		//		tN = 0.0;

		//		if (-d < 0.0)
		//			sN = 0.0;
		//		else if (-d > a)
		//			sN = sD;
		//		else
		//		{
		//			sN = -d;
		//			sD = a;
		//		}
		//	}
		//	else if (tN > tD)
		//	{
		//		tN = tD;
		//		if ((-d + b) < 0.0)
		//			sN = 0;
		//		else if ((-d + b) > a)
		//			sN = sD;
		//		else
		//		{
		//			sN = (-d + b);
		//			sD = a;
		//		}
		//	}

		//	if (Math.Abs ( sN ) < EPS) sc = 0.0;
		//	else sc = sN / sD;
		//	if (Math.Abs ( tN ) < EPS) tc = 0.0;
		//	else tc = tN / tD;

		//	point dP = new point ();
		//	dP.x = delta13.x + (sc * delta21.x) - (tc * delta41.x);
		//	dP.y = delta13.y + (sc * delta21.y) - (tc * delta41.y);
		//	dP.z = delta13.z + (sc * delta21.z) - (tc * delta41.z);

		//	return Math.Sqrt ( dot ( dP, dP ) );
		//}

		//private double dot ( point c1, point c2 )
		//{
		//	return (c1.x * c2.x + c1.y * c2.y + c1.z * c2.z);
		//}

		//private double norm ( point c1 )
		//{
		//	return Math.Sqrt ( dot ( c1, c1 ) );
		//}



		///////////////////////////////////////////////////////////////////////////////



	#endregion

	#region Other functions
		//increase or decrease the length of vector by size
		public static Vector3 AddVectorLength ( Vector3 vector, float size )
		{

			//get the vector length
			float magnitude = vector.Length();

			//calculate new vector length
			float newMagnitude = magnitude + size;

			//calculate the ratio of the new length to the old length
			float scale = newMagnitude / magnitude;

			//scale the vector
			return vector * scale;
		}

		//create a vector of direction "vector" with length "size"
		public static Vector3 SetVectorLength ( Vector3 vector, float size )
		{

			//normalize the vector
			Vector3 vectorNormalized = Vector3.Normalize ( vector );

			//scale the vector
			return vectorNormalized *= size;
		}


		//Find the line of intersection between two planes.	The planes are defined by a normal and a point on that plane.
		//The outputs are a point on the line and a vector which indicates it's direction. If the planes are not parallel, 
		//the function outputs true, otherwise false.
		public static bool PlanePlaneIntersection ( out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position )
		{

			linePoint = new Vector3 ( 0, 0, 0 );
			lineVec = new Vector3 ( 0, 0, 0 );

			//We can get the direction of the line of intersection of the two planes by calculating the 
			//cross product of the normals of the two planes. Note that this is just a direction and the line
			//is not fixed in space yet. We need a point for that to go with the line vector.
			lineVec = Vector3.Cross ( plane1Normal, plane2Normal );

			//Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
			//the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
			//errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
			//the cross product of the normal of plane2 and the lineDirection.		
			Vector3 ldir = Vector3.Cross ( plane2Normal, lineVec );

			float denominator = Vector3.Dot ( plane1Normal, ldir );

			//Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
			if (Math.Abs ( denominator ) > 0.006f)
			{

				Vector3 plane1ToPlane2 = plane1Position - plane2Position;
				float t = Vector3.Dot ( plane1Normal, plane1ToPlane2 ) / denominator;
				linePoint = plane2Position + t * ldir;

				return true;
			}

			//output not valid
			else
			{
				return false;
			}
		}

		//Get the intersection between a line and a plane. 
		//If the line and plane are not parallel, the function outputs true, otherwise false.
		public static bool LinePlaneIntersection ( out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint )
		{

			float length;
			float dotNumerator;
			float dotDenominator;
			Vector3 vector;
			intersection = new Vector3 ( 0, 0, 0 );

			//calculate the distance between the linePoint and the line-plane intersection point
			dotNumerator = Vector3.Dot ( (planePoint - linePoint), planeNormal );
			dotDenominator = Vector3.Dot ( lineVec, planeNormal );

			//line and plane are not parallel
			if (dotDenominator != 0.0f)
			{
				length = dotNumerator / dotDenominator;

				//create a vector from the linePoint to the intersection point
				vector = SetVectorLength ( lineVec, length );

				//get the coordinates of the line-plane intersection point
				intersection = linePoint + vector;

				return true;
			}

			//output not valid
			else
			{
				return false;
			}
		}

		//This function returns a point which is a projection from a point to a line.
		//The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
		public static Vector3 ProjectPointOnLine ( Vector3 linePoint, Vector3 lineVec, Vector3 point )
		{

			//get vector from point on line to point in space
			Vector3 linePointToPoint = point - linePoint;

			float t = Vector3.Dot ( linePointToPoint, lineVec );

			return linePoint + lineVec * t;
		}

		//This function returns a point which is a projection from a point to a line segment.
		//If the projected point lies outside of the line segment, the projected point will 
		//be clamped to the appropriate line edge.
		//If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
		public static Vector3 ProjectPointOnLineSegment ( Vector3 linePoint1, Vector3 linePoint2, Vector3 point )
		{

			Vector3 vector = linePoint2 - linePoint1;
			vector.Normalize ();

			Vector3 projectedPoint = ProjectPointOnLine ( linePoint1, vector, point );

			int side = PointOnWhichSideOfLineSegment ( linePoint1, linePoint2, projectedPoint );

			//The projected point is on the line segment
			if (side == 0)
			{
				return projectedPoint;
			}

			if (side == 1)
			{
				return linePoint1;
			}

			if (side == 2)
			{
				return linePoint2;
			}

			//output is invalid
			return new Vector3 ( 0, 0, 0 );
		}

		//This function returns a point which is a projection from a point to a plane.
		public static Vector3 ProjectPointOnPlane ( Vector3 planeNormal, Vector3 planePoint, Vector3 point )
		{
			float distance;
			Vector3 translationVector;

			//First calculate the distance from the point to the plane:
			distance = SignedDistancePlanePoint ( planeNormal, planePoint, point );

			//Reverse the sign of the distance
			distance *= -1;

			//Get a translation vector
			translationVector = SetVectorLength ( planeNormal, distance );

			//Translate the point to form a projection
			return point + translationVector;
		}

		//Projects a vector onto a plane. The output is not normalized.
		public static Vector3 ProjectVectorOnPlane ( Vector3 planeNormal, Vector3 vector )
		{
			return vector - (Vector3.Dot ( vector, planeNormal ) * planeNormal);
		}

		//Get the shortest distance between a point and a plane. The output is signed so it holds information
		//as to which side of the plane normal the point is.
		public static float SignedDistancePlanePoint ( Vector3 planeNormal, Vector3 planePoint, Vector3 point )
		{
			return Vector3.Dot ( planeNormal, (point - planePoint) );
		}

		//This function calculates a signed (+ or - sign instead of being ambiguous) dot product. It is basically used
		//to figure out whether a vector is positioned to the left or right of another vector. The way this is done is
		//by calculating a vector perpendicular to one of the vectors and using that as a reference. This is because
		//the result of a dot product only has signed information when an angle is transitioning between more or less
		//then 90 degrees.
		public static float SignedDotProduct ( Vector3 vectorA, Vector3 vectorB, Vector3 normal )
		{

			Vector3 perpVector;
			float dot;

			//Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
			perpVector = Vector3.Cross ( normal, vectorA );

			//Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
			dot = Vector3.Dot ( perpVector, vectorB );

			return dot;
		}

		

		//Calculate the angle between a vector and a plane. The plane is made by a normal vector.
		//Output is in radians.
		public static float AngleVectorPlane ( Vector3 vector, Vector3 normal )
		{
			float dot;
			float angle;

			//calculate the the dot product between the two input vectors. This gives the cosine between the two vectors
			dot = Vector3.Dot ( vector, normal );

			//this is in radians
			angle = (float)Math.Acos ( dot );

			return 1.570796326794897f - angle; //90 degrees - angle
		}

		//Calculate the dot product as an angle
		public static float DotProductAngle ( Vector3 vec1, Vector3 vec2 )
		{

			double dot;
			double angle;

			//get the dot product
			dot = Vector3.Dot ( vec1, vec2 );

			//Clamp to prevent NaN error. Shouldn't need this in the first place, but there could be a rounding error issue.
			if (dot < -1.0f)
			{
				dot = -1.0f;
			}
			if (dot > 1.0f)
			{
				dot = 1.0f;
			}

			//Calculate the angle. The output is in radians
			//This step can be skipped for optimization...
			angle = Math.Acos ( dot );

			return (float)angle;
		}

		//This function finds out on which side of a line segment the point is located.
		//The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
		//the line segment, project it on the line using ProjectPointOnLine() first.
		//Returns 0 if point is on the line segment.
		//Returns 1 if point is outside of the line segment and located on the side of linePoint1.
		//Returns 2 if point is outside of the line segment and located on the side of linePoint2.
		public static int PointOnWhichSideOfLineSegment ( Vector3 linePoint1, Vector3 linePoint2, Vector3 point )
		{

			Vector3 lineVec = linePoint2 - linePoint1;
			Vector3 pointVec = point - linePoint1;

			float dot = Vector3.Dot ( pointVec, lineVec );

			//point is on side of linePoint2, compared to linePoint1
			if (dot > 0)
			{

				//point is on the line segment
				if (pointVec.Length() <= lineVec.Length())
				{

					return 0;
				}

				//point is not on the line segment and it is on the side of linePoint2
				else
				{

					return 2;
				}
			}

			//Point is not on side of linePoint2, compared to linePoint1.
			//Point is not on the line segment and it is on the side of linePoint1.
			else
			{

				return 1;
			}
		}
 


	#endregion



	}
}
