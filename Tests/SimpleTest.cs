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
    }
}
