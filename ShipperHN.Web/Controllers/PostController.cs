using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using ShipperHN.Business;
using ShipperHN.Business.Entities;
using ShipperHN.Business.HEAD;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShipperHN.Business.LOG;

namespace ShipperHN.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly PostBusiness _postBusiness;
        private readonly LogControl _logControl;
        private readonly ShipperHNDBcontext _shipperHndBcontext;

        public PostController()
        {
            _shipperHndBcontext = new ShipperHNDBcontext();
            _logControl = new LogControl();
            _postBusiness = new PostBusiness(_shipperHndBcontext);
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
        public ActionResult GetPost(string idpost)
        {
            Post post = _postBusiness.GetPostById(idpost);
            return PartialView("~/Views/_PostPreview.cshtml", post);
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
            if(string.IsNullOrEmpty(data))
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

                    if (string.IsNullOrEmpty(user_url)
                        || string.IsNullOrEmpty(message)
                        || string.IsNullOrEmpty(post_id)
                        )
                    {
                        continue;
                    }
                    if (_shipperHndBcontext.Posts.FirstOrDefault(x => x.PostId.Equals(post_id)) == null)
                    {
                        User user = null;
                        if (_shipperHndBcontext.Users.FirstOrDefault(x => x.UserId.Equals(user_id)) == null)
                        {
                            user = _shipperHndBcontext.Users.Add(new User()
                            {
                                UserId = user_id,
                                UserProfilePicture = user_picture,
                                Name = user_fullname,
                                UserProfileUrl = user_url
                            });

                        }
                        else
                        {
                            user = _shipperHndBcontext.Users.FirstOrDefault(x => x.UserId.Equals(user_id));
                        }

//                        String locations = _postBusiness.DectectLocation(message);
//
//                        string[] split = locations.Split(',');
                        

                        Post post = new Post()
                        {
                            PostId = post_id,
                            Message = message,
                            User = user,
                            CreatedTime = DateTime.Now
                        };

                        _shipperHndBcontext.Posts.Add(post);

                        _shipperHndBcontext.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            
            return "Success";
        }

        [HttpPost]
        public string TestLoginAngular(string email, string password, bool isRemember)
        {
            TestLoginAngularClass testLoginAngularClass = new TestLoginAngularClass
            {
                StatusCode = "FAILED",
            };
            if (email.Contains("admin") || email.Contains("guess"))
            {
                testLoginAngularClass.StatusCode = "SUCCESS";
                testLoginAngularClass.State = "/dashboard-project";
                testLoginAngularClass.TokenKey = "TokenKey";
            }
            return JsonConvert.SerializeObject(testLoginAngularClass);
        }

        public class TestLoginAngularClass
        {
            public string StatusCode { get; set; }
            public string TokenKey { get; set; }
            public string State { get; set; }
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