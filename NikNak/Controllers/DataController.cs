using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.Mvc;
using Dapper;
using NikNak.Services;


namespace NikNak.Controllers
{
    using System.Configuration;
    using System.Data.SqlServerCe;

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

        [HttpGet]
        public JsonResult Data(string data)
        {
            //var dbFactory = new OrmLiteConnectionFactory(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\App_Data\Database1.mdf;Integrated Security=True;User Instance=True");

            //Use in-memory Sqlite DB instead
            //var dbFactory = new OrmLiteConnectionFactory(
            //    ":memory:", false, SqliteOrmLiteDialectProvider.Instance);

            //Non-intrusive: All extension methods hang off System.Data.* interfaces
            //IDbConnection dbConn = dbFactory.OpenDbConnection();
            //IDbCommand dbCmd = dbConn.CreateCommand();

            //dbCmd.CreateTable<BloodGlucoseReading>();

            using (var connection = ConnectionFactory.GetOpenConnection())
            {
                connection.Execute(@"insert into BloodGlucoseReadings(ReadingDateTime, Value) values (@ReadingDate, @Value);",
                  new { ReadingDate = DateTime.UtcNow, Value = 9.8M });
                connection.Execute(@"insert into BloodGlucoseReadings(ReadingDateTime, Value) values (@ReadingDate, @Value);",
                  new { ReadingDate = DateTime.UtcNow.AddDays(-1), Value = 4.8M });
                connection.Execute(@"insert into BloodGlucoseReadings(ReadingDateTime, Value) values (@ReadingDate, @Value);",
                  new { ReadingDate = DateTime.UtcNow.AddDays(-2), Value = 5.8M });
                connection.Execute(@"insert into BloodGlucoseReadings(ReadingDateTime, Value) values (@ReadingDate, @Value);",
                  new { ReadingDate = DateTime.UtcNow.AddDays(-3), Value = 7.8M });
            }

            return new JsonResult();
        }
    }

    public class ConnectionFactory
    {
        public static DbConnection GetOpenConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DatabaseService"].ConnectionString;
            var connection = new SqlCeConnection(connectionString);
                //new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\App_Data\Database1.sdf;Integrated Security=True;User Instance=True");
            connection.Open();

            return connection;
        }
    }

}
