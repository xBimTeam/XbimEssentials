using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Common.Geometry
{
	public class NewellNormalEvaluator
	{
		/*
		 * 
		 * Pseudocode from https://www.khronos.org/opengl/wiki/Calculating_a_Surface_Normal
		 
		   Set Vertex Normal to (0, 0, 0)

		   Begin Cycle for Index in [0, Polygon.vertexNumber)

			  Set Vertex Current to Polygon.verts[Index]
			  Set Vertex Next    to Polygon.verts[(Index plus 1) mod Polygon.vertexNumber]

			  Set Normal.x to Sum of Normal.x and (multiply (Current.y minus Next.y) by (Current.z plus Next.z))
			  Set Normal.y to Sum of Normal.y and (multiply (Current.z minus Next.z) by (Current.x plus Next.x))
			  Set Normal.z to Sum of Normal.z and (multiply (Current.x minus Next.x) by (Current.y plus Next.y))

		   End Cycle

		   Returning Normalize(Normal)
		 
		 */

		double firstX = 0;
		double firstY = 0;
		double firstZ = 0;

		double prevX = 0;
		double prevY = 0;
		double prevZ = 0;

		double nX = 0;
		double nY = 0;
		double nZ = 0;

		bool first = true;

		public double Precision { get; set; }

		public bool AddVertex(double vertexX, double  vertexY, double vertexZ)
		{
			if (first)
			{
				firstX = vertexX;
				firstY = vertexY;
				firstZ = vertexZ;
				prevX = vertexX;
				prevY = vertexY;
				prevZ = vertexZ;
				first = false;
				return false;
			}
			else
			{
				// in pseudocode it uses current and next, since we're only computing on arrival of "next"
				// we're using last + vertex
				// 
				//  current => last
				//  next    => vertex
				//
				nX += (prevY - vertexY) * (prevZ + vertexZ);
				nY += (prevZ - vertexZ) * (prevX + vertexX);
				nZ += (prevX - vertexX) * (prevY + vertexY);

				prevX = vertexX;
				prevY = vertexY;
				prevZ = vertexZ;
				Debug.WriteLine($"Normal x:{nX} y:{nY} z:{nZ}");
				return Math.Abs(nX) > Precision ||
					Math.Abs(nY) > Precision ||
					Math.Abs(nZ) > Precision;
			}
		}

		public XbimVector3D GetNormal(bool ComputeLastAndReset = true)
		{
			if (!ComputeLastAndReset)
			{
				return new XbimVector3D(nX, nY, nZ).Normalized();
			}

			AddVertex(firstX, firstY, firstZ);

			// cache the return value
			var ret = new XbimVector3D(nX, nY, nZ);

			// reset and then return
			firstX = 0;
			firstY = 0;
			firstZ = 0;

			prevX = 0;
			prevY = 0;
			prevZ = 0;

			nX = 0;
			nY = 0;
			nZ = 0;

			first = true;
			Debug.WriteLine("NewellNormalEvaluator reset");
			// finally return
			return ret.Normalized();
		}
	}
}
