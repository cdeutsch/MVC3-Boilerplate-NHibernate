using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Web.Repositories.Config
{
    public class RequiredConvention : AttributePropertyConvention<RequiredAttribute>
    {
        protected override void Apply(RequiredAttribute attribute, IPropertyInstance instance)
        {
            instance.Not.Nullable();
        }
    }
}