namespace SBaier.Master
{
    public class PlanetFaceData
    {
        public EvaluationPointData[] EvaluationPoints { get; }

        public PlanetFaceData(EvaluationPointData[] evaluationPoints)
		{
            EvaluationPoints = evaluationPoints;
        }
    }
}