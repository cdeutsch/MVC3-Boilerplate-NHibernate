using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Web.Repositories.Config
{
    public class StringLengthMaxAttribute : StringLengthAttribute
    {
        public StringLengthMaxAttribute()
            : base(10000)
        {
            // 10000 is an arbitrary number large enough to be in the nvarchar(max) range 
        }
    }

    public class StringLengthConvention : AttributePropertyConvention<StringLengthAttribute>
    {
        protected override void Apply(StringLengthAttribute attribute, IPropertyInstance instance)
        {
            instance.Length(attribute.MaximumLength);
        }
    }
}