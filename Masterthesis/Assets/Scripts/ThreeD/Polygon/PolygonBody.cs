using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class PolygonBody
    {
		private readonly Polygon[] _polygons;
		private readonly Vector3[] _vertices;

		public PolygonBody(Polygon[] polygons, Vector3[] vertices)
		{
			_polygons = polygons;
			_vertices = vertices;
		}

		public float GetDistanceTo(Vector3 point, int polygonIndex)
		{
			Vector3 pointOnBorder = GetNearestBorderPointOf(point, polygonIndex);
			Vector3 distance = point.FastSubstract(pointOnBorder);
			return distance.magnitude;
		}

		public Vector3 GetNearestBorderPointOf(Vector3 point, int polygonIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			float minDistance = float.MaxValue;
			Vector3 pointOnBorder = Vector3.zero;
			int cornersAmount = polygon.VertexIndices.Length;
			for (int i = 0; i < cornersAmount; i++)
			{
				Vector3 current = GetNearestBorderPointOf(point, polygon, i);
				Vector3 distanceVector = point.FastSubstract(current);
				float distance = distanceVector.magnitude;
				if (distance < minDistance)
				{
					minDistance = distance;
					pointOnBorder = current;
				}
			}
			return pointOnBorder;
		}
		private Vector3 GetNearestBorderPointOf(Vector3 point, Polygon polygon, int vertexIndex)
		{
			int[] indices = polygon.VertexIndices;
			int cornersAmount = indices.Length;
			Vector3 corner0 = _vertices[indices[vertexIndex]];
			Vector3 corner1 = _vertices[indices[(vertexIndex + 1) % cornersAmount]];
			Vector3 border = corner1.FastSubstract(corner0);
			Vector3 pointToCorner0 = point.FastSubstract(corner0);
			float t0 = Vector3.Dot(border, pointToCorner0) / Vector3.Dot(border, border);
			if (t0 <= 0)
				return corner0;
			else if (t0 >= 1)
				return corner1;
			else
				return corner0.FastAdd(border.FastMultiply(t0));
		}

		public Vector3[] GetVertices(int polygonIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			Vector3[] result = new Vector3[polygon.VertexIndices.Length];
			for (int i = 0; i < polygon.VertexIndices.Length; i++)
				result[i] = _vertices[polygon.VertexIndices[i]];
			return result;
		}



		public float GetMaxDiameter(int polygonIndex)
		{
			float result = 0;
			Polygon polygon = _polygons[polygonIndex];
			for (int i = 0; i < polygon.VertexIndices.Length; i++)
			{
				Vector3 c1 = _vertices[polygon.VertexIndices[i]];
				for (int j = i + 1; j < polygon.VertexIndices.Length; j++)
				{
					Vector3 c2 = _vertices[polygon.VertexIndices[j]];
					float distance = (c1.FastSubstract(c2)).magnitude;
					if (distance > result)
						result = distance;
				}
			}
			return result;
		}

		public float GetDistanceToCornersSum(int polygonIndex, Vector3 point)
		{
			Polygon polygon = _polygons[polygonIndex];
			float result = 0;
			for (int i = 0; i < polygon.VertexIndices.Length; i++)
			{
				Vector3 corner = _vertices[polygon.VertexIndices[i]];
				float distance = (point - corner).magnitude;
				result += distance;
			}
			return result;
		}

		public Polygon GetPolygon(int polygonIndex)
		{
			return _polygons[polygonIndex];
		}

		public Vector3 GetVertex (int vertexIndex)
		{
			return _vertices[vertexIndex];
		}

		public Vector3 GetNormal (int polygonIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			Vector3 result = Vector3.zero;
			int vertexCount = polygon.VertexIndices.Length;
			for (int i = 0; i < vertexCount; i++)
			{
				Vector3 v0 = _vertices[polygon.VertexIndices[i]];
				Vector3 v1 = _vertices[polygon.VertexIndices[(i + 1) % vertexCount]];
				result += Vector3.Cross(v0, v1);
			}
			return result.normalized;
		}
	}
}