using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using ShipperHN.Business;
using ShipperHN.Business.Entities;
using ShipperHN.Business.HEAD;
using Newtonsoft.Json;
using ShipperHN.Business.LOG;

namespace ShipperHN.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly PostBusiness _postBusiness;
        private readonly LogControl _logControl;

        public PostController()
        {
            _logControl = new LogControl();
            var shipperHndBcontext = new ShipperHNDBcontext();
            _postBusiness = new PostBusiness(shipperHndBcontext);
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
        public void AddPosts(string data)
        {
            if(string.IsNullOrEmpty(data))
            {
                return;
            }
            int count = int.Parse(data.Split(new[] { "#COUNT#"}, StringSplitOptions.None)[0]);
            string posts = data.Split(new[] { "#COUNT#" }, StringSplitOptions.None)[1];
            string [] listIdPosts = new string[count];
            string [] listUserNames = new string[count];
            string [] listUserPictures = new string[count];
            string [] listMessages = new string[count];
            string [] listIdUsers = new string[count];
            DateTime [] listCreatedTimes = new DateTime[count];
            string[] tmp = posts.Split(new[] {"@@##@@"}, StringSplitOptions.None);
            int k = 0;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    listUserNames[i] = tmp[k++];
                    listIdPosts[i] = tmp[k++];
                    listIdUsers[i] = tmp[k++];
                    listUserPictures[i] = tmp[k++];
                    listMessages[i] = tmp[k++];
                    listCreatedTimes[i] = DateTime.Parse(tmp[k++]);
                }
                catch (Exception e)
                {
                    _logControl.AddLog(1, "PostController.cs/AddPosts", "Type: " + e.GetType()
                                   + " | Message: " + e.Message + " | InnerException: " + e.InnerException);
                }
               
            }
            _postBusiness.AddPosts(listIdPosts, listIdUsers, listUserNames, listUserPictures, listMessages, listCreatedTimes);
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