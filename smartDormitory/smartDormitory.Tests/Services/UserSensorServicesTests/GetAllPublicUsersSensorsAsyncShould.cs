﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using smartDormitory.Data;
using smartDormitory.Data.Context;
using smartDormitory.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace smartDormitory.Tests.Services.UserSensorServicesTests
{
    [TestClass]
    public class GetAllPublicUsersSensorsAsyncShould
    {
        private DbContextOptions<smartDormitoryDbContext> contextOptions;

        //[TestMethod]
        //public async Task ReturnAllPublicUserSensors()
        //{
        //    // Arrange
        //    contextOptions = new DbContextOptionsBuilder<smartDormitoryDbContext>()
        //        .UseInMemoryDatabase(databaseName: "ThrowArgumentException_WhenMinValueIsOutOfSensorValueRange")
        //        .Options;

        //    using (var assertContext = new smartDormitoryDbContext(contextOptions))
        //    {
        //        await assertContext.Sensors.AddAsync(
        //            new Sensor
        //            {
        //                Id = 1,
        //                PollingInterval = 10,
        //                Description = "Some description",
        //                Tag = "Some tag",
        //                MinValue = 10.00,
        //                MaxValue = 20.00,
        //                TimeStamp = DateTime.Now,
        //                Value = 15.00,
        //                Url = "Some URL",
        //                ModifiedOn = DateTime.Now,
        //            });

        //        var userId = Guid.NewGuid().ToString();
        //        await assertContext.UserSensors.AddRangeAsync(
        //            new UserSensors
        //            {
        //                UserId = userId,
        //                SensorId = 1,
        //                Name = "Some name",
        //                Description = "Some description",
        //                MinValue = 11,
        //                MaxValue = 18,
        //                PollingInterval = 13,
        //                Latitude = 1.15,
        //                Longitude = 5.15,
        //                IsPublic = true,
        //                Alarm = false,
        //                IsDeleted = false,
        //                ImageUrl = "Some Url",
        //            },
        //            new UserSensors
        //            {
        //                UserId = userId,
        //                SensorId = 1,
        //                Name = "Some name1",
        //                Description = "Some description1",
        //                MinValue = 12,
        //                MaxValue = 19,
        //                PollingInterval = 23,
        //                Latitude = 2.15,
        //                Longitude = 6.15,
        //                IsPublic = true,
        //                Alarm = false,
        //                IsDeleted = true,
        //                ImageUrl = "Some Url 1",
        //            },
        //            new UserSensors
        //            {
        //                UserId = userId,
        //                SensorId = 1,
        //                Name = "Some name2",
        //                Description = "Some description2",
        //                MinValue = 13,
        //                MaxValue = 20,
        //                PollingInterval = 33,
        //                Latitude = 3.15,
        //                Longitude = 7.15,
        //                IsPublic = false,
        //                Alarm = false,
        //                IsDeleted = false,
        //                ImageUrl = "Some Url 2",
        //            });
        //        await assertContext.Users.AddAsync(new User { Id = userId,UserName = "Gosho", UserSensors new List<UserSensors>() { } });


        //        await assertContext.SaveChangesAsync();
        //    }
        //}



    }
}
