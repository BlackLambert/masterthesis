namespace SBaier.Master
{
    public class PlanetData
    {
        public float KernalRadius { get; }
        public float AtmosphereRadius { get; }
        public Seed Seed { get; }
        public float HullThickness => AtmosphereRadius - KernalRadius;

        public PlanetData(
            float kernalRadius,
            float atmosphereRadius,
            Seed seed)
		{
            KernalRadius = kernalRadius;
            AtmosphereRadius = atmosphereRadius;
            Seed = seed;
        }
    }
}