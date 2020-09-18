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

            
            using (StreamReader sr = new StreamReader(@"/data\useragents.txt", System.Text.Encoding.Default))
            {
                int i = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                   HomeController.ua[i] = line;
                    i++;
                }
            }
           

           PageSearchingSheduler.Start();
          TelegramListenerSheduler.Start();
            AnnouncementSheduler.Start();
        }
    }
}
