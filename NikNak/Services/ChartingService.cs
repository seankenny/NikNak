using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;

namespace NikNak.Services
{
    public class ChartingService
    {
        public MemoryStream BuildChart( )
        {
            // slug in some data
            var data = new Dictionary<string, float>
                {
                    { "test", 10.023f },
                    { "test2", 20.020f },
                    { "test3", 19.203f },
                    { "test4", 4.039f },
                    { "test5", 5.343f }
                };


            var chart = new Chart();
            
            // configure your chart area (dimensions, etc) here.
            var area = new ChartArea();
            chart.ChartAreas.Add(area);

            // create and customize your data series.
            var series = new Series();
            foreach (var item in data)
            {
                series.Points.AddXY(item.Key, item.Value);
            }
            series.Label = "#PERCENT{P0}";
            series.Font = new Font("Segoe UI", 8.0f, FontStyle.Bold);
            series.ChartType = SeriesChartType.Pie;
            series["PieLabelStyle"] = "Outside";

            chart.Series.Add(series);

            var returnStream = new MemoryStream();
            chart.ImageType = ChartImageType.Png;
            chart.SaveImage(returnStream);
            returnStream.Position = 0;
            return returnStream;
        }
    }
}