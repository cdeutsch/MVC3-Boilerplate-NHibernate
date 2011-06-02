using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using NHibernate;
using NHibernate.Linq;
using Web.Models;
using NHibernate.Validator.Constraints;
namespace Web.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(ISession session)
            : base(session)
        {
        }

        public User CreateUser(string Username, string Password, string Email)
        {
            using (ITransaction tx = Session.BeginTransaction())
            {
                //create a new user.
                User user = new User();
                user.Username = Username;
                user.Email = Email;
                user.Enabled = true;
                //create salt for password hash.
                user.PasswordSalt = CreateSalt();
                user.PasswordHash = CreatePasswordHash(Password, user.PasswordSalt);
                user.Created = DateTime.Now;
                user.Updated = user.Created;

                Session.Save(user);
                tx.Commit();

                return user;
            }
            
        }

        public User CompleteRegistration(string Username, string FirstName, string LastName)
        {
            
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                throw new ApplicationException("First name or last name is required.");
            }
            
            //get the user that should have been created by the membership provider.
            User user = GetUser(Username);
            if (user == null) 
            {
                throw new ApplicationException("The newly created User could not be found.");
            }

            //update values membership provider did not set.
            user.FirstName = FirstName;
            user.LastName = LastName;

            Session.Update(user);

            return user;
            
        }

        public User DeleteUser(string Username, bool DeleteData)
        {
            User user = GetUser(Username);
            if (user == null)
            {
                throw new ApplicationException("User not found.");
            }
            if (DeleteData)
            {
                Session.Delete(user);
            }
            else
            {
                user.Enabled = false;
                Session.Update(user);
            }

            return user;
        }

        /// <summary>
        /// Will return a User if the specified Username matches with the Email or Username fields.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        public User GetUser(string Username)
        {
            return GetUser(Username, false);
        }
        /// <summary>
        /// Will return a User if the specified Username matches with the Email or Username fields.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Username"></param>
        /// <param name="IncludeDisabled"></param>
        /// <returns></returns>
        public User GetUser(string Username, bool IncludeDisabled)
        {
            return Session.Query<User>().SingleOrDefault(oo => (oo.Username.ToLower() == Username.ToLower() || oo.Email.ToLower() == Username.ToLower()) && (IncludeDisabled || oo.Enabled == true));
        }
        public User GetUser(long UserId)
        {
            return GetUser(UserId, false);
        }
        public User GetUser(long UserId, bool IncludeDisabled)
        {
            return Session.Query<User>().SingleOrDefault(oo => (oo.Id == UserId) && (IncludeDisabled || oo.Enabled == true));
        }

        public User GetUserByUsername(string Username)
        {
            return (from uu in Session.Query<User>()
                    where uu.Enabled == true
                    && uu.Username.ToLower() == Username.ToLower()
                    select uu).SingleOrDefault();
        }

        public User GetUserByEmail(string Email)
        {
            return (from uu in Session.Query<User>()
                    where uu.Enabled == true
                    && uu.Email.ToLower() == Email.ToLower()
                    select uu).SingleOrDefault();
        }

        public IQueryable<User> FindUserByUsername(string UsernameQuery)
        {
            return from uu in Session.Query<User>()
                        where uu.Enabled == true
                        && uu.Username.ToLower().Contains(UsernameQuery.ToLower())
                        orderby uu.Username
                        select uu;
        }

        public IQueryable<User> FindUserByEmail(string EmailQuery)
        {
            return from uu in Session.Query<User>()
                   where uu.Enabled == true
                   && uu.Email.ToLower().Contains(EmailQuery.ToLower())
                   orderby uu.Email
                   select uu;
        }

        public bool ValidateUser(string Username, string Password)
        {
            bool valid = false;
            User user = GetUser(Username);
            if (user != null)
            {
                //validate password by creating hash using salt.
                if (CreatePasswordHash(Password, user.PasswordSalt) == user.PasswordHash)
                {
                    valid = true;
                    user.LastLogin = DateTime.Now;
                    
                    Session.Update(user);
                }
            }
            return valid;
        }

        public bool ChangePassword(string Username, string OldPassword, string NewPassword)
        {
            bool bSuccess = false;
            User user = GetUser(Username);
            if (user != null)
            {
                //validate password by creating hash using salt.
                if (CreatePasswordHash(OldPassword, user.PasswordSalt) == user.PasswordHash)
                {
                    //ok to change password.
                    user.PasswordSalt = CreateSalt();
                    user.PasswordHash = CreatePasswordHash(NewPassword, user.PasswordSalt);

                    Session.Update(user);
                    bSuccess = true;
                }
            }        
            return bSuccess;
        }

        /// <summary>
        /// Creates Salt with default size of 16.
        /// </summary>
        /// <returns></returns>
        public string CreateSalt()
        {
            //default size to 16.
            return CreateSalt(16);
        }

        public string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        public string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            //string hashedPwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");

            //manually do what FormsAuthentication does so we don't have to rely on a reference to System.Web.
            HashAlgorithm algorithm = SHA1.Create();
            var bytes = algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltAndPwd));
            string hex = BitConverter.ToString(bytes);
            string hashedPwd = hex.Replace("-", "");

            return hashedPwd;
        }
    }
}