using System.Collections.Generic;
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

        public FileStreamResult GetChart(int? type)
        {
            var service = new Services.ChartingService();

            // slug in some data
            var data = new Dictionary<string, float>
                {
                    { "09:00", 1 },
                    { "10:00", 1.1f },
                    { "11:00", 1.2f },
                    { "12:00", 1.9f },
                    { "13:00", 2.3f },
                    { "14:00", 5.343f },
                    { "15:00", 5.4f },
                    { "16:00", 2.343f },
                    { "17:00", 3.343f },
                    { "18:00", 3.343f },
                    { "19:00", 4.343f },
                    { "20:00", 5.343f },
                    { "21:00", 6.343f },
                    { "22:00", 5.343f },
                    { "23:00", 4.343f }

                };

            var chart = service.BuildChart(type, data);

            return new FileStreamResult(chart, "image/png");
        }

    }
}
