using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace Web.Infrastructure.Repositories
{
    public class Repository<T>
    {
        private readonly ISession _session;

        public Repository(ISession session)
        {
            _session = session;
        }

        protected internal virtual ISession Session
        {
            get
            {
                return _session;
            }
        }

        public virtual void Add(T obj)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.Save(obj);
                transaction.Commit();
            }
        }
        public virtual void Update(T obj)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.Update(obj);
                transaction.Commit();
            }
        }
        public virtual void Save(T obj)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(obj);
                transaction.Commit();
            }
        }
        public virtual void Delete(T obj)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.Delete(obj);
                transaction.Commit();
            }
        }
    }
}