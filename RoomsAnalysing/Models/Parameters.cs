using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace RoomsAnalysing.Models
{
    public class Parameters
    {
        public long price1  { get; set; }
        public long price2 { get; set; }
        public int flat1 { get; set; }
        public int flat2 { get; set; }
        public int max_flat1 { get; set; }
        public int max_flat2 { get; set; }
        public double metro_distance1 { get; set; }
        public double metro_distance2 { get; set; }
        public int centre_distance1 { get; set; }
        public int centre_distance2 { get; set; }
        public double S1 { get; set; }
        public double S2 { get; set; }
        public bool [] num { get; set; }
        public int page { get; set; }

        public Parameters(bool[] n,long p1 = -1, long p2 = -1, int f1 = -1, int f2 = -1, int mf1 = -1, int mf2 = -1, double md1 = -1, double md2 = -1,int cd1 = -1, int cd2 = -1,double s1 = -1, double s2 = -1)
        {
            page = 1;
            price1 = p1;
            price2 = p2;
            flat1 = f1;
            flat2 = f2;
            max_flat1 = mf1;
            max_flat2 = mf2;
            metro_distance1 = md1;
            metro_distance2 = md2;
            centre_distance1 = cd1;
            centre_distance2 = cd2;
            S1 = s1;
            S2 = s2;
            num = n;
        }
       public  bool IsEqual (Room room)
        {
          
            if (price1 != -1)
                if (room.price < price1)
                    return false;
            if (price2 != -1)
                if (room.price > price2)
                    return false;
            if (flat1 != -1)
                if (room.flat < flat1)
                    return false;
            if (flat2 != -1)
                if (room.flat > flat2)
                    return false;
            if (max_flat1 != -1)
                if (room.max_flat < max_flat1)
                    return false;
            if (max_flat2 != -1)
                if (room.max_flat > max_flat2)
                    return false;
            if (metro_distance1 != -1)
                if (room.metro_distance < metro_distance1)
                    return false;
            if (metro_distance2 != -1)
                if (room.metro_distance > metro_distance2)
                    return false;
         
            if (S1 != -1)
                if (room.S < S1)
                    return false;
            if (S2 != -1)
                if (room.S > S2)
                    return false;
            if (num[room.num] == false)
                return false;

            return true;
        }

    }
}