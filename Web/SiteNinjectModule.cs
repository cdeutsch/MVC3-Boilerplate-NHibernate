using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Modules;
using Web.Infrastructure.FormsAuthenticationService;
using Web.Models;
using NHibernate;
using Web.Models.ViewModels;

namespace Web
{
    public class SiteNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFormsAuthenticationService>().To<FormsAuthenticationService>();
            Bind<IMembershipService>().To<AccountMembershipService>();
            Bind<ISession>().ToMethod(x => MvcApplication.SessionFactory.OpenSession());
        }
    }
}