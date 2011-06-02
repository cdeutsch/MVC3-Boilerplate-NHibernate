using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Web.Infrastructure.Repositories.Config
{
    public class TableNameConvention : IClassConvention
    {
        public void Apply(FluentNHibernate.Conventions.Instances.IClassInstance instance)
        {
            string typeName = instance.EntityType.Name;

            instance.Table(typeName.MakePlural());
        }
    }
}