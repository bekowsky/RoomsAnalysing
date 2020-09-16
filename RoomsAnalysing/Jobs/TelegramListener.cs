
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Accord.Math;
using Newtonsoft.Json;
using Quartz;
using RoomsAnalysing.Controllers;
using RoomsAnalysing.Models;
namespace RoomsAnalysing.Jobs
{
    public class TelegramListener : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.telegram.org/bot1202283433:AAGQoFjX2yBYI_P-KTpK4dURHqPc-kvCua0/getUpdates");
           
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string data = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == "")
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }
                data = readStream.ReadToEnd();

                dynamic well = JsonConvert.DeserializeObject<dynamic>(data);
                int n = well.result.Count;

                for (int i = 0; i < n; i++) {

               

                    if (!HomeController.LoginId.ContainsKey(Convert.ToString( well.result[i].message.chat.username)))
                    {
                        HomeController.LoginId.Add(Convert.ToString(well.result[i].message.chat.username), Convert.ToString(well.result[i].message.chat.id));

                    }
            }
               
            }
            }
    }
}