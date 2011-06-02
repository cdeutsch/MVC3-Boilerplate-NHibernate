using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models;
using NHibernate;
using NHibernate.Linq;

namespace Web.Infrastructure.Repositories
{
    public class RoleRepository : Repository<Role>
    {
        public RoleRepository(ISession session)
            : base(session)
        {
        }

        public Role CreateRole(string RoleName)
        {
            using (ITransaction tx = Session.BeginTransaction())
            {
                Role role = new Role();
                role.RoleName = RoleName;

                Session.Save(role);
                tx.Commit();

                return role;
            }
        }

        public Role GetRole(string RoleName)
        {
            return Session.Query<Role>().SingleOrDefault(oo => oo.RoleName.ToLower() == RoleName.ToLower());
        }

        public bool IsUserInRole(string Username, string RoleName)
        {
            return FindUsersInRole(RoleName, Username).Count() == 1;
        }

        public bool RoleExists(string RoleName)
        {
            return Session.Query<Role>().Where(rr => rr.RoleName.ToLower() == RoleName.ToLower()).Count() == 1;
        }

        public IQueryable<User> FindUsersInRole(string RoleName)
        {
            return from uu in Session.Query<User>()
                   where uu.Roles.Any(rr => rr.RoleName.ToLower() == RoleName.ToLower())
                    && uu.Enabled == true
                   orderby uu.Username
                   select uu;
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="RoleName"></param>
        /// <param name="UsernameQuery">Partial username.</param>
        /// <returns></returns>
        public IQueryable<User> FindUsersInRole(string RoleName, string UsernameQuery)
        {
            return from uu in FindUsersInRole(RoleName)
                   where uu.Username.ToLower().Contains(UsernameQuery.ToLower())
                   orderby uu.Username
                   select uu;
        }

        public IQueryable<Role> GetRolesForUser(string Username)
        {
            return from rr in Session.Query<Role>()
                   where rr.Users.Any(uu => uu.Username.ToLower() == Username.ToLower() && uu.Enabled == true)
                   select rr;
        }

        public void AddUsersToRoles(string[] Usernames, string[] RoleNames)
        {
            using (ITransaction tx = Session.BeginTransaction())
            {
                //get all roles
                List<Role> lstRoles = Session.Query<Role>().ToList();
                //loop thru users.
                foreach (String username in Usernames)
                {
                    //get the user.
                    var _userRepository = new UserRepository(MvcApplication.SessionFactory.GetCurrentSession());
                    User user = _userRepository.GetUser(username);
                    if (user != null)
                    {
                        //loop thru roles.
                        foreach (string rolename in RoleNames)
                        {
                            //find the roleid of the role we need to add.
                            Role role = lstRoles.SingleOrDefault(oo => oo.RoleName.ToLower() == rolename.ToLower());
                            if (role != null)
                            {
                                //check if the user already has this role.
                                if (!user.Roles.Contains(role))
                                {
                                    //add the role.
                                    user.Roles.Add(role);
                                }
                            }
                        }
                    }
                    Session.Update(user);
                }
                tx.Commit();
            }
            
        }

        public void RemoveUsersFromRoles(string[] Usernames, string[] RoleNames)
        {
            using (ITransaction tx = Session.BeginTransaction())
            {
                foreach (string roleName in RoleNames)
                {
                    //get the role
                    Role role = GetRole(roleName);
                    if (role != null)
                    {
                        foreach (string userName in Usernames)
                        {
                            User user = role.Users.SingleOrDefault(uu => uu.Username.ToLower() == userName.ToLower());
                            if (user != null)
                            {
                                role.Users.Remove(user);
                            }
                            Session.Update(user);
                        }
                    }

                }
                tx.Commit();
            }
            
        }

        public bool DeleteRole(string RoleName, bool ErrorIfPopulated)
        {
            using (ITransaction tx = Session.BeginTransaction())
            {
                Role role = GetRole(RoleName);
                if (role != null)
                {
                    ////TODO: make sure NHibernate cascades the delte
                    //if (ErrorIfPopulated) //  && GetUsersInRole(roleName).Length > 0)
                    //{
                    //    if (role.Users.Count() > 0)
                    //    {
                    //        throw new ApplicationException("Cannot delete a populated role.");
                    //    }
                    //}
                    //else
                    //{
                    //    //remove all users in this role (we shouldn't get here if throwOnPopulatedRole is true.
                    //    foreach (User user in role.Users)
                    //    {
                    //        role.Users.Remove(user);
                    //    }
                    //}

                    //remove the role.
                    Session.Delete(role);
                    tx.Commit();

                    return true;
                }
                else
                {
                    throw new ApplicationException("Role does not exist.");
                }
            }
        }
    }
}