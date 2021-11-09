using Godot;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace GodotNUnitRunner.Tests
{
    public class SimpleTest
    {
        [Test]
        public void Two_Plus_Two_Equals_Four()
        {
            Assert.AreEqual(4, 2 + 2);
        }

        [Test]
        public void This_Test_Takes_A_Long_Time()
        {
            System.Threading.Thread.Sleep(5000);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void This_Test_Has_Multiple_Cases(int caseNum)
        {
            Assert.Pass();
        }

        [TestCaseSource(nameof(CaseNumSource))]
        public void This_Test_Gets_Cases_From_An_IEnumerable(int caseNum)
        {
            Assert.Pass();
        }
        private static IEnumerable<object[]> CaseNumSource()
        {
            for (int i = 0; i < 4; i++)
                yield return new object[] { i };
        }
    }
}
