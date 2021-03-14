using System.Collections.Generic;
using NUnit.Framework;
using RobicServer.Models;

namespace RobicServer.Tests
{
    [TestFixture]
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void NetValue_Empty_ReturnsNull()
        {
            // Arrange
            var exercise = new Exercise();
            // Act
            var actual = exercise.NetValue;
            // Assert
            Assert.AreEqual(null, actual);
        }

        [Test]
        public void NetValue_Sets_ReturnTotal()
        {
            // Arrange
            var set = new Set
            {
                Reps = 10,
                Value = 20
            };
            var sets = new List<Set>();
            sets.Add(set);
            sets.Add(set);
            var exercise = new Exercise()
            {
                Sets = sets
            };
            // Act
            var actual = exercise.NetValue;
            // Assert
            var expected = 10 * 20 * 2;
            Assert.AreEqual(expected, actual);
        }
    }

}