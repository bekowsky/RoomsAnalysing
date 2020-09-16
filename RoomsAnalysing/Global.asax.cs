using RoomsAnalysing.Jobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using RoomsAnalysing.Controllers;
using RoomsAnalysing.Models;
using Newtonsoft.Json;

namespace RoomsAnalysing
{
    public class MvcApplication : System.Web.HttpApplication
    {
       // public static string[] ua = new string[3563];
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            
            using (StreamReader sr = new StreamReader(@"C:\data\useragents.txt", System.Text.Encoding.Default))
            {
                int i = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                   HomeController.ua[i] = line;
                    i++;
                }
            }
           


           string output = "";
            using (StreamReader sr = new StreamReader(@"C:\data\info1.txt", System.Text.Encoding.UTF8))
            {
               
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    output += line;
                }

            }
           HomeController.RoomsAvito = JsonConvert.DeserializeObject<LinkedList<Room>>(output);
            var m = HomeController.RoomsAvito.GroupBy(x => x.id).Select(x => x.First());
            var c = m.OrderBy(x => x.metro).Select(x => x);

            foreach (Room room in c)
            {

                try
                {

                    HomeController.MetroInfos.Single(x => x.metro == room.metro).price += room.price;
                    HomeController.MetroInfos.Single(x => x.metro == room.metro).num++;
                }
                catch (Exception e)
                {
                    HomeController.MetroInfos.AddLast(new MetroInfo { metro = room.metro, num = 1, price = room.price });

                }

            }

            foreach (MetroInfo Metroinfos in HomeController.MetroInfos)
            {

                Metroinfos.price = (int)(Metroinfos.price / Metroinfos.num);
                Metroinfos.k = Metroinfos.price - 15000000;
                Metroinfos.k /= 1000000;
            }
            HomeController.ModelTrain();

            PageSearchingSheduler.Start();
           TelegramListenerSheduler.Start();
            AnnouncementSheduler.Start();
        }
    }
}
