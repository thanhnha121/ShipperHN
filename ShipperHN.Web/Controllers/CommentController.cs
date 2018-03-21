using System.Web.Mvc;
using ShipperHN.Business;

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
    }
}