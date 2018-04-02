using System.Web.Optimization;

namespace ShipperHN.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/js/jquery.min.js",
                        "~/js/jquery.cookie.js",
                        "~/js/home.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/style.css",
                      "~/Content/Responsive.css",
                      "~/Content/loading-effect.css"
                      ));
        }
    }
}
