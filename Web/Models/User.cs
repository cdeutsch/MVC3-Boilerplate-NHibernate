using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using NHibernate.Validator.Constraints;

namespace Web.Models {

    public class User : IAuditable
    {
        public User()
        {
            ////this seems to be necessary if DB is empty.
            //if (Roles == null)
            //{
            //    Roles = new List<Role>();
            //}
        }

        public virtual long Id { get; private set; }

        [Length(Max = 320), NotNull]
        [StringLength(320)]
        [Required(ErrorMessage = "Email is required")]
        public virtual string Email { get; set; }

        [Length(Max = 50), NotNull]
        [StringLength(50)]
        [Required(ErrorMessage = "Username is required")]
        public virtual string Username { get; set; }

        [Length(Max = 100)]
        [StringLength(100)]
        public virtual string PasswordHash { get; set; }

        [Length(Max = 25)]
        [StringLength(25)]
        public virtual string PasswordSalt { get; set; }

        [Length(Max = 100)]
        [StringLength(100)]
        public virtual string FirstName { get; set; }

        [Length(Max = 100)]
        [StringLength(100)]
        public virtual string LastName { get; set; }

        [Length(Max = 250)]
        [StringLength(250)]
        public virtual string Address1 { get; set; }

        [Length(Max = 250)]
        [StringLength(250)]
        public virtual string Address2 { get; set; }

        [Length(Max = 250)]
        [StringLength(250)]
        public virtual string City { get; set; }

        [Length(Max = 250)]
        [StringLength(250)]
        public virtual string State { get; set; }

        [Length(Max = 250)]
        [StringLength(250)]
        public virtual string Zip { get; set; }

        [Length(Max = 250)]
        [StringLength(250)]
        public virtual string Country { get; set; }

        [Length(Max = 50)]
        [StringLength(50)]
        public virtual string Phone { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual DateTime? LastLogin { get; set; }
        public virtual DateTime Updated { get; set; }
        public virtual DateTime Created { get; set; }

        //relationships:
        public virtual ICollection<Role> Roles { get; set; }

        public virtual string GetFriendlyName()
        {
            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
            {
                return (FirstName + " " + LastName).Trim();
            }
            else
            {
                return "";
            }
        }

        public virtual void JustLoggedIn()
        {
            Updated = DateTime.Now;
            LastLogin = DateTime.Now;
        }

        //overrides basic equality. By overriding this
        //you're telling the container how to find this object
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(User))
            {
                var comp = (User)obj;
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
