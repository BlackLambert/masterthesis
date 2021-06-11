

using System;

namespace SBaier.Master
{
	public class OctaveNoise : Noise3D
	{
		public int OctavesCount { get; }
		public Noise3D BaseNoise { get; }
		public double StartFrequency { get; }
		public double StartWeight { get; }

		public NoiseType NoiseType => NoiseType.Octave;


		public OctaveNoise(Arguments args)
		{
			OctavesCount = args.OctavesCount;
			BaseNoise = args.BaseNoise;
			StartFrequency = args.StartFrequency;
			StartWeight = args.StartWeight;
		}

		public double Evaluate(double x, double y, double z)
		{
			double result = 0;
			for (int i = 0; i < OctavesCount; i++)
			{
				double factor = Math.Pow(2, i);
				double ff = StartFrequency * factor;
				double weight = StartWeight / factor;
				result += (BaseNoise.Evaluate(x * ff, y * ff, z * ff) - 0.5f) * weight;
			}
			return Clamp01(result + 0.5f);
		}

		public double Evaluate(double x, double y)
		{
			return Evaluate(x, y, 0);
		}

		private double Clamp01(double result)
		{
			return (result > 1) ? 1 : (result < 0) ? 0 : result;
		}

		public class Arguments
		{
			public int OctavesCount { get; }
			public Noise3D BaseNoise { get; }
			public double StartFrequency { get; }
			public double StartWeight { get; }


			public Arguments(int octavesCount, Noise3D baseNoise, double startFrequency, double startWeight)
			{
				CheckStartWeightOutOfRange(startWeight);
				CheckStartFrequencyOutOfRange(startFrequency);

				OctavesCount = octavesCount;
				BaseNoise = baseNoise;
				StartFrequency = startFrequency;
				StartWeight = startWeight;
			}

			private void CheckStartWeightOutOfRange(double startWeight)
			{
				if (startWeight > 1 || startWeight < 0)
					throw new ArgumentOutOfRangeException();
			}

			private void CheckStartFrequencyOutOfRange(double startFrequency)
			{
				if (startFrequency < 1)
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}