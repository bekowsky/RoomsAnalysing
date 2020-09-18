
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Regression.Linear;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RoomsAnalysing.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace RoomsAnalysing.Controllers
{

    public static class HtmlNodeExtension
    {
       
        public static HtmlNode GetNode(this HtmlNode node, string classname)
        {
            if (node != null)
            {
                foreach (var item in node.ChildNodes)
                    if ((item.Attributes.Count > 0) && (item.Attributes.Contains("class")))
                        if (item.Attributes["class"].Value == classname)
                            return item;
            }
            return null;
        }

        public static HtmlNode GetNode(this HtmlNode node,string atrname, string classname)
        {
            if (node != null)
            {
                foreach (var item in node.ChildNodes)
                    if ((item.Attributes.Count > 0) && (item.Attributes.Contains(atrname)))
                        if (item.Attributes[atrname].Value == classname)
                            return item;
            }
            return null;
        }
    }
    public class HomeController : Controller
    {
        
        public static Random random = new Random();
        public static string[] ua = new string[3563];
        public static LinkedList<string> DopRefs = new LinkedList<string>();
        public static LinkedList<string> Errors = new LinkedList<string>();
        public static LinkedList<Room> BufferRoom = new LinkedList<Room>();
        public static LinkedList<MapClass> mapClasses = new LinkedList<MapClass>();
        public static MultipleLinearRegression regression = new MultipleLinearRegression();
        public static MultipleLinearRegression regression1 = new MultipleLinearRegression();
        public static LinkedList<MetroInfo> MetroInfos = new LinkedList<MetroInfo>();
        public static Dictionary<string, string> LoginId = new Dictionary<string, string>();
        public static NotificationList NotificationList = new NotificationList();
        public static  BookRepository repo;
        public HomeController()
        {
            repo = new BookRepository();
            var c = repo.List().OrderBy(x => x.metro).Select(x => x);

            foreach (Room room in c)
            {

                try
                {

                  MetroInfos.Single(x => x.metro == room.metro).price += room.price;
                    MetroInfos.Single(x => x.metro == room.metro).num++;
                }
                catch (Exception e)
                {
                  MetroInfos.AddLast(new MetroInfo { metro = room.metro, num = 1, price = room.price });

                }

            }

            foreach (MetroInfo Metroinfos in MetroInfos)
            {

                Metroinfos.price = (int)(Metroinfos.price / Metroinfos.num);
                Metroinfos.k = Metroinfos.price - 15000000;
                Metroinfos.k /= 1000000;
            }
                ModelTrain();
        }
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult Statistics()
        {
          
            Polynomial p = new Polynomial(2, 1);



            var m = repo.List().GroupBy(x => x.id).Select(x => x.First());
            var c = m.OrderBy(x => x.metro).Select(x => x);
           

            foreach (Room roomAvito in c)
            {
                int n = 1;
                if (roomAvito.room_type == "Вторичка")
                    n = 0;
                double k = 0;
                foreach (MetroInfo info in MetroInfos)
                    if (info.metro == roomAvito.metro)
                    {
                        k = info.k;
                        break;
                    }
                double[] z = p.Transform(new double[] { k, roomAvito.metro_distance, roomAvito.centre_distance, roomAvito.S, roomAvito.flat, roomAvito.max_flat, roomAvito.num, n });
                if (roomAvito.prediction < 1000000)
                    roomAvito.prediction = 1000000;
                if (roomAvito.prediction > 60000000)
                    roomAvito.prediction = 60000000;
            }


            ViewBag.Metro = MetroInfos;
            return View(c);
        }


        [HttpPost]
        public ActionResult ParamPage()
        {
            return PartialView();
        }


        [HttpPost]
        public ActionResult MailListPage(string mail,string telegram, string act)
        {
           if (act == "search")
            {
                Parameters parameters;
                try
                {
                    parameters = NotificationList.SearchParam(mail, telegram);
                    
                }
                catch (Exception e)
                {
                    parameters = null;
                }

                return PartialView(parameters);
            } else
            {
                
                NotificationList.Drop(mail, telegram);
                return PartialView(null);
            }
           
        }




        [HttpPost]
        public ActionResult AddMail(Parameters parameters)
        {



            return PartialView();
        }


        [HttpPost]
        public ActionResult RoomItems(PrimalParam param, int n, string sort, int page)
        {

            int count = Convert.ToInt32(n);
            int skip = count * (page - 1);
            bool[] num = new bool[10];
            num[0] = param.num0;
            num[1] = param.num1;
            num[2] = param.num2;
            num[3] = param.num3;
            if (param.num4)
                for (int i = 4; i < 10; i++)
                    num[i] = true;

            Parameters parameters = new Parameters(num, Convert.ToInt64(param.price1.Replace(" ", "").Replace(" ", "")), Convert.ToInt64(param.price2.Replace(" ", "").Replace(" ", "")), Convert.ToInt32(param.flat1), Convert.ToInt32(param.flat2), Convert.ToInt32(param.max_flat1), Convert.ToInt32(param.max_flat2), Convert.ToDouble(param.metro_distance1.Replace(" ", "").Replace(" ", "")), Convert.ToDouble(param.metro_distance2.Replace(" ", "").Replace(" ", "")), Convert.ToInt32(param.centre_distance1.Replace(" ", "").Replace(" ", "")), Convert.ToInt32(param.centre_distance2.Replace(" ", "").Replace(" ", "")), Convert.ToDouble(param.S1), Convert.ToDouble(param.S2));
            ICollection<Room> rooms = new LinkedList<Room>();
            ViewBag.page = page;
            Polynomial p = new Polynomial(2, 1);
            double[] inp;
            foreach (Room room in repo.List())
                if (parameters.IsEqual(room))
                {

                                  
                        int t = 1;
                        if (room.room_type == "Вторичка")
                            t = 0;
                        double k = 0;
                        foreach (MetroInfo info in MetroInfos)
                            if (info.metro == room.metro)
                            {
                                k = info.k;
                                break;
                            }
                        inp = new double[] { k, room.centre_distance, room.metro_distance, room.S, room.num, t };
                        double[] transformed = p.Transform(inp);


                        room.prediction =  Convert.ToInt64((regression1.Transform(transformed)));
                    room.adress =regression1.Transform(transformed).ToString("#,#",new CultureInfo("ru-RU"));
                    room.room_type = room.price.ToString("#,#", new CultureInfo("ru-RU"));
                    rooms.Add(room);
                }

         
            if (sort == "1")
                return PartialView(rooms.OrderBy(x => x.price).Skip(skip).Take(count));
            if (sort == "2")
                return PartialView(rooms.OrderByDescending(x => x.price).Skip(skip).Take(count));
            if (sort == "3")
                return PartialView(rooms.OrderByDescending(x => x.dateTime).Skip(skip).Take(count));


            return PartialView();
        }

        public ActionResult About()
        {
            string output = "";
            using (StreamReader sr = new StreamReader(@"C:\data\map.txt", System.Text.Encoding.UTF8))
            {

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    output += line;
                }

            }
            mapClasses = JsonConvert.DeserializeObject<LinkedList<MapClass>>(output);

            return View(mapClasses);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ActionName("Contact")]
        public ActionResult Contact1(PrimalParam param, string mailname, string telegramname)
        {

            bool[] num = new bool[10];
            num[0] = param.num0;
            num[1] = param.num1;
            num[2] = param.num2;
            num[3] = param.num3;
            if (param.num4)
                for (int i = 4; i < 10; i++)
                    num[i] = true;

            Parameters parameters = new Parameters(num, Convert.ToInt64(param.price1.Replace(" ", "").Replace(" ", "")), Convert.ToInt64(param.price2.Replace(" ", "").Replace(" ", "")), Convert.ToInt32(param.flat1), Convert.ToInt32(param.flat2), Convert.ToInt32(param.max_flat1), Convert.ToInt32(param.max_flat2), Convert.ToDouble(param.metro_distance1.Replace(" ", "").Replace(" ", "")), Convert.ToDouble(param.metro_distance2.Replace(" ", "").Replace(" ", "")), Convert.ToInt32(param.centre_distance1.Replace(" ", "").Replace(" ", "")), Convert.ToInt32(param.centre_distance2.Replace(" ", "").Replace(" ", "")), Convert.ToDouble(param.S1), Convert.ToDouble(param.S2));
            NotificationList.Refresh(parameters, mailname, telegramname);

            return View();
        }



        public static void Search_data(string url, string room_type, int i)
        {
            string data = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + Convert.ToString(i));
     
            request.UserAgent = ua[random.Next(1, 3000)];
           
            request.Accept = "text/css,*/*;q=0.1";
            
            request.Headers.Add("Cookie", "u=2k2bgyex.pkwev6.gdi46cy6j6; buyer_location_id=637640; buyer_selected_search_radius4=0_general; __cfduid=d0460e5c20c7526dacdb5d86605e64e331586011299; v=1587912293; luri=moskva; sx=H4sIAAAAAAACAwXBMQ6AIAwAwL90dkAsWviNNsaYiiVB6GD8u3cv0OZz7k%2FRm1HEVawiaqoO0gsdElxrie0U34IRs1Q2RcaD0EyECQbYIY2BlhhpjtP3%2FS%2BEWtxUAAAA; so=1587912293; dfp_group=15; sessid=492903f69029a6ccd18833af4da86b4c.1587912294; f=5.0c4f4b6d233fb90636b4dd61b04726f147e1eada7172e06c47e1eada7172e06c47e1eada7172e06c47e1eada7172e06cb59320d6eb6303c1b59320d6eb6303c1b59320d6eb6303c147e1eada7172e06c8a38e2c5b3e08b898a38e2c5b3e08b890df103df0c26013a7b0d53c7afc06d0b2ebf3cb6fd35a0ac7b0d53c7afc06d0b8b1472fe2f9ba6b9e2bfa4611aac769efa4d7ea84258c63d59c9621b2c0fa58fd50b96489ab264ed3de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe24b139bf5510fef5f186983488f942d6512f2e9b11da1bd3ac85bb1ea081291a5194a51a925c58bed8746e1777574c790f2ad963065013cdcd21ab7cd585086e0422f7d8d602e4cfe32a740d09aef1e01343076c04c14cfd09d9b2ff8011cc827cbf1a5019b899285ad09145d3e31a569ca0c25e61978406f1cf5fc67cf4c79a971e7cb57bbcb8e0fd1d953d27484fd8115b1adc30b19c0484f24917cd5f6b2ac");
        
            request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");


          
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;
                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }
                    data = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(data);
                    HtmlNode node = html.DocumentNode;
                

                    var Nodes = node.ChildNodes[3].ChildNodes[3].GetNode("js-single-page single-page").GetNode("index-center-2ZEUx index-center_withTitle-3Ne4c index-responsive-3H_cC").GetNode("index-inner-LCNXs index-innerCatalog-mLDlZ").GetNode("index-content-2lnSO").GetNode("index-root-2c0gs").FirstChild.ChildNodes[1].ChildNodes.Where(x => x.Name == "div");

                    foreach (var item in Nodes)
                    {
                        if (item.Attributes[0].Value == "snippet snippet-horizontal  snippet-redesign  item-snippet-with-aside item item_table clearfix js-catalog-item-enum item-with-contact js-item-extended")
                        {
                                Room roomAvito = new Room();

                                roomAvito.room_type = room_type;

                                string id = item.Attributes[4].Value;
                                roomAvito.id = id;

                                string avito_link = item.ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[2].Value;
                                roomAvito.avito_link = avito_link;

                                string pre_title = item.ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[1].ChildNodes[0].InnerText;
                                roomAvito.HeadInfoParse(pre_title);


                                string pre_price = item.ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[3].InnerText;
                                roomAvito.PriceParse(pre_price);



                                string metro = "";
                                
                               
                            HtmlNode metro_dist = item.GetNode("item__line").GetNode("item_table-wrapper").GetNode("description item_table-description").GetNode("data-marker", "item-address").GetNode("item-address").GetNode("itemprop", "address").GetNode("item-address-georeferences").GetNode("item-address-georeferences").GetNode("item-address-georeferences-item");
                        if (metro_dist.GetNode("item-address-georeferences-item__content") == null || metro_dist.GetNode("item-address-georeferences-item__after") == null)
                        {
                            metro = "Не Москва";
                        }
                        else
                        {
                            metro = metro_dist.GetNode("item-address-georeferences-item__content").InnerText;

                            string pre_distance;

                            pre_distance = metro_dist.GetNode("item-address-georeferences-item__after").InnerText;
                            roomAvito.DistanseParse(pre_distance);
                        }
                              //  string adress = item.GetNode("item__line").GetNode("item_table-wrapper").GetNode("description item_table-description").GetNode("data-marker", "item-address").GetNode("item-address").GetNode("itemprop", "address").GetNode("item-address__string").InnerText;
                               // roomAvito.AdressParse(adress);
                                roomAvito.metro = metro;
                                roomAvito.centre_distance = (int)MetroDistanseCalculating(roomAvito.adress);

                                string datatime = item.GetNode("item__line").GetNode("item_table-wrapper").GetNode("description item_table-description").GetNode("snippet-date-row").GetNode("snippet-date-info").Attributes["data-tooltip"].Value;

                              
                                roomAvito.date = datatime;
                                roomAvito.dataParce(datatime);

                               
                               

                                BufferRoom.AddLast(roomAvito);


                            }
                            } 
                    }

                }
           
        

        static int Search_refs(string url, string room_type, int i)
        {
            string data = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + Convert.ToString(i));
            request.UserAgent = ua[random.Next(1, 3000)];
            request.Accept = "text/css,*/*;q=0.1";
            request.Headers.Add("Cookie", "u=2k2bgyex.pkwev6.gdi46cy6j6; buyer_location_id=637640; buyer_selected_search_radius4=0_general; __cfduid=d0460e5c20c7526dacdb5d86605e64e331586011299; v=1587912293; luri=moskva; sx=H4sIAAAAAAACAwXBMQ6AIAwAwL90dkAsWviNNsaYiiVB6GD8u3cv0OZz7k%2FRm1HEVawiaqoO0gsdElxrie0U34IRs1Q2RcaD0EyECQbYIY2BlhhpjtP3%2FS%2BEWtxUAAAA; so=1587912293; dfp_group=15; sessid=492903f69029a6ccd18833af4da86b4c.1587912294; f=5.0c4f4b6d233fb90636b4dd61b04726f147e1eada7172e06c47e1eada7172e06c47e1eada7172e06c47e1eada7172e06cb59320d6eb6303c1b59320d6eb6303c1b59320d6eb6303c147e1eada7172e06c8a38e2c5b3e08b898a38e2c5b3e08b890df103df0c26013a7b0d53c7afc06d0b2ebf3cb6fd35a0ac7b0d53c7afc06d0b8b1472fe2f9ba6b9e2bfa4611aac769efa4d7ea84258c63d59c9621b2c0fa58fd50b96489ab264ed3de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe23de19da9ed218fe24b139bf5510fef5f186983488f942d6512f2e9b11da1bd3ac85bb1ea081291a5194a51a925c58bed8746e1777574c790f2ad963065013cdcd21ab7cd585086e0422f7d8d602e4cfe32a740d09aef1e01343076c04c14cfd09d9b2ff8011cc827cbf1a5019b899285ad09145d3e31a569ca0c25e61978406f1cf5fc67cf4c79a971e7cb57bbcb8e0fd1d953d27484fd8115b1adc30b19c0484f24917cd5f6b2ac");
            //request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");


            try
            {

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;
                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }
                    data = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(data);
                    HtmlNode node = html.DocumentNode;


                    var jopa = node.ChildNodes[3].ChildNodes[3].GetNode("js-single-page single-page").GetNode("index-center-2ZEUx index-center_withTitle-3Ne4c index-responsive-3H_cC").GetNode("index-inner-LCNXs").GetNode("index-content-2lnSO index-content_narrow-1fWCX").GetNode("js-pages pagination-pagination-2j5na");
                    int nomer = 1;
                    if (jopa != null)
                        nomer = jopa.ChildNodes[1].LastChild.ChildNodes.Count - 1;

                    var Nodes = node.ChildNodes[3].ChildNodes[3].GetNode("js-single-page single-page").GetNode("index-center-2ZEUx index-center_withTitle-3Ne4c index-responsive-3H_cC").GetNode("index-inner-LCNXs").GetNode("index-content-2lnSO index-content_narrow-1fWCX").GetNode("index-root-2c0gs").FirstChild.ChildNodes[1].ChildNodes.Where(x => x.Name == "div");



                    foreach (var item in Nodes)
                    {
                        if (item.Attributes[0].Value == "snippet-horizontal   item item_table clearfix js-catalog-item-enum item-with-contact js-item-extended")
                        {

                            //  var j =      item.ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[5].ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes.Count;
                            Room roomAvito = new Room();

                            roomAvito.room_type = room_type;

                            string id = item.Attributes[4].Value;
                            roomAvito.id = id;

                            string avito_link = item.ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[2].Value;
                            roomAvito.avito_link = avito_link;

                            string pre_title = item.ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[1].ChildNodes[0].InnerText;
                            roomAvito.HeadInfoParse(pre_title);


                            string pre_price = item.ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[3].InnerText;
                            roomAvito.PriceParse(pre_price);



                            string metro = "";
                            var bag = item.ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[7];
                            try
                            {

                                metro = bag.ChildNodes[bag.ChildNodes.Count - 2].ChildNodes[0].ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[1].InnerText;

                                string pre_distance;

                                pre_distance = bag.ChildNodes[bag.ChildNodes.Count - 2].ChildNodes[0].ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[3].InnerText;
                                roomAvito.DistanseParse(pre_distance);
                            }
                            catch (Exception ex)
                            {
                                metro = "Не москва";
                            }

                            string adress = bag.ChildNodes[bag.ChildNodes.Count - 2].ChildNodes[0].ChildNodes[0].ChildNodes[0].InnerText;
                            roomAvito.AdressParse(adress);
                            roomAvito.metro = metro;
                            roomAvito.centre_distance = (int)MetroDistanseCalculating(roomAvito.adress);



                            string datatime = item.ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[9].GetNode("snippet-date-row").ChildNodes[0].Attributes[3].Value;
                            if (datatime == "")
                            {
                                datatime = item.ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[9].GetNode("snippet-date-row").ChildNodes[0].InnerText;
                                datatime = datatime.Substring(2, datatime.Length - 3);
                            }
                            roomAvito.date = datatime;


                           

                            BufferRoom.AddLast(roomAvito);



                        }
                    }
                    return nomer;
                }
            }
            catch (Exception error)
            {
                Errors.Append(url);
                return 0;
            }
            return 0;
        }

        public static void Search_Cian()
        {
            LinkedList<CianModel> list = new LinkedList<CianModel>();
            string j = @"{""jsonQuery"":{""region"":{""type"":""terms"",""value"":[1]},""_type"":""flatsale"",""room"":{""type"":""terms"",""value"":[1,2]},""engine_version"":{ ""type"":""term"",""value"":2},""page"":{ ""type"":""term"",""value"":54}},""offerIds"":[223880567,231803326,232031026,229407144,226421508,223368289,217247501,231748559,231962491,231957802,228465732,228586781,219255968,230686257,203366357,228534863,228817980,229540432,223939601,211473915,213597457,223045546,226270725,227934975,230529264],""pageNumber"":1,""queryString"":""deal_type = sale & engine_version = 2 & offer_type = flat & p = 54 & region = 1 & room1 = 1 & room2 = 1""}";
            var httpRequest = (HttpWebRequest)WebRequest.Create("https://api.cian.ru/search-offers/v1/get-infinite-search-result-desktop/");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            using (var requestStream = httpRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(j);
            }
            string answer;
            using (var httpResponse = httpRequest.GetResponse())
            using (var responseStream = httpResponse.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                string response = reader.ReadToEnd();
                int n =  response.IndexOf('[');
                response = response.Substring(n, response.Length - n - 1) ;
                list = JsonConvert.DeserializeObject<LinkedList<CianModel>>(response);
            }

            string j1 = @"{""cianOfferIds"":[225147990,219255833,228288865,228788883,227086038,229655389,227901662,231185680,223322944,220043554,230905642,225544252,230955983,227970474,218948204,231601321,207665287,211077403,226291726,231623895,226910796,231015193,227973842,228611752,200016435],""jsonQuery"":{ ""region"":{ ""type"":""terms"",""value"":[1]},""_type"":""flatsale"",""room"":{""type"":""terms"",""value"":[1,2]},""engine_version"":{""type"":""term"",""value"":2},""page"":{""type"":""term"",""value"":54}}}";

            httpRequest = (HttpWebRequest)WebRequest.Create("https://api.cian.ru/search-offers/v1/get-offers-by-ids-desktop/");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            using (var requestStream = httpRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(j1);
            }
            
            using (var httpResponse = httpRequest.GetResponse())
            using (var responseStream = httpResponse.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                string response = reader.ReadToEnd();
                int n = response.IndexOf('[');
                response = response.Substring(n, response.Length - n - 1);
                var t = JsonConvert.DeserializeObject(response);
            }




        }
        public static void searchDop()
        {
           
            int j = 1;
            foreach (string str in DopRefs)
            {
                j = Search_refs(str, "Новостройка", j);
                if (j > 1)
                    for (int k = 2; k <= j; k++)
                        Search_refs(str, "Новостройка", k);              
            }
          
        }

        public static string yandex(string adress)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://geocode-maps.yandex.ru/1.x/?apikey=9ef17b0e-34e8-4246-96a8-7732347817de&geocode=Москва " + adress);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string data;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;
                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }
                    data = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(data);
                    HtmlNode node = html.DocumentNode;
                    string nod = node.ChildNodes[1].ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes[4].InnerText;
                    return (nod);
                }
                return ("-1");
            }
            catch (Exception e)
            {
                Errors.Append(adress);
                return ("-1");
            }
        }

        public static double calc(double b, double a)
        {

            double w1 = a * Math.PI / 180;
            double d1 = b * Math.PI / 180;
            double w2 = 55.753595 * Math.PI / 180;
            double d2 = 37.621031 * Math.PI / 180;


            double d = 12742000 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((w2 - w1) / 2), 2) + Math.Cos(w1) * Math.Cos(w2) * Math.Pow(Math.Sin((d2 - d1) / 2), 2)));
            return d;
        }

        public static double MetroDistanseCalculating (string adress)
        {
           string box = yandex(adress);
            double d;
            if (box != "-1")
            {
                string[] points = box.Split(' ');
                d = calc(Convert.ToDouble(points[0].Replace('.', ',')), Convert.ToDouble(points[1].Replace('.', ',')));
                if (d > 0 && d < 55000)
                    return d;
            }

            return -1;
        }

        public static void ModelTrain()
        {
            // var m = RoomsAvito.GroupBy(x => x.id).Select(x => x.First());
           // var RoomsAvito = HomeController.RoomsAvito.Take(30000);
            var m = repo.List().Count();
             
           
  
            double[][] inp = new double[repo.List().Count()][];
            double[] outp = new double[repo.List().Count()];
            int i = 0;

          
            foreach (Room roomAvito in repo.List())
            {

                int n = 1;
                if (roomAvito.room_type == "Вторичка")
                    n = 0;
                double k = 0;
                foreach(MetroInfo info in MetroInfos)
                    if(info.metro == roomAvito.metro)
                    {
                        k = info.k;
                        break;
                    }
                 inp[i] = new double[] {k, roomAvito.centre_distance, roomAvito.metro_distance, roomAvito.S, roomAvito.num,n };
                outp[i] = (int)roomAvito.price;
                i++;
            }
             Accord.Math.Random.Generator.Seed = 0;
             var ols = new OrdinaryLeastSquares();
             {
                 ols.UseIntercept = true;
                 ols.IsRobust = true;
             };

             regression = ols.Learn(inp, outp);


            Polynomial p = new Polynomial(2, 1);

            double[][] z = p.Transform(inp);
            
            // Now, create an usual OLS algorithm
            var ols1 = new OrdinaryLeastSquares()
            {
                UseIntercept = true
            };

            // Use the algorithm to learn a multiple regression
           regression1 = ols1.Learn(z, outp);

            // Check the quality of the regression:
            



            
           

        }

   

       public static void SendEmail(string mail, string url, double percent)
        {
            MailAddress from = new MailAddress("roomanalyzing@mail.ru", "Уведомление");
            MailAddress to = new MailAddress(mail);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Объявление о выгодном подходящем вам варианте";
            m.Body = "По вашим критериям мы нашли для вас подходящий вариант "+ url +" на " + Convert.ToString(percent) + "% дешевле таких же на рынке";
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 2525);
            smtp.Credentials = new NetworkCredential("roomanalyzing@mail.ru", "Kbhjq1337");
            smtp.EnableSsl = true;
       
            smtp.Send(m);
            
        }

        public static long CalculatingPrice(Room room)
        {
            Polynomial p = new Polynomial(2, 1);
            int t = 1;
            if (room.room_type == "Вторичка")
                t = 0;
            double k = 0;
            foreach (MetroInfo info in MetroInfos)
                if (info.metro == room.metro)
                {
                    k = info.k;
                    break;
                }
           double [] inp = new double[] { k, room.centre_distance, room.metro_distance, room.S, room.num, t };
            double[] transformed = p.Transform(inp);
           return ( Convert.ToInt64((regression1.Transform(transformed))));

        }


    }
}