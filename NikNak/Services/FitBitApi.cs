using System;
using System.Collections.ObjectModel;

using NikNak.ViewModels;

namespace NikNak.Services
{
    public class FitBitApi
    {
        public FitBitDataViewModel GetFitBitData( )
        {
            var viewModelDummy = new FitBitDataViewModel
                {
                    UserName = "test User",
                    Activities = new Collection<ActivityType>
                        {
                            new ActivityType { ActivityTypeName = "Activities", Date = DateTime.Now.AddDays(-10) },
                            new ActivityType { ActivityTypeName = "Activities", Date = DateTime.Now.AddDays(-8) },
                            new ActivityType { ActivityTypeName = "Activities", Date = DateTime.Now.AddDays(-6) },
                            new ActivityType { ActivityTypeName = "Activities", Date = DateTime.Now.AddDays(-4) },
                            new ActivityType { ActivityTypeName = "Activities", Date = DateTime.Now.AddDays(-3) },
                            new ActivityType { ActivityTypeName = "Activities", Date = DateTime.Now.AddDays(-2) },
                            new ActivityType { ActivityTypeName = "Activities", Date = DateTime.Now.AddDays(-1) }
                        }
                };

            return viewModelDummy;
        }
    }
}