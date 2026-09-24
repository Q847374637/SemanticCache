using System.Numerics.Tensors;

namespace Semantic.Core
{
    public static class VectorMath
    {
        public static float SimilarityCheck(float[] incomingVector, float[] cachedVector, bool isNormalized)
        {
            if (incomingVector.Length != cachedVector.Length) throw new ArgumentException("Vectors must have the same length.");

            if (isNormalized == true) return TensorPrimitives.Dot(incomingVector, cachedVector);

            return TensorPrimitives.CosineSimilarity(incomingVector, cachedVector);

        }

    }
}