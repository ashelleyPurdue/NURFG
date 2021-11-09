using Godot;
using System;
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
    }
}
