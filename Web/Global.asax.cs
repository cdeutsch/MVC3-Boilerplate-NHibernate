using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.Models;
using System.Data.Entity;
using Ninject.Web.Mvc;
using NHibernate;
using NHibernate.Context;
using Ninject;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.IO;
using FluentNHibernate.Automapping;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Conventions.Helpers;
using Web.Infrastructure.Repositories.Config;
using Web.Repositories.Config;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Event;
using NHibernate.Validator.Cfg.Loquacious;

namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : NinjectHttpApplication
    {
        private const string DbFile = "|DataDirectory|SiteDB.sdf";

        /// <summary>
        /// Create the Nhibernate Sessionfactory
        /// </summary>
        public static ISessionFactory SessionFactory = CreateSessionFactory();
        public MvcApplication()
        {
            this.BeginRequest += new EventHandler(MvcApplication_BeginRequest);
            this.EndRequest += new EventHandler(MvcApplication_EndRequest);
        }

        void MvcApplication_EndRequest(object sender, EventArgs e)
        {
            CurrentSessionContext.Unbind(SessionFactory).Dispose();
        }

        void MvcApplication_BeginRequest(object sender, EventArgs e)
        {
            CurrentSessionContext.Bind(SessionFactory.OpenSession());
        }

        public static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(MsSqlCeConfiguration.Standard.ShowSql().ConnectionString(c => c.Is("data source=" + DbFile)))
                .Mappings(m =>
                    m.AutoMappings.Add(CreateAutomappings))
                //.Mappings(m =>
                //    m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                    //m.FluentMappings.AddFromAssemblyOf<Web.Models.User>())
                .ExposeConfiguration(ConfigureNhibernate)
                .BuildSessionFactory();

            
            //return Fluently.Configure()
            //    .Database(MsSqlCeConfiguration.Standard.ShowSql().ConnectionString(c => c.Is("data source=" + DbFile)))
            //    .Mappings(m =>
            //        m.AutoMappings.Add(CreateAutomappings))
            //    .ExposeConfiguration(BuildSchema)
            //    .BuildSessionFactory();



            //var cfg = new Configuration().Configure(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nhibernate.config"));
            //NHibernateProfiler.Initialize();
            //return cfg.BuildSessionFactory();

        }

        private static void ConfigureNhibernate(Configuration config)
        {
            var provider = new NHibernateSharedEngineProvider();
            NHibernate.Validator.Cfg.Environment.SharedEngineProvider = provider;

            var nhvConfiguration = new NHibernate.Validator.Cfg.Loquacious.FluentConfiguration();

            nhvConfiguration
               .SetDefaultValidatorMode(ValidatorMode.OverrideExternalWithAttribute)
               .Register(Assembly.GetExecutingAssembly().ValidationDefinitions())
               .IntegrateWithNHibernate
                   .AvoidingDDLConstraints()
                   .RegisteringListeners();

            ValidatorEngine validatorEngine = NHibernate.Validator.Cfg.Environment.SharedEngineProvider.GetEngine();
            validatorEngine.Configure(nhvConfiguration);

            ValidatorInitializer.Initialize(config, validatorEngine);


            config.SetProperty("current_session_context_class", "thread_static");

            //fix bug: http://stackoverflow.com/questions/2361730/assertionfailure-null-identifier-fluentnh-sqlserverce
            config.SetProperty("connection.release_mode", "on_close");

            //// delete the existing db on each run
            //if (File.Exists(DbFile))
            //    File.Delete(DbFile);

            if (!File.Exists(HttpContext.Current.Server.MapPath(DbFile.Replace(@"|DataDirectory|", @"~/App_Data/"))))
            {
                SqlCEDbHelper.CreateDatabaseFile(DbFile);

                // this NHibernate tool takes a configuration (with mapping info in)
                // and exports a database schema from it
                new SchemaExport(config)
                    .Create(false, true);
            }
        }

        static AutoPersistenceModel CreateAutomappings()
        {
            // This is the actual automapping - use AutoMap to start automapping,
            // then pick one of the static methods to specify what to map (in this case
            // all the classes in the assembly that contains Employee), and then either
            // use the Setup and Where methods to restrict that behaviour, or (preferably)
            // supply a configuration instance of your definition to control the automapper.
            return AutoMap.AssemblyOf<User>(new SiteAutomappingConfiguration())
                .Conventions.Add(ConventionBuilder.Id.Always(x => x.GeneratedBy.Identity()))
                .Conventions.Add<CascadeConvention>()
                .Conventions.Add<TableNameConvention>()
                .Conventions.Add<RequiredConvention>()
                .Conventions.Add<StringLengthConvention>();
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.MapRoute(
                "Login", // Route name
                "login", // URL with parameters
                new { controller = "Session", action = "login" } // Parameter defaults
            );
            routes.MapRoute(
                "Logout", // Route name
                "logout", // URL with parameters
                new { controller = "Session", action = "logout" } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Error()
        {
            Exception lastException = Server.GetLastError();
            // Log the exception.
            Elmah.ErrorSignal.FromCurrentContext().Raise(lastException);
        }

        /// <summary>
        /// Ninject Kernel loads 
        /// </summary>
        /// <returns></returns>
        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            return kernel;
        }

        /// <summary>
        /// Creates the controllerfactory (I think)
        /// </summary>
        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}