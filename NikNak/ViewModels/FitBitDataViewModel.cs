using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NikNak.ViewModels
{
    public class FitBitDataViewModel
    {
        public string UserName { get; set; }

        public ICollection<ActivityType> Activities { get; set; }
    }

    public class ActivityType
    {
        public DateTime Date { get; set; }

        public string ActivityTypeName { get; set; }
    }
}