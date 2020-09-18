
using System.Threading.Tasks;
using Quartz;
using RoomsAnalysing.Controllers;
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