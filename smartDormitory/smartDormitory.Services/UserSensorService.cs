﻿using Microsoft.EntityFrameworkCore;
using smartDormitory.Data;
using smartDormitory.Data.Context;
using smartDormitory.Data.Models;
using smartDormitory.Services.Contracts;
using smartDormitory.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smartDormitory.Services
{
    public class UserSensorService : IUserSensorService
    {
        private readonly smartDormitoryDbContext context;

        public UserSensorService(smartDormitoryDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddSensorAsync(string userId, int sensorId, string name, string description, double minValue, double maxValue, int pollingInterval, double latitude, double longitude, bool isPublic, bool alarm, string imageUrl)
        {
            Validator.IfNull<ArgumentNullException>(userId, "Sensor Id cannot be less or equal 0!");
            Validator.IfNull<ArgumentNullException>(name, "Name cannot be null!");
            Validator.IfNull<ArgumentNullException>(description, "Description cannot be null!");
            Validator.IfIsNotPositive(sensorId, "Sensor Id cannot be less than 0!");
            Validator.IfIsNotInRangeInclusive(name.Length, 3, 20, "Name must be between 3 and 20 symbols!");
            Validator.IfIsNotInRangeInclusive(description.Length, 3, 250, "Description must be between 3 and 250 symbols!");
            Validator.IfNull<ArgumentNullException>(imageUrl, "Image URL cannot be null or empty!");
            Validator.IfIsNotInRangeInclusive(pollingInterval, 10, 40, "Polling interval must be between 10 and 40 seconds!");

            var sensor = await this.context.Sensors.FirstOrDefaultAsync(s => s.Id == sensorId);

            Validator.IfNull<ArgumentNullException>(sensor, $"Sensor with Id {sensorId} does not exist!");

            Validator.IfIsNotInRangeInclusive(minValue, sensor.MinValue, sensor.MaxValue, $"Minimal value must be between {sensor.MinValue} and {sensor.MaxValue}!");
            Validator.IfIsNotInRangeInclusive(maxValue, sensor.MinValue, sensor.MaxValue, $"Maximal value must be between {sensor.MinValue} and {sensor.MaxValue}!");                  

            var newUserSensor = new UserSensors
            {
                SensorId = sensorId,
                UserId = userId,
                Name = name,
                Description = description,
                MinValue = minValue,
                MaxValue = maxValue,
                PollingInterval = pollingInterval,
                Latitude = latitude,
                Longitude = longitude,
                IsPublic = isPublic,
                Alarm = alarm,
                ImageUrl = imageUrl,
                IsDeleted = false
            };

            this.context.UserSensors.Add(newUserSensor);
            this.context.SaveChanges();
        }

        public async Task<IEnumerable<UserSensors>> GetAllPublicUsersSensorsAsync()
        {
            return await this.context
                                .UserSensors
                                .Where(s => s.IsPublic && !s.IsDeleted)
                                .Include(u => u.User)
                                .Include(s => s.Sensor)
                                .ToListAsync();
        }

        public async Task<IEnumerable<double>> GetSensorsTypeMinMaxValues(string tag)
        {
            Validator.IfNull<ArgumentNullException>(tag, "Tag cannot be null!");

            var minMax = await this.context.Sensors
                .Where(s => s.Tag.Contains(tag.ToLower(), StringComparison.InvariantCultureIgnoreCase))
                .Select(s => new
                {
                    s.MinValue,
                    s.MaxValue
                })
               .ToListAsync();

            return new List<double> { minMax[0].MinValue, minMax[0].MaxValue };
        }

        public async Task<IEnumerable<UserSensors>> GetAllUserSensorsAsync(string id)
        {
            Validator.IfNull<ArgumentNullException>(id, "Id cannot be null!");

            return await this.context.UserSensors
                .Where(us => us.UserId == id && !us.IsDeleted)
                .Include(s => s.Sensor)
                    .ThenInclude(s => s.MeasureType)
                .ToListAsync();
        }

        public int TotalContainingText(string searchText)
        {
            Validator.IfNull<ArgumentNullException>(searchText, "Search text cannot be null!");

            return this.context.UserSensors
                .Where(s => s.Sensor.Tag.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) && !s.IsDeleted)
                .ToList()
                .Count();
        }

        public int Total()
        {
            return this.context.UserSensors.Where(s => !s.IsDeleted).Count();
        }

        public async Task<IEnumerable<UserSensors>> GetAllPrivateUserSensorsAsync(string id)
        {
            Validator.IfNull<ArgumentNullException>(id, "Id cannot be null!");

            return await this.context.UserSensors
                                     .Where(s => s.UserId == id && !s.IsPublic && !s.IsDeleted)
                                     .Include(u => u.User)
                                     .Include(s => s.Sensor)
                                     .ToListAsync();
        }

        public async Task<IEnumerable<UserSensors>> GetAllUsersSensorsAsync(string searchByName, string searchByTag, int page = 1, int pageSize = 10)
        {
            Validator.IfNull<ArgumentNullException>(searchByName, "Search by name text cannot be null!");
            Validator.IfNull<ArgumentNullException>(searchByTag, "Search by tag text cannot be null");

            return await this.context.UserSensors
                .Where(us => us.User.UserName.Contains(searchByName, StringComparison.InvariantCultureIgnoreCase) && !us.IsDeleted)
                .Where(us => us.Sensor.Tag.Contains(searchByTag, StringComparison.InvariantCultureIgnoreCase) && !us.IsDeleted)
                .Include(s => s.Sensor)
                    .ThenInclude(s => s.MeasureType)
                .Include(u => u.User)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public int TotalByName(string textName)
        {
            return this.context.UserSensors.Where(s => s.User.UserName
                                               .Contains(textName, StringComparison.InvariantCultureIgnoreCase) && !s.IsDeleted)
                                           .Include(s => s.User)
                                           .Count();
        }

        public async Task EditSensor(int userSensorId, string icbSensorId, string name, string description, double minValue, double maxValue, int pollingInterval, double latitude, double longitude, bool isPublic, bool alarm)
        {

            Validator.IfIsNotPositive(userSensorId, "User sensor Id cannot be less or equal 0!");
            Validator.IfNull<ArgumentNullException>(icbSensorId, "Icb sensor Id cannot be null!");
            Validator.IfNull<ArgumentNullException>(name, "Name cannot be null!");
            Validator.IfIsNotInRangeInclusive(name.Length, 3, 20, "Name must be between 3 and 20 symbols!");
            Validator.IfNull<ArgumentNullException>(description, "Description cannot be null!");
            Validator.IfIsNotInRangeInclusive(pollingInterval, 10, 40, "Polling interval must be between 10 and 40 seconds!");          

            var sensor = await this.context.Sensors.FirstOrDefaultAsync(s => s.IcbSensorId == icbSensorId);  
         
            Validator.IfNull<ArgumentNullException>(sensor, $"ICB Sensor with Id {icbSensorId} does not exist!");
            Validator.IfIsNotInRangeInclusive(minValue, sensor.MinValue, sensor.MaxValue, $"Minimal value must be between {sensor.MinValue} and {sensor.MaxValue}!");
            Validator.IfIsNotInRangeInclusive(maxValue, sensor.MinValue, sensor.MaxValue, $"Maximal value must be between {sensor.MinValue} and {sensor.MaxValue}!");                     

            var userSensorToUpdate = await this.context.UserSensors.FirstOrDefaultAsync(s => s.Id == userSensorId);

            Validator.IfNull<ArgumentNullException>(userSensorToUpdate, "Error: Sensor not found!");       

            userSensorToUpdate.Name = name;
            userSensorToUpdate.Description = description;
            userSensorToUpdate.MinValue = minValue;
            userSensorToUpdate.MaxValue = maxValue;
            userSensorToUpdate.PollingInterval = pollingInterval;
            userSensorToUpdate.Latitude = latitude;
            userSensorToUpdate.Longitude = longitude;
            userSensorToUpdate.IsPublic = isPublic;
            userSensorToUpdate.Alarm = alarm;        

            this.context.UserSensors.Update(userSensorToUpdate);
            this.context.SaveChanges();
        }

        public async Task DeleteSensor(int id)
        {
            Validator.IfIsNotPositive(id, "Invalid Id!");            

             var sensor = await this.context.UserSensors.
                FirstOrDefaultAsync(s => s.Id == id);

            Validator.IfNull<ArgumentNullException>(sensor, $"Sensor with Id {id} does not exist!");            

            sensor.IsDeleted = true;

            this.context.UserSensors.Update(sensor);
            this.context.SaveChanges();
        }

        public async Task<Sensor> UpdateSensorValue(string apiSensorId)
        {
            Validator.IfNull<ArgumentNullException>(apiSensorId, "Sensor Id cannot be null!");                   

            return await this.context.Sensors.FirstOrDefaultAsync(s => s.IcbSensorId == apiSensorId);
        }

        public async Task<IEnumerable<UserSensors>> GetAllPublicAndPrivateUsersSensorsAsync()
        {
            return await this.context.UserSensors
                                     .Where(s => !s.IsDeleted)
                                     .Include(u => u.User)
                                     .Include(s => s.Sensor)
                                     .ToListAsync();
        }
    }
}
