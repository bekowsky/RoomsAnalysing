using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoomsAnalysing.Models
{
    public class NotificationList
    {
       LinkedList<Notification> Notifications;
        
      public  NotificationList()
        {
            Notifications = new LinkedList<Notification>() ;
        }
    public Parameters SearchParam(string mail,string telegram)
        {
            foreach(Notification notification in Notifications)
            {
                if (notification.IsContains(mail, telegram))
                    return (notification.GetParameters());
            }
            return null;
        }
        public void AddNotification(Notification notification)
        {
            Notifications.AddLast(notification);
        }
       public void SetUpdate (Room room)
        {
            if (Notifications != null)
            foreach(Notification notification in Notifications)
            {
                notification.TakeUpdate(room);
            }
        }
       public void AllNotifications()
        {
          
                foreach (Notification notification in Notifications)
                notification.SendNotifications();
        }

        public void Refresh(Parameters parameters, string mail, string telegram)
        {
            bool flag = true;
            foreach (Notification notification in Notifications)
                if (notification.IsContains(mail, telegram))
                {
                    notification.Refresh(parameters, mail, telegram);
                    flag = false;
                }
            if (flag)        
                AddNotification(new Notification(parameters, mail, telegram));
            

        }

        public void Drop(string mail,string telegram)
        {
            Notification n = new Notification(null,"","");
            foreach(Notification notification in Notifications)
            {
                if (notification.IsContains(mail,telegram))
                {
                    n = notification;
                }
            }
            Notifications.Remove(n);
        }

        public int check()
        {
           return( Notifications.Last.Value.UpdateCount());
        }

    }
}