using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models;

namespace Web.Infrastructure.Repositories
{
    public static class AuditableRepository
    {
        public static void DefaultAuditableToNow(IAuditable obj)
        {
            obj.Created = DateTime.Now;
            obj.Updated = obj.Created;
        }
    }
}