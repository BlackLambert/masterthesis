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
			int[] indices = polygon.VertexIndices;
			for (int i = 0; i < cornersAmount; i++)
			{
				Vector3 current = GetNearestBorderPointOf(point, indices, i);
				Vector3 distanceVector = point.FastSubstract(current);
				float distance = distanceVector.sqrMagnitude;
				if (distance < minDistance)
				{
					minDistance = distance;
					pointOnBorder = current;
				}
			}
			return pointOnBorder;
		}

		private Vector3 GetNearestBorderPointOf(Vector3 point, int[] indices, int vertexIndex)
		{
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
				result += GetBorderNormal(polygon, vertexCount, i);
			return result.normalized;
		}

		private Vector3 GetBorderNormal(Polygon polygon, int vertexCount, int vertexIndex)
		{
			Vector3 v0 = _vertices[polygon.VertexIndices[vertexIndex]];
			Vector3 v1 = _vertices[polygon.VertexIndices[(vertexIndex + 1) % vertexCount]];
			return Vector3.Cross(v0, v1);
		}
	}
}