using System.Web.Mvc;

namespace NikNak.Controllers
{
    public class DataController : Controller
    {
        //
        // GET: /Data/

        public ActionResult Index()
        {
            // get the fitbit feed
            var service = new Services.FitBitApi();

            var viewModel = service.GetFitBitData();

            return View(viewModel);
        }

        public FileStreamResult GetChart( )
        {
            var service = new Services.ChartingService();

            var chart = service.BuildChart();
            return new FileStreamResult(chart, "image/png");
        }

    }
}
