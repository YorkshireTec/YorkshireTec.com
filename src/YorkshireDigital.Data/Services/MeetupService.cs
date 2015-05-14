﻿namespace YorkshireDigital.Data.Services
{
    using System;
    using System.Linq;
    using YorkshireDigital.MeetupApi.Clients;
    using YorkshireDigital.MeetupApi.Models;
    using YorkshireDigital.MeetupApi.Requests;

    public class MeetupService
    {
        private readonly IMeetupClient meetupClient;

        public MeetupService(IMeetupClient meetupClient)
        {
            this.meetupClient = meetupClient;
        }

        public bool GroupExists(string groupName)
        {
            var request = new GroupsRequest {GroupUrlName = groupName};

            var response = meetupClient.Groups.Get(request);

            return response.Results.Any();
        }

        public Group GetGroup(string groupName)
        {
            var request = new GroupsRequest { GroupUrlName = groupName };

            var response = meetupClient.Groups.Get(request);

            return response.Results.SingleOrDefault();
        }

        public void LinkGroup(Domain.Group.Group @group, string groupName)
        {
            var meetupGroup = GetGroup(groupName);

            if (meetupGroup == null)
            {
                throw new Exception(string.Format("No group found with name {0}", groupName));
            }

            @group.MeetupId = meetupGroup.Id;
            
            // TODO: Create New event sync background task

            // var id = BackgroundJob.Enqueue<NewEventSyncTask>(x => x.Execute(@group.Id, meetupGroup.Id), Cron.Hourly);
            // @group.NewEventSyncTaskId = id;
        }
    }
}
