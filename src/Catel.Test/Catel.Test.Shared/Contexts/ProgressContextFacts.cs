// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressContextFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Contexts
{
    using NUnit.Framework;

    [TestFixture]
    public class ProgressContextFacts
    {
        [TestCase(10, 0, 10, 0d)]
        [TestCase(10, 1, 10, 10d)]
        [TestCase(100, 1, 10, 1d)]
        [TestCase(100, 2, 10, 2d)]
        [TestCase(100, 5, 10, 5d)]
        [TestCase(100, 8, 10, 8d)]
        [TestCase(100, 10, 10, 10d)]
        [TestCase(100, 12, 10, 12d)]
        [TestCase(100, 50, 10, 50d)]
        public void CorrectlyCalculatesPercentage(int totalCount, int currentCount, int numberOfRefreshes, double expectedPercentage)
        {
            var progressContext = new ProgressContext(totalCount, numberOfRefreshes);
            progressContext.CurrentCount = currentCount;

            var percentage = progressContext.Percentage;

            Assert.AreEqual(expectedPercentage, percentage);
        }

        [TestCase(10, 0, 10, 0)]
        [TestCase(10, 1, 10, 1)]
        [TestCase(100, 1, 10, 0)]
        [TestCase(100, 2, 10, 0)]
        [TestCase(100, 5, 10, 0)]
        [TestCase(100, 8, 10, 0)]
        [TestCase(100, 10, 10, 1)]
        [TestCase(100, 12, 10, 1)]
        [TestCase(100, 50, 10, 5)]
        public void CorrectlyCalculatesCurrentRefreshNumber(int totalCount, int currentCount, int numberOfRefreshes, int expectedRefreshNumber)
        {
            var progressContext = new ProgressContext(totalCount, numberOfRefreshes);
            progressContext.CurrentCount = currentCount;

            var currentRefreshNumber = progressContext.CurrentRefreshNumber;

            Assert.AreEqual(expectedRefreshNumber, currentRefreshNumber);
        }
    }
}