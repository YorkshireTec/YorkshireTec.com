﻿using System;
using System.Configuration;
using EasyNetQ;
using YorkshireDigital.Data.Messages;

namespace YorkshireDigital.Data.Tasks
{
    public class GroupSyncTask
    {
        public void Execute(string groupId)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MessageQueue"];
            if (connectionString == null || connectionString.ConnectionString == string.Empty)
            {
                throw new Exception("easynetq connection string is missing or empty");
            }

            System.Console.WriteLine("Processing GroupSyncTask for GroupId " + groupId);

            var message = new GroupSyncMessage(groupId);

            using (var bus = RabbitHutch.CreateBus(connectionString.ConnectionString))
            {
                bus.Publish<IHandleMeetupRequest>(message);
            }
            System.Console.WriteLine("Processing Complete");
        }
    }
}