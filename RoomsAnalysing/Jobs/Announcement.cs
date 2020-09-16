
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Accord.Math;
using Quartz;
using RoomsAnalysing.Controllers;
using RoomsAnalysing.Models;
namespace RoomsAnalysing.Jobs
{
    public class Announcement : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            lock (HomeController.NotificationList)
            {
                HomeController.NotificationList.AllNotifications();
            }
        }
    }
}