using System;

namespace SBaier.Master
{
	public interface NoiseFactory
	{
		int RecursionDepthLimit { get; set; }
		Noise3D Create(NoiseSettings settings, Seed baseSeed);
		void ClearCache();


		public class RecursionDepthLimitReachedException : Exception
		{

		}
	}
}