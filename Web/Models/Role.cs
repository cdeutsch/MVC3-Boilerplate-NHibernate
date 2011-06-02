using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using NHibernate.Validator.Constraints;

namespace Web.Models {

    public class Role 
    {
        public Role()
        {
            //this seems to be necessary if DB is empty.
            if (Users == null)
            {
                Users = new List<User>();
            }
        }

        public virtual long Id { get; private set; }

        [Length(Max = 50), NotNull]
        [StringLength(50)]
        [Required(ErrorMessage = "Role name is required")]
        public virtual string RoleName { get; set; }

        //relationships:
        public virtual ICollection<User> Users { get; set; }

        //overrides basic equality. By overriding this
        //you're telling the container how to find this object
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Role))
            {
                var comp = (Role)obj;
                return comp.Id.Equals(this.Id);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }

        

    }

}
