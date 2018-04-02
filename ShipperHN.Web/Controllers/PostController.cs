using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Web.Mvc;
using ShipperHN.Business;
using ShipperHN.Business.Entities;
using Newtonsoft.Json.Linq;
using ShipperHN.Business.LOG;

namespace ShipperHN.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly PostBusiness _postBusiness;
        public LogControl Control { get; }
        private readonly ShipperHNDBcontext _shipperHndBcontext;
        private readonly PhoneNumberBusiness _phoneNumberBusiness;
        private LogControl _logControl;

        public PostController()
        {
            _shipperHndBcontext = new ShipperHNDBcontext();
            Control = new LogControl();
            _postBusiness = new PostBusiness(_shipperHndBcontext);
            _phoneNumberBusiness = new PhoneNumberBusiness(_shipperHndBcontext);
            _logControl = new LogControl();
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

        [HttpPost]
        public ActionResult LoadMorePost(string lastTime, string location, int[] listIds)
        {

            List<Post> posts = new List<Post>();
            if (lastTime == null || listIds == null)
            {
                return PartialView("~/Views/_PostList.cshtml", posts);
            }

            DateTime time = DateTime.Parse(lastTime);

            if (!string.IsNullOrEmpty(location))
            {
                posts = _shipperHndBcontext.Posts
                    .Include(x => x.User)
                    .Include(x => x.Locations)
                    .Include(x => x.User.PhoneNumbers)
                    .Where(x => x.Locations.Any(y => y.Name.Contains(location) || y.Streets.Contains(location))
                        && !listIds.Contains(x.Id) && x.InsertTime <= time)
                    .OrderByDescending(x => x.InsertTime)
                    .Take(20)
                    .ToList();
            }
            else
            {
                posts = _shipperHndBcontext.Posts
                    .Include(x => x.User)
                    .Include(x => x.Locations)
                    .Include(x => x.User.PhoneNumbers)
                    .Where(x => !listIds.Contains(x.Id) && x.InsertTime <= time)
                    .OrderByDescending(x => x.InsertTime)
                    .Take(20)
                    .ToList();
            }
            return PartialView("~/Views/_PostList.cshtml", posts);
        }

        [HttpPost]
        public ActionResult LoadPrePost(string topTime, string location, int[] listIds)
        {
            List<Post> posts = new List<Post>();
            if (topTime == null || listIds == null)
            {
                return PartialView("~/Views/_PostListLoadTop.cshtml", posts);
            }
            DateTime time = DateTime.Parse(topTime);
            if (!string.IsNullOrEmpty(location))
            {
                posts = _shipperHndBcontext.Posts
                    .Include(x => x.User)
                    .Include(x => x.Locations)
                    .Include(x => x.User.PhoneNumbers)
                    .Where(x => x.Locations.Any(y => y.Name.Contains(location) || y.Streets.Contains(location))
                        && !listIds.Contains(x.Id) && x.InsertTime >= time)
                    .OrderByDescending(x => x.InsertTime)
                    .Take(20)
                    .ToList();
            }
            else
            {
                posts = _shipperHndBcontext.Posts
                    .Include(x => x.User)
                    .Include(x => x.Locations)
                    .Include(x => x.User.PhoneNumbers)
                    .Where(x => !listIds.Contains(x.Id) && x.InsertTime >= time)
                    .OrderByDescending(x => x.InsertTime)
                    .Take(20)
                    .ToList();
            }
            ViewData["posts"] = posts;
            return PartialView("~/Views/_PostListLoadTop.cshtml", posts);
        }

        [HttpPost]
        public ActionResult Search(string input)
        {
            List<Post> posts;
            if (input.StartsWith("@"))
            {
                string searchInput = input.Substring(1, input.Length - 1);
                posts = _shipperHndBcontext.Posts
                    .Include(x => x.User)
                    .Include(x => x.Locations)
                    .Include(x => x.User.PhoneNumbers)
                    .Where(x => x.User.PhoneNumbers.Any(y => y.Phone.Contains(searchInput)))
                    .OrderByDescending(x => x.InsertTime)
                    .Take(20)
                    .ToList();
            }
            else if (input.StartsWith("#"))
            {
                string searchInput = input.Substring(1, input.Length - 1);
                posts = _shipperHndBcontext.Posts
                    .Include(x => x.User)
                    .Include(x => x.Locations)
                    .Include(x => x.User.PhoneNumbers)
                    .Where(x => x.User.Name.Contains(searchInput))
                    .OrderByDescending(x => x.InsertTime)
                    .Take(20)
                    .ToList();
            }
            else
            {
                string searchInput = input.ToLower();
                posts = _shipperHndBcontext.Posts
                    .Include(x => x.User)
                    .Include(x => x.Locations)
                    .Include(x => x.User.PhoneNumbers)
                    .Where(x => x.Message.ToLower().Contains(searchInput)
                        || x.PostId.Contains(searchInput))
                    .OrderByDescending(x => x.InsertTime)
                    .Take(20)
                    .ToList();
            }
            return PartialView("~/Views/_SearchResult.cshtml", posts);
        }

        [HttpPost]
        public string LoadNoti(string[] districts)
        {
            string result = "";
            if (districts.Length == 0)
            {
                return result;
            }

            DateTime[] input = new DateTime[districts.Length];
            for (int i = 0; i < districts.Length; i++)
            {
                input[i] = DateTime.Parse(districts[i]);
            }
            string connectionString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

            using (var conn = new SqlConnection(connectionString))
            using (var command = new SqlCommand("GetLocationNoti", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                conn.Open();

                command.Parameters.Add("@badinhdate", SqlDbType.DateTime).Value = input[0];
                command.Parameters.Add("@hoankiemdate", SqlDbType.DateTime).Value = input[1];
                command.Parameters.Add("@tayhodate", SqlDbType.DateTime).Value = input[2];
                command.Parameters.Add("@longbiendate", SqlDbType.DateTime).Value = input[3];
                command.Parameters.Add("@caugiaydate", SqlDbType.DateTime).Value = input[4];
                command.Parameters.Add("@dongdadate", SqlDbType.DateTime).Value = input[5];
                command.Parameters.Add("@haibatrungdate", SqlDbType.DateTime).Value = input[6];
                command.Parameters.Add("@hoangmaidate", SqlDbType.DateTime).Value = input[7];
                command.Parameters.Add("@thanhxuandate", SqlDbType.DateTime).Value = input[8];
                command.Parameters.Add("@hadongdate", SqlDbType.DateTime).Value = input[9];
                command.Parameters.Add("@bactuliemdate", SqlDbType.DateTime).Value = input[10];
                command.Parameters.Add("@namtuliemdate", SqlDbType.DateTime).Value = input[11];
                command.Parameters.Add("@ngoaithanhdate", SqlDbType.DateTime).Value = input[12];

                var dataReader = command.ExecuteReader();
                int index = 0;

                while (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        result += dataReader.GetInt32(0);
                        if (index != districts.Length - 1)
                        {
                            result += ",";
                        }
                        index++;
                    }
                    dataReader.NextResult();
                }

                dataReader.Close();
                conn.Close();
            }

            return result;
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
                    string check = message.Length >= 50 ? message.Substring(0, 50) : message;
                    if (_shipperHndBcontext.Posts.FirstOrDefault(x => x.PostId.Equals(post_id)) == null
                        && _shipperHndBcontext.Posts.FirstOrDefault(x => x.Message.Contains(check)) == null)
                    {
                        User user;
                        if (!string.IsNullOrEmpty(user_id) && _shipperHndBcontext.Users.FirstOrDefault(x => x.UserId.Equals(user_id)) == null
                            || string.IsNullOrEmpty(user_id) && _shipperHndBcontext.Users.FirstOrDefault(x => x.UserProfileUrl.Equals(user_url)) == null)
                        {
                            user = _shipperHndBcontext.Users.Add(new User
                            {
                                UserId = user_id,
                                UserProfilePicture = user_picture,
                                Name = user_fullname,
                                UserProfileUrl = user_url
                            });
                            try
                            {
                                _shipperHndBcontext.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                _logControl.AddLog(1, "AddPosts", ex.Message);
                                // ignored
                            }
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
                        catch (Exception exception)
                        {
                            _logControl.AddLog(1, "AddPosts", exception.Message);
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
                        catch (Exception e)
                        {
                            _logControl.AddLog(1, "AddPosts", e.Message);
                            // ignored
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logControl.AddLog(1, "AddPosts", e.Message);
                return e.Message;
            }

            return "Success";
        }

        public ActionResult Index(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                List<Post> posts = _shipperHndBcontext.Posts
                    .Include(x => x.User)
                    .Include(x => x.Locations)
                    .Include(x => x.User.PhoneNumbers)
                    .OrderByDescending(x => x.InsertTime)
                    .Take(7)
                    .ToList();
                ViewData["location"] = string.Empty;
                ViewData["posts"] = posts;
                return View();
            }
            else
            {
                string localtion = url.Replace("-", " ").ToLower();
                List<Post> posts = _shipperHndBcontext.Posts
                    .Where(x => x.Locations.Any(y => y.Name.ToLower().Contains(localtion)
                        || y.Streets.ToLower().Contains(localtion)))
                    .Include(x => x.User)
                    .Include(x => x.Locations)
                    .Include(x => x.User.PhoneNumbers)
                    .OrderByDescending(x => x.InsertTime)
                    .Take(7)
                    .ToList();
                ViewData["posts"] = posts;
                ViewData["location"] = localtion;
                return View();
            }
        }

    }
}