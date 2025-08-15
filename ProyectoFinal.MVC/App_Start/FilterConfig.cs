using System.Web;
using System.Web.Mvc;

namespace ProyectoFinal.MVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute()); // Requiere autenticación para todas las rutas
        }
    }
}
