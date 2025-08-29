using Microsoft.AspNetCore.Mvc;

namespace Inspinia.Controllers
{
    /// <summary>
    /// Controller for dashboard views and functionality.
    /// </summary>
    public class DashboardsController : Controller
    {
        /// <summary>
        /// Returns the main dashboard view.
        /// </summary>
        /// <returns>The dashboard index view.</returns>
        public IActionResult Index() => View();
    }
}