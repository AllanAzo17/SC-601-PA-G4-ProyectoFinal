using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ProyectoFinal.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            System.Diagnostics.Debug.WriteLine("Aplicación MVC iniciada correctamente");
        }

        protected void Application_End()
        {
            System.Diagnostics.Debug.WriteLine("Aplicación MVC finalizada");
        }
    }
}
