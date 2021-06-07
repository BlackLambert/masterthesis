namespace SBaier.Master
{
	public interface Noise3D : Noise2D
	{
		double Evaluate(double x, double y, double z);
	}
}