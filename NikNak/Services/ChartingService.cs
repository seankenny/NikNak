using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;

namespace NikNak.Services
{
    public class ChartingService : IChartingService
    {
        public MemoryStream BuildChart(int? type, IDictionary<string, float> dataPoints)
        {
            // default to line
            var chartType = type == null ? SeriesChartType.Line : (SeriesChartType)type;

            var chart = new Chart();
            
            // configure your chart area (dimensions, etc) here.
            var area = new ChartArea();
            chart.ChartAreas.Add(area);
            TickMark tm = new TickMark();
           
            // create and customize your data series.
            var series = new Series();
            foreach (var item in dataPoints)
            {
                series.Points.AddXY(item.Key, item.Value);
            }
            
            //series.Label = "#PERCENT{P0}";
            series.Font = new Font("Segoe UI", 8.0f, FontStyle.Bold);
            series.ChartType = chartType;
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