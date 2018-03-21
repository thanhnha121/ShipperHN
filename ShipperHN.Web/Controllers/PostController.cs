using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.Threading;
using System.Web.Mvc;
using ShipperHN.Business;
using ShipperHN.Business.Entities;
using ShipperHN.Business.HEAD;
using Newtonsoft.Json.Linq;
using ShipperHN.Business.LOG;

namespace ShipperHN.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly PostBusiness _postBusiness;
        private readonly LogControl _logControl;
        private readonly ShipperHNDBcontext _shipperHndBcontext;
        private readonly PhoneNumberBusiness _phoneNumberBusiness;

        public PostController()
        {
            _shipperHndBcontext = new ShipperHNDBcontext();
            _logControl = new LogControl();
            _postBusiness = new PostBusiness(_shipperHndBcontext);
            _phoneNumberBusiness = new PhoneNumberBusiness(_shipperHndBcontext);
        }

        private DateTime DateTimeConverter(string input)
        {
            try
            {
                return DateTime.Parse(input.Substring(0, 24));
            }
            catch (Exception)
            {
                return DateTime.Parse(input);
            }
        }

        [HttpGet]
        public ActionResult GetMorePosts(string bottomTime, string lastPostId)
        {
            List<Post> posts = _postBusiness.GetMorePosts(DateTimeConverter(bottomTime), lastPostId);
            return PartialView("~/Views/_PostList.cshtml", posts);
        }

        [HttpGet]
        public string UpdateBottomStatus(string bottomTime, string lastPostId)
        {
            return _postBusiness.UpdateBottomStatus(DateTimeConverter(bottomTime), lastPostId);
        }

        [HttpGet]
        public ActionResult GetNewPosts(string topTime, string firstPostId)
        {
            List<Post> posts = _postBusiness.GetNewPosts(DateTimeConverter(topTime), firstPostId);
            return PartialView("~/Views/_PostList.cshtml", posts);
        }

        [HttpGet]
        public string UpdateTopStatus(string topTime, string firstPostId)
        {
            return _postBusiness.UpdateTopStatus(DateTimeConverter(topTime), firstPostId);
        }

        [HttpGet]
        public ActionResult GetNewLocationPostCount(String[] locationsDatimes)
        {
            DateTime[] dateTimes = new DateTime[13];
            for (int i = 0; i < locationsDatimes.Length; i++)
            {
                dateTimes[i] = DateTimeConverter(locationsDatimes[i]);
            }
            string[] result = _postBusiness.GetNewLocationPostCount(dateTimes);
            return PartialView("~/Views/_LocationNav.cshtml", result);
        }

        [HttpGet]
        public ActionResult FilterByLocation(string location)
        {
            List<Post> posts = _postBusiness.FilterByLocation(location);
            return PartialView("~/Views/_PostList.cshtml", posts);
        }

        [HttpGet]
        public string UpdateFilterStatus(string location)
        {
            return _postBusiness.UpdateFilterStatus(location);
        }

        [HttpGet]
        public ActionResult FilterByLocationGetBottom(string bottomTime, string lastPostId, string location)
        {
            List<Post> posts = _postBusiness.FilterByLocationGetBottom(DateTimeConverter(bottomTime), lastPostId, location);
            return PartialView("~/Views/_PostList.cshtml", posts);
        }

        [HttpGet]
        public string UpdateFilterBottomStatus(string bottomTime, string lastPostId, string location)
        {
            return _postBusiness.UpdateFilterBottomStatus(DateTimeConverter(bottomTime), lastPostId, location);
        }

        [HttpGet]
        public ActionResult FilterByLocationGetTop(string topTime, string firstPostId, string location)
        {
            List<Post> posts = _postBusiness.FilterByLocationGetTop(DateTimeConverter(topTime), firstPostId, location);
            return PartialView("~/Views/_PostList.cshtml", posts);
        }

        [HttpGet]
        public string UpdateFilterTopStatus(string topTime, string firstPostId, string location)
        {
            return _postBusiness.UpdateFilterTopStatus(DateTimeConverter(topTime), firstPostId, location);
        }

        [HttpGet]
        public ActionResult GetAllPostsByUserId(string userid)
        {
            List<Post> posts = _postBusiness.GetAllPostsByUserId(userid);
            return PartialView("~/Views/_SearchPostResult.cshtml", posts);
        }

        [HttpPost]
        [ValidateInput(false)]
        public string AddPosts(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return "Error: Empty data!";
            }

            try
            {
                JObject json = JObject.Parse(data);
                foreach (var jToken in json["list_posts"])
                {
                    string user_fullname = jToken["user_fullname"].ToString();
                    string user_picture = jToken["user_picture"].ToString();
                    string user_url = jToken["user_url"].ToString();
                    string user_id = jToken["user_id"].ToString();
                    string message = jToken["message"].ToString();
                    string post_id = jToken["post_id"].ToString();
                    string full_picture = jToken["full_picture"].ToString();

                    if (string.IsNullOrEmpty(user_url)
                        || string.IsNullOrEmpty(message)
                        || string.IsNullOrEmpty(post_id)
                        )
                    {
                        continue;
                    }
                    if (_shipperHndBcontext.Posts.FirstOrDefault(x => x.PostId.Equals(post_id)) == null)
                    {
                        User user;
                        if ((!string.IsNullOrEmpty(user_id) && _shipperHndBcontext.Users.FirstOrDefault(x => x.UserId.Equals(user_id)) == null)
                            || (string.IsNullOrEmpty(user_id) && _shipperHndBcontext.Users.FirstOrDefault(x => x.UserProfileUrl.Equals(user_url)) == null))
                        {
                            user = _shipperHndBcontext.Users.Add(new User
                            {
                                UserId = user_id,
                                UserProfilePicture = user_picture,
                                Name = user_fullname,
                                UserProfileUrl = user_url
                            });
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(user_id))
                            {
                                user = _shipperHndBcontext.Users
                                    .Include(t => t.PhoneNumbers)
                                    .FirstOrDefault(t => t.UserProfileUrl.Equals(user_url));
                            }
                            else
                            {
                                user = _shipperHndBcontext.Users
                                    .Include(t => t.PhoneNumbers)
                                    .FirstOrDefault(t => t.UserId.Equals(user_id));
                            }

                        }

                        List<Location> locations = _postBusiness.DectectLocation(message);

                        List<Match> matches = _phoneNumberBusiness.DetetectPhoneNumber(message);
                        if (matches != null && matches.Count > 0)
                        {
                            foreach (Match match in matches)
                            {
                                _phoneNumberBusiness.GetPhone(match, user);
                            }
                        }

                        List<Location> addLocations = new List<Location>();
                        foreach (var location in locations)
                        {
                            addLocations.Add(_shipperHndBcontext.Locations.Add(location));
                        }

                        try
                        {
                            _shipperHndBcontext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        Post post = new Post
                        {
                            PostId = post_id,
                            Message = message,
                            User = user,
                            Locations = addLocations,
                            CreatedTime = DateTime.Now,
                            FullPicture = full_picture
                        };

                        _shipperHndBcontext.Posts.Add(post);

                        try
                        {
                            _shipperHndBcontext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return "Success";
        }

        public ActionResult Index()
        {
            FetchData fetchData = new FetchData(new ShipperHNDBcontext());
            Thread t = new Thread(fetchData.Run);
            t.Start();
            List<Post> posts = _postBusiness.Top20Post();
            ViewData["posts"] = posts;
            return View();
        }

    }
}