using System.Collections.Generic;

namespace NikNak.Services
{
    public interface IBloodMonitorService
    {
        ICollection<double> GetData();
    }
}