using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalAPI.ActivityManager.Library
{
    public enum WorkLevel
    {
        Intern = 1,
        Junior,
        Senior,
        Manager,
        CEO,
    }

    public enum ActivityType
    {
        Service,
        Project,
        Mockup,
        POC,
        Analysis
    }

    public static class Data
    {
        public static List<Activity> Activities = new List<Activity>();
    }
}
