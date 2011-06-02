using System;
using FluentNHibernate.Automapping;
using Web.Models;

namespace Web.Infrastructure.Repositories.Config
{
    /// <summary>
    /// This is an example automapping configuration. You should create your own that either
    /// implements IAutomappingConfiguration directly, or inherits from DefaultAutomappingConfiguration.
    /// Overriding methods in this class will alter how the automapper behaves.
    /// </summary>
    class SiteAutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            // specify the criteria that types must meet in order to be mapped
            // any type for which this method returns false will not be mapped.
            return type.Namespace == "Web.Models";
            //return type == typeof(User);
        }

        public override bool IsComponent(Type type)
        {
            // override this method to specify which types should be treated as components
            // if you have a large list of types, you should consider maintaining a list of them
            // somewhere or using some form of conventional and/or attribute design
            //return type == typeof(Location);
            return false;
        }
    }
}