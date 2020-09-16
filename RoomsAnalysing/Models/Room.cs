using RoomsAnalysing.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace RoomsAnalysing.Models
{
    public class Room
    {
        public string id { get; set; }
        public string avito_link { get; set; }
        public string cian_link { get; set; }
        public long price { get; set; }
        public int flat { get; set; }
        public int max_flat { get; set; }
        public int days_remains { get; set; }
        public double metro_distance { get; set; }
        public int centre_distance { get; set; }
        public double S { get; set; }
        public int num { get; set; }
        public string date { get; set; }
        public string metro { get; set; }
        public string adress { get; set; }
        public string room_type { get; set; }
       public DateTime dateTime { get; set; }
        public long prediction { get; set; }


        public void HeadInfoParse(string data)
        {
            if (data.Contains("Студия"))
                num = 0;
            else
                num = (int)char.GetNumericValue(data[0]);
            int i, j, k = data.IndexOf(',');
            string box = "";
            for (i = k + 2; data[i] != ' '; i++)
                box += data[i];

            S = Convert.ToDouble(box.Replace('.', ','));
            box = "";
            j = i;
            for (i = j + 1; data[i] != ' '; i++) ;
            j = i;
            for (i = j + 1; data[i] != '/'; i++)
                box += data[i];
            j = i;
            flat = Convert.ToInt32(box);
            box = "";

            for (i = j + 1; data[i] != ' '; i++)
                box += data[i];
            max_flat = Convert.ToInt32(box);

        }

        public void IdParse(string data)
        {

            string box = "";
            int k = data.IndexOf("flat");
            for (int i = k + 5; data[i] != '/'; i++)
            {
                box += data[i];
            }

            id = box;
        }

        public void PriceParse(string data)
        {
            string box = "";

            for (int j = 0; j < data.Length && data[j] != '₽'; j++)
                if ((data[j] != ' ') && (data[j] != '\n'))
                    box += data[j];
            price = Convert.ToInt64(box);
        }

        public void DeadLineParse(string data)
        {
            if ((data == "") || data.Contains("сдан"))
                days_remains = 0;
            else
            {
                int k = data.IndexOf(" кв.");
                int mounth = (int)char.GetNumericValue(data[k - 1]);
                k = data.IndexOf(" года");
                string box = "";
                for (int i = k - 4; data[i] != ' '; i++)
                    box += data[i];
                int year = Convert.ToInt32(box);
                DateTime end = new DateTime(year, mounth * 3, 1);
                TimeSpan timeSpan = end - DateTime.Now;
                if (timeSpan.Days > 0)
                    days_remains = timeSpan.Days;
                else
                    days_remains = 0;

            }
        }
        public void DistanseParse(string data)
        {
            if (data.Length == 1)
                metro_distance = 0;
            else
            {
                string box = "";
                for (int i = 1; data[i] != '\t' && data[i] != ' '; i++)
                    box += data[i];
                metro_distance = Convert.ToDouble(box);
                if (metro_distance < 10)
                    metro_distance *= 1000;
            }
        }
        public void dataParce(string data)
        {
            int num = 4;
            string box = "", hour = "", minute = "";
            int i, j, k;
            for (i = 0; data[i] != ' '; i++)
                box += data[i];
            if (data.Contains("марта"))
                num = 3;
            if (data.Contains("апреля"))
                num = 4;
            if (data.Contains("мая"))
                num = 5;
            if (data.Contains("июня"))
                num = 6;
            if (data.Contains("июля"))
                num = 7;
            if (data.Contains("августа"))
                num = 8;
            if (data.Contains("сентября"))
                num = 9;
            if (data.Contains("октября"))
                num = 10;
            if (data.Contains("ноября"))
                num = 11;

            i = data.IndexOf(':');
            for (j = i - 2; data[j] != ':'; j++)
                hour += data[j];
            for (i = j+1; i < data.Length; i++)
                minute += data[i];

            dateTime = new DateTime(2020, num, Convert.ToInt32(box), Convert.ToInt32(hour), Convert.ToInt32(minute), 0);

        }
        public void AdressParse(string adr)
        {
            string box = "";
            for (int i = 0; i < adr.Length; i++)
                if (adr[i] != '\n')
                    box += adr[i];
            adress = box.Trim();
        }
    }
}