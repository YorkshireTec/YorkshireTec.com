﻿namespace YorkshireDigital.Web.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using YorkshireDigital.Data.Domain.Events;
    using YorkshireDigital.Data.Domain.Shared;

    public class AdminEventViewModel
    {
        public string UniqueName { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Location { get; set; }
        public string Region { get; set; }
        public decimal Price { get; set; }
        public byte[] Photo { get; set; }
        public string GroupName { get; set; }
        public string GroupId { get; set; }
        public string Interests { get; set; }

        public List<AdminInterestViewModel> AvailableInterests { get; set; }
        public List<AdminEventTalkViewModel> Talks { get; set; }

        public AdminEventViewModel()
        {
            AvailableInterests = new List<AdminInterestViewModel>();
            Talks = new List<AdminEventTalkViewModel>();
        }

        public static AdminEventViewModel FromDomain(Event @event)
        {
            return Mapper.DynamicMap<Event, AdminEventViewModel>(@event);
        }

        public Event ToDomain()
        {
            var domain = Mapper.DynamicMap<AdminEventViewModel, Event>(this);
            domain.SynopsisFormat = TextFormat.Markdown;
            return domain;
        }

        public void UpdateDomain(Event @event)
        {
            Mapper.DynamicMap(this, @event);
            @event.SynopsisFormat = TextFormat.Markdown;
        }
    }
}