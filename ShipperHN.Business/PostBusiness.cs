using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using ShipperHN.Business.Entities;
using ShipperHN.Business.LOG;

namespace ShipperHN.Business
{
    public partial class PostBusiness
    {
        private readonly ShipperHNDBcontext _shipperHNDBcontext;
        private readonly LogControl _logControl;

        public PostBusiness(ShipperHNDBcontext shipperHndBcontext)
        {
            _logControl = new LogControl();
            _shipperHNDBcontext = shipperHndBcontext;
        }

        public void AddPosts(List<Post> posts)
        {
            foreach (Post post in posts)
            {
                try
                {
                    _shipperHNDBcontext.Posts.Add(post);
                    _shipperHNDBcontext.SaveChanges();
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(DbUpdateException))
                    {
                        _logControl.AddLog(0, "PostBusiness.cs/AddPosts", "Type: " + e.GetType()
                                    + " | Message: " + e.Message + " | InnerException: " + e.InnerException);
                        _shipperHNDBcontext.Posts.Remove(post);
                    }
                }
            }
        }

        public Post GetPostById(string PostId)
        {
            Post post = _shipperHNDBcontext.Posts.Where(x => x.PostId.Equals(PostId))
                .Include(x => x.User)
                .Include(x => x.User.PhoneNumbers)
                .Include(x => x.Comments)
                .FirstOrDefault();
            return post;
        }

        public List<Post> Top20Post()
        {
            var posts = _shipperHNDBcontext.Posts
                .Include(x => x.User)
                .Include(x => x.User.PhoneNumbers)
                .OrderByDescending(x => x.InsertTime)
                .Take(20)
                .ToList();
            return posts;
        }

        public List<Post> Top20PostSearchByName(string input)
        {
            List<Post> posts = new List<Post>();
            return posts;
        }

        public bool IsExists(string PostId)
        {
            return _shipperHNDBcontext.Posts.FirstOrDefault(x => x.PostId.Equals(PostId)) != null;
        }

        public Post GetPost(JObject jo)
        {
            var createdTime = DateTime.Parse((string)jo["created_time"]);
            var userId = (string)jo["from"]["id"];
            var message = (string)jo["message"];
            var PostId = (string)jo["id"];
            var fullPicture = jo["full_picture"] != null ? (string)jo["full_picture"] : "";
            Post post = new Post
            {
                CreatedTime = createdTime,
                PostId = PostId,
                Message = message,
                User = _shipperHNDBcontext.Users.FirstOrDefault(x => x.Id.Equals(userId)),
                InsertTime = DateTime.Now,
                Places = DectectLocation(message),
                FullPicture = fullPicture
            };
            return post;
        }

        public string[] GetNewLocationPostCount(DateTime[] dateTimes)
        {
            string[] result = new string[13];
            string[] quans = new Post().GetLocations();
            for (int i = 0; i < quans.Count(); i++)
            {
                string quan = quans[i];
                DateTime tmpDateTime = dateTimes[i];
                int count =
                    _shipperHNDBcontext.Posts.Where(x => x.Places.Contains(quan) && x.InsertTime > tmpDateTime)
                        .ToList().Count;
                result[i] = quans[i] + "@" + count;
            }
            return result;
        }

