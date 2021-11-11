using Godot;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NURFG.Tests
{
    public class SimpleTest
    {
        [Test]
        public void This_Test_Passes()
        {
            Assert.AreEqual(4, 2 + 2);
        }

        [Test]
        public void This_Test_Is_Supposed_To_Fail()
        {
            Assert.Fail();
        }

        [Test]
        public void This_Test_Has_A_Warning()
        {
            Assert.Warn("Warning: this message contains trace amounts of humor.");
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

        [Test]
        public void This_Test_Write_To_Console()
        {
            Console.WriteLine("Hello world!");
        }

        [Test]
        public void This_Test_Uses_Godot_Classes()
        {
            var node = new Node2D();
            node._Process(1337);
        }
    }
}
