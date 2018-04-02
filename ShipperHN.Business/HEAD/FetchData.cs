using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Configuration;
using Facebook;
using Newtonsoft.Json.Linq;
using ShipperHN.Business.Entities;
using ShipperHN.Business.LOG;

namespace ShipperHN.Business.HEAD
{
    public class FetchData
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _idgroup1;
        private readonly string _idgroup2;
        private readonly PostBusiness _postBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly PhoneNumberBusiness _phoneNumberBusiness;
        private FacebookClient _fc;
        private readonly ShipperHNDBcontext _shipperHndBcontext;
        private readonly LogControl _logControl;

        public FetchData(ShipperHNDBcontext shipperHndBcontext)
        {
            _logControl = new LogControl();
            _clientId = WebConfigurationManager.AppSettings["CLIENT_ID"];
            _clientSecret = WebConfigurationManager.AppSettings["CLIENT_SECRET"];
            _idgroup1 = WebConfigurationManager.AppSettings["IDGROUP1"];
            _idgroup2 = WebConfigurationManager.AppSettings["IDGROUP2"];

            _shipperHndBcontext = shipperHndBcontext;
            _postBusiness = new PostBusiness(_shipperHndBcontext);
            _userBusiness = new UserBusiness(_shipperHndBcontext);
            _phoneNumberBusiness = new PhoneNumberBusiness(_shipperHndBcontext);
        }

