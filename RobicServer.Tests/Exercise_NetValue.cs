using System.Collections.Generic;
using NUnit.Framework;
using RobicServer.Models;

namespace RobicServer.Tests
{
    [TestFixture]
    public class Exercise_NetValue
    {
        private List<List<Set>> _data;

        [SetUp]
        public void Setup()
        {
            _data = new List<List<Set>>();
            _data.Add(new List<Set>(){
                new Set() {
                    Reps = 10,
                    Value = 20
                },
                new Set() {
                    Reps = 10,
                    Value = 20
                },
            });
            _data.Add(new List<Set>(){
                new Set() {
                    Reps = 10,
                    Value = null
                },
                new Set() {
                    Reps = 10,
                    Value = 20
                },
            });
            _data.Add(new List<Set>(){
                new Set() {
                    Reps = 10,
                    Value = 20
                },
                new Set() {
                    Reps = null,
                    Value = 20
                },
            });
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

        [TestCase(0, 400)]
        [TestCase(1, 200)]
        [TestCase(2, 200)]
        public void NetValue_Sets_ReturnTotal(int dataIndex, double expectedValue)
        {
            // Arrange
            var sets = _data[dataIndex];
            var exercise = new Exercise()
            {
                Sets = sets
            };
            // Act
            var actual = exercise.NetValue;
            // Assert
            Assert.AreEqual(expectedValue, actual);
        }
    }

}