﻿namespace YorkshireTec.MailingList.Modules
{
    using Nancy;
    using NHibernate;
    using YorkshireTec.Infrastructure;
    using YorkshireTec.MailingList.ViewModels;

    public class SubscribedModule : BaseModule
    {
        public SubscribedModule(ISessionFactory sessionFactory)
            : base(sessionFactory, "MailingList/Subscribed")
        {
            Get["/"] = _ =>
            {
                var viewModel = new IndexViewModel();
                var model = GetBaseModel(viewModel);

                model.Page.Title = "Bringing Yorkshires digital community together";
                return Negotiate.WithModel(model).WithView("Subscribed");
            };
        }
    }
}