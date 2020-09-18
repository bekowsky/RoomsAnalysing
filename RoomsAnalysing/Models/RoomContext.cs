using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RoomsAnalysing.Models
{
    public class RoomContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
    
    }

    public class BookRepository : IDisposable
    {
        private RoomContext db = new RoomContext();

        public void Save(Room room)
        {
            db.Rooms.Add(room);
            db.SaveChanges();
        }
        public IEnumerable<Room> List()
        {
            return db.Rooms;
        }
        public Room Get(int id)
        {
            return db.Rooms.Find(id);
        }

        public void Remove(Room room)
        {
             db.Rooms.Remove(room);
            db.SaveChanges();
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}