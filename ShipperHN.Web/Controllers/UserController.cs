using System.Collections.Generic;
using System.Web.Mvc;
using ShipperHN.Business;
using ShipperHN.Business.Entities;

namespace ShipperHN.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserBusiness _userBusiness;

        public UserController()
        {
            var shipperHndBcontext = new ShipperHNDBcontext();
            _userBusiness = new UserBusiness(shipperHndBcontext);
        }

        // GET: User
        [HttpGet]
        public ActionResult SearchUserByName(string name)
        {
            List<User> users = _userBusiness.SearchUserByName(name); 
            return PartialView("~/Views/_SearchResult.cshtml", users);
        }

        [HttpGet]
        public ActionResult SearchUserByPhone(string phone)
        {
            List<User> users = _userBusiness.SearchUserByPhone(phone);
            return PartialView("~/Views/_SearchResult.cshtml", users);
        }

    }
}