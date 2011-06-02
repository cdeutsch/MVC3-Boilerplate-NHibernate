using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using NHibernate;
using NHibernate.Validator.Constraints;

namespace Web.Models {
    public class UserActivity {

        public UserActivity()
        {

        }

        public virtual long Id { get; private set; }

        public virtual long UserId { get; set; }

        [Length(Max = 50)]
        [StringLength(50)]
        public virtual string Activity { get; set; }

        public virtual DateTime Created { get; set; }

        private readonly ISession _session;
        public UserActivity(ISession session)
        {
            _session = session;
        }
        public virtual void LogIt(long userId, string activity)
        {
            using (ITransaction tx = _session.BeginTransaction())
            {
                var log = new UserActivity();
                log.Activity = activity;
                log.Created = DateTime.Now;
                log.UserId = userId;
                _session.Save(log);
                tx.Commit();
            }
        }
        
    }
}
