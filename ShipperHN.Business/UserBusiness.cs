using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Facebook;
using Newtonsoft.Json.Linq;
using ShipperHN.Business.Entities;
using ShipperHN.Business.LOG;

namespace ShipperHN.Business
{
    public class UserBusiness
    {
        private readonly ShipperHNDBcontext _shipperHndBcontext;
        private readonly LogControl _logControl;

        public UserBusiness(ShipperHNDBcontext shipperHndBcontext)
        {
            _logControl = new LogControl();
            _shipperHndBcontext = shipperHndBcontext;
        }

        public User GetUser(FacebookClient fc, string userid)
        {
            if (_shipperHndBcontext.Users.FirstOrDefault(x => x.Id.Equals(userid)) != null)
            {
                return _shipperHndBcontext.Users.FirstOrDefault(x => x.Id.Equals(userid));
            }
            string userFetch = fc.Get(userid + "?fields=picture,name").ToString();
            JObject jsonUser = JObject.Parse(userFetch);
            User user = new User
            {
                Id = (string)jsonUser["id"],
                Name = (string)jsonUser["name"],
                AvataLink = (string)jsonUser["picture"]["data"]["url"],
                LatestViewNotificationTime = DateTime.Now
            };
            try
            {
                _shipperHndBcontext.Users.Add(user);
                _shipperHndBcontext.SaveChanges();
            }
            catch (Exception e)
            {
                _logControl.AddLog(0, "PostBusiness.cs/AddPosts", "Type: " + e.GetType()
                                    + " | Message: " + e.Message + " | InnerException: " + e.InnerException);
            }
            return user;
        }

        public List<User> SearchUserByName(string name)
        {
            List<User> users = _shipperHndBcontext.Users.Where(x => x.Name.Trim().ToLower().Contains(name.Trim().ToLower()))
                .Include(x => x.PhoneNumbers)
                .OrderBy(x => x.Name)
                .Take(10).ToList();
            return users;
        }

        public List<User> SearchUserByPhone(string phone)
        {
            List<User> users = _shipperHndBcontext.Users
                .Where(x => x.PhoneNumbers.Any(y => y.Phone.Trim().Contains(phone.Trim())))
                .Include(x => x.PhoneNumbers)
                .OrderBy(x => x.Name)
                .Take(10).ToList();
            return users;
        }
    }
}
