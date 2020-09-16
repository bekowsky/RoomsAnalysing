using Accord.MachineLearning;
using RoomsAnalysing.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web;

namespace RoomsAnalysing.Models
{
    public class Notification
    {
        Parameters parameters;
        string mail;
        string telegram;
        LinkedList<Room> update;

       public  Notification(Parameters _parameters,string _mail,string _telegram)
        {
            parameters = _parameters;
            mail = _mail;
            telegram = _telegram;
            update = new LinkedList<Room> ();
        }

        public Parameters GetParameters()
        {
            return parameters;
        }
    public  void TakeUpdate(Room _update)
        {
          if(parameters.IsEqual(_update))
            update.AddLast(_update);
        }
       public bool IsContains(string m, string tg)
        {
            if ((mail == m && m!="") || (telegram == tg)&&(tg!=""))
                return true;
            return false;
        }

        public void Refresh(Parameters param, string m, string tg)
        {
            parameters = param;
            mail = m;
            telegram = tg;
        }
        public int UpdateCount()
        {
            return update.Count();
        }
        public void SendNotifications()
        {
      
            
            string text = "Вас может заинтересовать \n";
            if (update.Count>0)
            {
                int i = 0;
                foreach (Room room in update)
                {
                     text += "Цена " + room.price.ToString("#,#", new CultureInfo("ru-RU")) + " Средняя цена на рынке " + room.prediction.ToString("#,#", new CultureInfo("ru-RU")) + " ссылка " + "https://www.avito.ru" + room.avito_link + "\n\n\n";
                    i++;
                    if (i> 10)
                    {
                        text += "И много других вариантов";
                        break;
                    }
                       
                }
                if (telegram != "")
                {
                    WebRequest request = WebRequest.Create("https://api.telegram.org/bot1202283433:AAGQoFjX2yBYI_P-KTpK4dURHqPc-kvCua0/sendMessage?chat_id=" + HomeController.LoginId[telegram] + "&text=" + text);
                  
                    request.GetResponse();
                }
                if (mail != "")
                {
                    MailAddress from = new MailAddress("roomanalyzing@mail.ru", "Уведомление");
                    MailAddress to = new MailAddress(mail + "@mail.ru");
                    MailMessage m = new MailMessage(from, to);
                    m.Subject = "Объявление о выгодном подходящем вам варианте";
                    m.Body = text;
                    SmtpClient smtp = new SmtpClient("smtp.mail.ru", 2525);
                    smtp.Credentials = new NetworkCredential("roomanalyzing@mail.ru", "Kbhjq1337");
                    smtp.EnableSsl = true;

                    smtp.Send(m);
                }
               update = new LinkedList<Room> ();
            }
        }

    }
}