using System.Collections.Generic;
using System.Web.Mvc;
using ShipperHN.Business;
using ShipperHN.Business.Entities;

namespace ShipperHN.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly CommentBusiness _commentBusiness;

        public CommentController()
        {
            ShipperHNDBcontext shipperHndBcontext = new ShipperHNDBcontext();
            _commentBusiness = new CommentBusiness(shipperHndBcontext);
        }

        [HttpGet]
        public ActionResult GetTop20CommentsFB(string idpost)
        {
            List<Comment> comments = _commentBusiness.GetTop20CommentsFB(idpost);
            return PartialView("~/Views/_Comments.cshtml", comments);
        }
    }
}