        public List<JObject> GetJsonData(string dataFetch)
        {
            List<JObject> jos = new List<JObject>();
            try
            {
                if (dataFetch == "")
                {
                    return jos;
                }
                JObject o = JObject.Parse(dataFetch);
                for (int i = 0; i < o["data"].Count(); i++)
                {
                    if (o["data"][i]["message"] == null && o["data"][i]["from"] != null)
                    {
                        jos.Add(JObject.Parse(o["data"][i].ToString()));
                    }
                    else
                    {
                        var jToken = o["data"][i]["message"];
                        if (jToken != null && (jToken.ToString().Length < 1000 && o["data"][i]["from"] != null))
                        {
                            jos.Add(JObject.Parse(o["data"][i].ToString()));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logControl.AddLog(1, "FetchData.cs/GetJsonData", "Type: " + e.GetType()
                                                                + " | Message: " + e.Message + " | InnerException: " +
                                                                e.InnerException);
                Thread.Sleep(1000);
            }
            return jos;
        }

        //get access token from facebook
        public void GetAccessToken()
        {
            while (true)
            {
                try
                {
                    _fc = new FacebookClient();
                    dynamic result = _fc.Get("oauth/access_token", new
                    {
                        client_id = _clientId,
                        client_secret = _clientSecret,
                        grant_type = "client_credentials"

                    });
                    _fc.AccessToken = result.access_token;
                    break;
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(WebExceptionWrapper))
                    {
                        _logControl.AddLog(0, "GetAccessToken", "Type: " + e.GetType()
                                                                + " | Message: " + e.Message + " | InnerException: " +
                                                                e.InnerException);
                        Thread.Sleep(2000);
                    }
                }
            }
        }

        //set new token to web.config
        public void GetAccessTokenKey()
        {
            string address = "https://developers.facebook.com/tools/explorer/1382538675093138?method=GET&path=me%3Ffields%3Did%2Cname&version=v2.7"
                + _clientId + "|" + _clientSecret;
            using (WebClient client = new WebClient())
            {
                string ressponse = client.DownloadString(address);
                //string ressponse = "\"accessToken\":\"EAATpaV6dZCpIBAIH7gDBEVHov5qZBEai5ZBau1Ka3ibRXzd8C7agPcgFqQRBDZADVdaZCkQHHqENSK56vpQsVjFPPbeZAUNw2A8sexBvEZBjZBVUrqb7XnZC9VP9gV87vyLSeFSm33Lk7x2j2lxlZAyYzFScTcF4tYAEsVf6pSUZApyTLuqdWhoFnphNsZC7zOZAUfcEZD\",\"anonymousTokenAllowed\"";
                Regex regex = new Regex("\"accessToken\":\"(.*)\",\"anonymousTokenAllowed\"");
                Match match = regex.Match(ressponse);
                if (match.Success)
                {
                    WebConfigurationManager.AppSettings["access_token"] = "Pausing";
                }
            }
        }

        public void DoFetch()
        {
            string dataFetch1 = "";
            string dataFetch2 = "";

            //get data (posts - json) from facebook graph 
            try
            {
                if (!string.IsNullOrEmpty(_idgroup1))
                {
                    string address = "https://graph.facebook.com/"
                    + "v2.8/" + _idgroup1
                                 + "/feed?access_token=" + _clientId + "%7C" + _fc.AccessToken.Split('|')[1]
                                 + "&debug=all&fields=id%2Ccreated_time%2Cmessage%2Cfull_picture%2Cfrom&format=json&limit=10&method=get&pretty=0&suppress_http_code=1";
                    using (WebClient client = new WebClient())
                    {
                        dataFetch1 = client.DownloadString(address);
                    }
                }
                if (!string.IsNullOrEmpty(_idgroup2))
                {
                    string address = "https://graph.facebook.com/"
                    + "v2.8/" + _idgroup2
                                 + "/feed?access_token=" + _clientId + "%7C" + _fc.AccessToken.Split('|')[1]
                                 + "&debug=all&fields=id%2Ccreated_time%2Cmessage%2Cfull_picture%2Cfrom&format=json&limit=10&method=get&pretty=0&suppress_http_code=1";
                    using (WebClient client = new WebClient())
                    {
                        dataFetch2 = client.DownloadString(address);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(FacebookOAuthException))
                {
                    _logControl.AddLog(0, "FetchData.cs/GetAccessToken", "Type: " + e.GetType()
                        + " | Message: " + e.Message + " | InnerException: " + e.InnerException);
                }
                else
                {
                    _logControl.AddLog(1, "FetchData.cs/GetAccessToken", "Type: " + e.GetType()
                        + " | Message: " + e.Message + " | InnerException: " + e.InnerException);
                }
            }
            //convert dataFetch to List<json> posts
            List<JObject> jos1 = GetJsonData(dataFetch1);
            List<JObject> jos2 = GetJsonData(dataFetch2);

            List<Post> posts = new List<Post>();

            //data handing
            DataHanding(posts, jos1);
            DataHanding(posts, jos2);

            //remove same same posts
            for (int i = 0; i < posts.Count; i++)
            {
                for (int j = i + 1; j < posts.Count; j++)
                {
                    if (posts[i].Message.Length <= 50 || posts[j].Message.Length <= 50)
                    {
                        if (posts[i].Message.Equals(posts[j].Message))
                        {
                            if (posts[i].CreatedTime > posts[j].CreatedTime)
                            {
                                posts.RemoveAt(i);
                                i--;
                                break;
                            }
                            posts.RemoveAt(j);
                            j--;
                        }
                    }
                    else if (posts[i].Message.Substring(0, 50).Equals(posts[j].Message.Substring(0, 50)))
                    {
                        if (posts[i].CreatedTime > posts[j].CreatedTime)
                        {
                            posts.RemoveAt(i);
                            i--;
                            break;
                        }
                        posts.RemoveAt(j);
                        j--;
                    }
                }
            }

            //Add data to DB
            _postBusiness.AddPosts(posts);
        }

        public void DataHanding(List<Post> posts, List<JObject> jos)
        {
            foreach (JObject jo in jos)
            {
                string idpost = (string)jo["id"];
                string message = (string)jo["message"];

                //detected phone number in message
                List<Match> matches = _phoneNumberBusiness.DetetectPhoneNumber(message);

                string userid = (string)jo["from"]["id"];

                if (CheckMessage(message)                      //phone count > 5 => SIM sellers
                    && !_postBusiness.IsExists(idpost)
                    && userid != null
                    && !idpost.Contains(userid))                    //post of groups?
                {
                    User user = _userBusiness.GetUser(_fc, userid);

                    posts.Add(_postBusiness.GetPost(jo));

                    if (matches != null && matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            _phoneNumberBusiness.GetPhone(match, user);
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

            if (message.Length >= 50 && _shipperHndBcontext.Posts.Any(x => x.Message.Contains(message.Substring(0, 50))))
            {
                return false;
            }
            if (message.Length < 50 && _shipperHndBcontext.Posts.Any(x => x.Message.Contains(message)))
            {
                return false;
            }
            return true;
        }

        public void Run()
        {
            string fetchFlag = WebConfigurationManager.AppSettings["FetchFlag"];
//            if (fetchFlag.Equals("Pausing"))
//            {
//                try
//                {
//                    if (_shipperHndBcontext.Posts.Count() > 10000)
//                    {
//                        List<Post> posts = _shipperHndBcontext.Posts.OrderBy(x => x.CreatedTime)
//                            .Include(x => x.User.PhoneNumbers)
//                            .Include(x => x.User)
//                            .Include(x => x.Comments)
//                            .Take(5000).ToList();
//                        List<User> users = posts.Select(x => x.User).ToList();
//                        List<List<Comment>> commentLists = posts.Select(x => x.Comments).ToList();
//                        List<Comment> comments = new List<Comment>();
//                        foreach (List<Comment> commentList in commentLists)
//                        {
//                            comments.AddRange(commentList);
//                        }
//                        List<List<PhoneNumber>> phoneNumberLists = posts.Select(x => x.User.PhoneNumbers).ToList();
//                        List<PhoneNumber> phoneNumbers = new List<PhoneNumber>();
//                        foreach (List<PhoneNumber> numbers in phoneNumberLists)
//                        {
//                            phoneNumbers.AddRange(numbers);
//                        }
//
//                        _shipperHndBcontext.Comments.RemoveRange(comments);
//                        _shipperHndBcontext.Posts.RemoveRange(posts);
//                        _shipperHndBcontext.PhoneNumbers.RemoveRange(phoneNumbers);
//                        _shipperHndBcontext.Users.RemoveRange(users);
//                        _shipperHndBcontext.SaveChanges();
//                    }
//                }
//                catch (Exception e)
//                {
//                    _logControl.AddLog(1, "FetchData.cs/Run", "Type: " + e.GetType()
//                        + " | Message: " + e.Message + " | InnerException: " + e.InnerException);
//                }
//                // set fetch flag to running
//                WebConfigurationManager.AppSettings["FetchFlag"] = "Running";
//                int count = 0; //count of sessions
//                GetAccessToken();
//                //GetAccessTokenKey();
//                while (count < 5000)
//                {
//                    count++;
//                    try
//                    {
//                        DoFetch();
//                    }
//                    catch (WebExceptionWrapper e)
//                    {
//                        _logControl.AddLog(1, "FetchData.cs/Run", "Type: " + e.GetType()
//                        + " | Message: " + e.Message + " | InnerException: " + e.InnerException);
//                        Thread.Sleep(500);
//                    }
//                    Thread.Sleep(500);
//                }
//
//                // reset fetch flag
//                WebConfigurationManager.AppSettings["FetchFlag"] = "Pausing";
//            }
        }

    }
}

