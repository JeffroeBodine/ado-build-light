using NUnit.Framework;
using ADOBuildLight.Models;
using System.Collections.Generic;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class BusinessHoursTests
    {
        [Test]
        public void BusinessHours_Properties_CanBeSet()
        {
            // Arrange
            var businessHours = new BusinessHours();
            var startHour = 9;
            var endHour = 17;
            var daysOfWeek = new List<string> { "Monday", "Tuesday" };

            // Act
            businessHours.StartHour = startHour;
            businessHours.EndHour = endHour;
            businessHours.DaysOfWeek = daysOfWeek;

            // Assert
            Assert.That(businessHours.StartHour, Is.EqualTo(startHour));
            Assert.That(businessHours.EndHour, Is.EqualTo(endHour));
            Assert.That(businessHours.DaysOfWeek, Is.EqualTo(daysOfWeek));
        }
    }
}
