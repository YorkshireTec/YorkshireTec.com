namespace YorkshireDigital.Web.Events.Modules
{
    using AutoMapper;
    using Nancy;
    using Nancy.ModelBinding;
    using NHibernate;
    using YorkshireDigital.Web.Events.ViewModels;
    using YorkshireDigital.Web.Infrastructure;
    using YorkshireDigital.Data.Domain.Events;
    using YorkshireDigital.Data.Services;

    public class EventsModule : BaseModule
    {
        public EventsModule(ISessionFactory sessionFactory)
            : base(sessionFactory, "/events")
        {
            var service = new EventService(RequestSession);

            Get["/{id?}"] = p =>
            {
                var id = p.id;

                if (string.IsNullOrEmpty(id))
                {
                    return Negotiate.WithStatusCode(HttpStatusCode.NotFound);
                }

                Event model = service.Get(id);

                if (model == null)
                {
                    return Negotiate.WithStatusCode(HttpStatusCode.NotFound);
                }

                var viewModel = new EventDetailsModel(model);

                return Negotiate.WithModel(viewModel)
                    .WithStatusCode(HttpStatusCode.OK);
            };

            Post["/"] = _ =>
            {
                var newEvent = this.Bind<CalendarEventModel>();

                service.Save(Mapper.DynamicMap<Event>(newEvent));

                return Negotiate.WithStatusCode(HttpStatusCode.Created);
            };

            Put["/"] = _ => HttpStatusCode.ImATeapot;

            Delete["/{id}"] = p => HttpStatusCode.ImATeapot;
        }
    }
}