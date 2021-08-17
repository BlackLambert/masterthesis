using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class RandomPointsOnSphereGenerator
	{
		public Vector3[] Generate(int amount, float radius, Seed seed)
		{
			ValidateAmount(amount);
			ValidateRadius(radius);
			return DoGeneration(amount, radius, seed);
		}

		private Vector3[] DoGeneration(int amount, float radius, Seed seed)
		{
			Vector3[] vertices = new Vector3[amount];
			for (int i = 0; i < amount; i++)
				vertices[i] = CreateRandomVertex(seed).normalized * radius;
			return vertices;
		}

		private Vector3 CreateRandomVertex(Seed seed)
		{
			float ran0 = (float)seed.Random.NextDouble();
			float ran1 = (float)seed.Random.NextDouble();
			float angle0 = ran0 * 2 * Mathf.PI;
			float angle1 = Mathf.Acos(2 * ran1 - 1) - (Mathf.PI / 2);
			float x = Mathf.Cos(angle1) * Mathf.Cos(angle0);
			float y = Mathf.Cos(angle1) * Mathf.Sin(angle0);
			float z = Mathf.Sin(angle1);
			Vector3 result = new Vector3(x, y, z);
			if (result == Vector3.zero)
				return CreateRandomVertex(seed);
			return result;
		}

		private void ValidateAmount(int amount)
		{
			if (amount <= 0)
				throw new ArgumentOutOfRangeException();
		}
		private void ValidateRadius(float radius)
		{
			if (radius <= 0)
				throw new ArgumentOutOfRangeException();
		}
	}
}