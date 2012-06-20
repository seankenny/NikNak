using System.Collections.Generic;
using System.Web.Mvc;

using NikNak.Services;

namespace NikNak.Controllers
{
    public class DataController : Controller
    {
        private IFitBitApi fitbitService;
        private IChartingService chartingService;
        private IBloodMonitorService bloodMonitorService;

        public DataController(IFitBitApi fitbitService, IChartingService chartingService, IBloodMonitorService bloodMonitorService)
        {
            this.fitbitService = fitbitService;
            this.chartingService = chartingService;
            this.bloodMonitorService = bloodMonitorService;
        }

        // display some stuff
        public ActionResult Index()
        {
            // get the fitbit feed
            var viewModel = fitbitService.GetFitBitData();

            return View(viewModel);
        }

        // MS charting...
        public FileStreamResult GetChart(int? type)
        {
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

            var chart = chartingService.BuildChart(type, data);

            return new FileStreamResult(chart, "image/png");
        }

        // raphael
        public string GetChartData()
        {
            var data = bloodMonitorService.GetData();
            return string.Join(",", data);
        }
    }
}
