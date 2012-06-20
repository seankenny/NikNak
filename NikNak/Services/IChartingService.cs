using System.Collections.Generic;
using System.IO;

namespace NikNak.Services
{
    public interface IChartingService
    {
        MemoryStream BuildChart(int? type, IDictionary<string, float> dataPoints);
    }
}