        public List<Post> GetNewPosts(DateTime topTime, string firstPostId)
        {
            List<Post> posts = _shipperHNDBcontext.Posts
                .Where(x => x.InsertTime >= topTime)
                .Include(x => x.User)
                .Include(x => x.User.PhoneNumbers)
                .OrderByDescending(x => x.InsertTime).ToList();

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].PostId.Equals(firstPostId))
                {
                    posts.RemoveRange(i, posts.Count - i);
                    break;
                }
            }
            return posts;
        }

        public string UpdateTopStatus(DateTime topTime, string firstPostId)
        {
            string result;
            List<Post> posts = _shipperHNDBcontext.Posts
                .Where(x => x.InsertTime >= topTime)
                .Include(x => x.User)
                .Include(x => x.User.PhoneNumbers)
                .OrderByDescending(x => x.InsertTime).ToList();

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].PostId.Equals(firstPostId))
                {
                    posts.RemoveRange(i, posts.Count - i);
                    break;
                }
            }
            if (posts.Count > 0)
            {
                result = posts[posts.Count - 1].InsertTime + "@" + posts[0].PostId;
            }
            else
            {
                result = topTime + "@" + firstPostId;
            }
            return result;
        }

        public List<Post> GetMorePosts(DateTime bottomTime, string lastPostId)
        {
            List<Post> posts = _shipperHNDBcontext.Posts
                .Where(x => x.InsertTime <= bottomTime)
                .OrderByDescending(x => x.InsertTime)
                .Include(x => x.User)
                .Include(x => x.User.PhoneNumbers)
                .Take(20).ToList();

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].PostId.Equals(lastPostId))
                {
                    posts.RemoveRange(0, i);
                    break;
                }
            }
            return posts;
        }

        public string UpdateBottomStatus(DateTime bottomTime, string lastPostId)
        {
            string result;
            List<Post> posts = _shipperHNDBcontext.Posts
                .Where(x => x.InsertTime <= bottomTime)
                .OrderByDescending(x => x.InsertTime)
                .Include(x => x.User)
                .Include(x => x.User.PhoneNumbers)
                .Take(20).ToList();

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].PostId.Equals(lastPostId))
                {
                    posts.RemoveRange(0, i);
                    break;
                }
            }
            if (posts.Count > 0)
            {
                result = posts[posts.Count - 1].InsertTime + "@" + posts[posts.Count - 1].PostId + "@false";
            }
            else
            {
                result = bottomTime + "@" + lastPostId + "@true";
            }
            return result;
        }

        //Filter
        public List<Post> FilterByLocation(string location)
        {
            List<Post> posts = _shipperHNDBcontext.Posts
                .Where(x => x.Places.Contains(location))
                .Include(x => x.User)
                .Include(x => x.User.PhoneNumbers)
                .OrderByDescending(x => x.InsertTime)
                .Take(20).ToList();
            return posts;
        }

        public string UpdateFilterStatus(string location)
        {
            string result;
            List<Post> posts = _shipperHNDBcontext.Posts
                .Where(x => x.Places.Contains(location))
                .OrderByDescending(x => x.InsertTime)
                .Take(20).ToList();
            if (posts.Count >= 20)
            {
                result = posts[posts.Count - 1].PostId + "@false";
            }
            else
            {
                result = "x@true";
            }
            return result;
        }

        public List<Post> FilterByLocationGetBottom(DateTime bottomTime, string lastPostId, string location)
        {
            List<Post> posts = _shipperHNDBcontext.Posts
                .Where(x => x.InsertTime <= bottomTime && x.Places.Contains(location))
                .OrderByDescending(x => x.InsertTime)
                .Include(x => x.User)
                .Include(x => x.User.PhoneNumbers)
                .Take(20).ToList();

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].PostId.Equals(lastPostId))
                {
                    posts.RemoveRange(0, i);
                    break;
                }
            }
            return posts;
        }

        public string UpdateFilterBottomStatus(DateTime bottomTime, string lastPostId, string location)
        {
            string result;
            List<Post> posts = _shipperHNDBcontext.Posts
                .Where(x => x.InsertTime <= bottomTime && x.Places.Contains(location))
                .OrderByDescending(x => x.InsertTime)
                .Take(20).ToList();

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].PostId.Equals(lastPostId))
                {
                    posts.RemoveRange(0, i);
                    break;
                }
            }
            if (posts.Count > 0)
            {
                result = posts[posts.Count - 1].InsertTime + "@" + posts[posts.Count - 1].PostId + "@false";
            }
            else
            {
                result = bottomTime + "@" + lastPostId + "@true";
            }
            return result;
        }

        public List<Post> FilterByLocationGetTop(DateTime topTime, string firstPostId, string location)
        {
            List<Post> posts = _shipperHNDBcontext.Posts
                .Where(x => x.InsertTime >= topTime && x.Places.Contains(location))
                .Include(x => x.User)
                .Include(x => x.User.PhoneNumbers)
                .OrderByDescending(x => x.InsertTime).ToList();

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].PostId.Equals(firstPostId))
                {
                    posts.RemoveRange(i, posts.Count - i);
                    break;
                }
            }
            return posts;
        }

        public string UpdateFilterTopStatus(DateTime topTime, string firstPostId, string location)
        {
            string result;
            List<Post> posts = _shipperHNDBcontext.Posts
                .Where(x => x.InsertTime >= topTime && x.Places.Contains(location))
                .OrderByDescending(x => x.InsertTime).ToList();

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].PostId.Equals(firstPostId))
                {
                    posts.RemoveRange(i, posts.Count - i);
                    break;
                }
            }
            if (posts.Count > 0)
            {
                result = posts[posts.Count - 1].InsertTime + "@" + posts[0].PostId;
            }
            else
            {
                result = topTime + "@" + firstPostId;
            }
            return result;
        }

        public List<Post> GetAllPostsByUserId(string userId)
        {
            List<Post> posts = new List<Post>();
            posts = _shipperHNDBcontext.Posts.Where(x => x.User.Id.Equals(userId))
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedTime)
                .ToList();
            return posts;
        }

        public void AddPosts(string[] listPostIds, string[] listIdUsers, string[] listUserNames, string[] listUserPictures,
            string[] listMessages, DateTime[] listCreatedTimes)
        {
            List<Post> posts = new List<Post>();
            DataHanding(posts, listPostIds, listIdUsers, listUserNames, listUserPictures, listMessages, listCreatedTimes);
            AddPosts(posts);
        }

        public void DataHanding(List<Post> posts, string[] listPostIds, string[] listIdUsers, string[] listUserNames, string[] listUserPictures,
            string[] listMessages, DateTime[] listCreatedTimes)
        {
            PhoneNumberBusiness phoneNumberBusiness = new PhoneNumberBusiness(_shipperHNDBcontext);
            for (int i = 0; i < listPostIds.Count(); i++)
            {
                string message = listMessages[i];

                //detected phone number in message
                List<Match> matches = phoneNumberBusiness.DetetectPhoneNumber(message);

                string userid = listIdUsers[i];

                if (CheckMessage(message)                   //phone count > 5 => SIM sellers
                    && !IsExists(listPostIds[i])
                    && !listPostIds[i].Contains(userid))    //post of groups?
                {
                    User user;
                    if (_shipperHNDBcontext.Users.FirstOrDefault(x => x.Id.Equals(userid)) == null)
                    {
                        user = new User
                        {
                            UserId = userid,
                            Name = listUserNames[i],
                            LatestViewNotificationTime = DateTime.Now
                        };
                        _shipperHNDBcontext.Users.Add(user);
                        try
                        {
                            _shipperHNDBcontext.SaveChanges();
                        }
                        catch (Exception)
                        {
                            _shipperHNDBcontext.Users.Remove(user);
                            _shipperHNDBcontext.SaveChanges();
                        }
                    }
                    else
                    {
                        user = _shipperHNDBcontext.Users.FirstOrDefault(x => x.Id.Equals(userid));
                    }

                    Post post = new Post
                    {
                        User = user,
                        InsertTime = DateTime.Now,
                        Places = DectectLocation(message),
                        PostId = listPostIds[i],
                        CreatedTime = listCreatedTimes[i],
                        Message = listMessages[i]
                    };

                    posts.Add(post);

                    if (matches != null && matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            phoneNumberBusiness.GetPhone(match, user);
                        }
                    }
                }
            }
            
        }

        public bool CheckMessage(string message)
        {
            if (message == null
                || message.Length > 1000
                || message.Contains("#Nokia1280_110i")
                || message.Contains("https://www.youtube.com/")
                || message.Contains("THUỐC ĐẶC TRỊ : DẠ DÀY - VIÊM LOÉT DẠ DÀY - DỨT ĐIỂM - LÂU DÀI <3 <3")
                || message.Contains("http://adf.ly/")
                || message.Contains("!!!! NÁM - TÀN NHANG Ư...CHUYỆN NHỎ !!!")
                || message.Contains("MUA 1 TẶNG 1 - Sale 60K combo túi ví")
                || message.Contains("[NGÀY VÀNG - GIẢM GIÁ LỚN]")
                || message.Contains("!! AI BẢO NAM GIỚI LÀ KO ĐC LÀM ĐẸP !!!")
                || message.Contains("https://www.facebook.com/")
                || message.Contains("TUYỂN GẤP NHÂN VIÊN")
                || message.Contains("tuyển gấp nhân viên")
                || message.Contains("Nokia1280_110i")
                || message.Length < 30
                )
            {
                return false;
            }

            if (message.Length >= 50 && _shipperHNDBcontext.Posts.Any(x => x.Message.Contains(message.Substring(0, 50))))
            {
                return false;
            }
            if (message.Length < 50 && _shipperHNDBcontext.Posts.Any(x => x.Message.Contains(message)))
            {
                return false;
            }
            return true;
        }

    }
}

