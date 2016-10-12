using NUnit.Framework;

namespace Nostradamus.Tests
{
    static class AssertHelper
    {
        const float FloatAppromiateThreshold = 0.0001f;

        public static void AreApproximate(float expected, float actual)
        {
            Assert.That(actual, Is.EqualTo(expected).Within(FloatAppromiateThreshold));
        }
    }
}