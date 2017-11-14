using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Configuration;
using Facebook;
using Newtonsoft.Json.Linq;
using ShipperHN.Business.Entities;
using ShipperHN.Business.LOG;

namespace ShipperHN.Business
{
    public class CommentBusiness
    {
        private readonly LogControl _logControl;
        private readonly ShipperHNDBcontext _shipperHndBcontext;

        public CommentBusiness(ShipperHNDBcontext shipperHndBcontext)
        {
            _logControl = new LogControl();
            _shipperHndBcontext = shipperHndBcontext;
        }

        public void AddComment(Comment comment)
        {
            try
            {
                _shipperHndBcontext.Comments.Add(comment);
                _shipperHndBcontext.SaveChanges();
            }
            catch (Exception e)
            {
                _logControl.AddLog(1, "CommentBusiness.cs/AddComment", "Type: " + e.GetType()
                                                                + " | Message: " + e.Message + " | InnerException: " +
                                                                e.InnerException);
            }
        }

        public List<Comment> GetAllComments()
        {
            List<Comment> comments = new List<Comment>(_shipperHndBcontext.Set<Comment>()
                .Include(c => c.User));
            return comments;
        }

        public List<Comment> GetTop20Comments()
        {
            List<Comment> comments = new List<Comment>(_shipperHndBcontext.Set<Comment>().OrderByDescending(c => c.Time).Take(20));
            return comments;
        }

        public List<Comment> GetTop20CommentsFB(string idpost)
        {
            List<Comment> comments = new List<Comment>();
            FacebookClient fc = new FacebookClient();
            string clientId = WebConfigurationManager.AppSettings["CLIENT_ID"];
            string clientSecret = WebConfigurationManager.AppSettings["CLIENT_SECRET"];
            dynamic result = fc.Get("oauth/access_token", new
            {
                client_id = clientId,
                client_secret = clientSecret,
                grant_type = "client_credentials"
            });
            fc.AccessToken = result.access_token;

            UserBusiness userBusiness = new UserBusiness(_shipperHndBcontext);
            string cmsString = "";
            try
            {
                cmsString = fc.Get(idpost + "?fields=comments.limit(20){created_time,message,from}").ToString();
            }
            catch (Exception e)
            {
                _logControl.AddLog(1, "CommentBusiness.cs/GetTop20CommentsFB", "Type: " + e.GetType()
                                                                + " | Message: " + e.Message + " | InnerException: " +
                                                                e.InnerException);
                return comments;
            }

            if (string.IsNullOrEmpty(cmsString))
            {
                return comments;
            }
            
            JObject jo = JObject.Parse(cmsString);
            if (jo?["comments"] != null)
            {
                List<User> users = new List<User>();
                for (int i = 0; i < jo["comments"]["data"].Count(); i++)
                {
                    users.Add(userBusiness.GetUser(fc, jo["comments"]["data"][i]["from"]["id"].ToString()));
                }
                for (int i = 0; i < jo["comments"]["data"].Count(); i++)
                {
                    if (!jo["comments"]["data"][i]["message"].ToString().Equals(".") && !jo["comments"]["data"][i]["message"].ToString().Trim().Equals(""))
                    {
                        Comment comment = new Comment
                        {
                            User = users[i],
                            Time = DateTime.Parse(jo["comments"]["data"][i]["created_time"].ToString()),
                            Message = jo["comments"]["data"][i]["message"].ToString(),
                            Type = "facebook"
                        };
                        comments.Add(comment);
                    }
                }
            }
            
            return comments;
        }
    }
}
