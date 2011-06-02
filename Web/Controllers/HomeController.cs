using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Infrastructure.FormsAuthenticationService;
using NHibernate;
using Web.Infrastructure.Repositories;

namespace Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public IFormsAuthenticationService FormsAuthService { get; set; }

        private ISession _session;
        private UserRepository _userRepository;

        public HomeController(ISession session, IFormsAuthenticationService FormsAuthService)
        {
            _session = session;
            _userRepository = new UserRepository(_session);

            this.FormsAuthService = FormsAuthService;
        }

        public ActionResult Index()
        {
            var model = 1;

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }

}
