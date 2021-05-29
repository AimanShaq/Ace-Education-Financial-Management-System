using System.Web;
using System.Web.Mvc;

namespace Ace_Tuition_WBL
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
