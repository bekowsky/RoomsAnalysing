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
                    IEnumerable<Room> a = HomeController.RoomsAvito.Where(x => x.id == room.id);
               
                if (a.Count() > 0)
                {
                    HomeController.RoomsAvito.Remove(a.FirstOrDefault());
                }
                room.prediction = HomeController.CalculatingPrice(room);
                if (HomeController.NotificationList != null)
                    HomeController.NotificationList.SetUpdate(room);
                HomeController.RoomsAvito.AddFirst(room);
                //  HomeController.RoomsAvito = (LinkedList<Room>)HomeController.RoomsAvito.GroupBy(x => x.id).Select(x => x.First());
            }
            
            // HomeController.ModelTrain();
            HomeController.BufferRoom = new LinkedList<Room>();
            HomeController.DopRefs = new LinkedList<string>();
        }
    }
}