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
    public class PageSearching : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            
            HomeController.Search_data("https://www.avito.ru/moskva/kvartiry/prodam-ASgBAgICAUSSA8YQ?s=104&user=2&f=ASgBAQICAUSSA8YQAUDKCLT~WIRZgFmCWYpZmqwBmKwBlqwBlKwBiFmGWQ&p=", "-",1);
            // HomeController.searchDop();
           

                foreach (Room room in HomeController.BufferRoom)
                {
                    IEnumerable<Room> a = HomeController.repo.List().Where(x => x.id == room.id);
               
                if (a.Count() > 0)
                {
                    HomeController.repo.Remove(a.FirstOrDefault());
                }
                room.prediction = HomeController.CalculatingPrice(room);
                if (HomeController.NotificationList != null)
                    HomeController.NotificationList.SetUpdate(room);
                HomeController.repo.Save(room);
                
            }
            
            // HomeController.ModelTrain();
            HomeController.BufferRoom = new LinkedList<Room>();
            HomeController.DopRefs = new LinkedList<string>();
        }
    }
}