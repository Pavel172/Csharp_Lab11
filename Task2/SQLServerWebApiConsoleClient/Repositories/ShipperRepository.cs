using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Concurrent;
using WebAPIModels.Models;

namespace SQLServerWebApiConsoleClient.Repositories
{
    public class ShipperRepository : IShipperRepository
    {

        public static ConcurrentDictionary<int, Shipper> shippersCache;
        private NorthwindContext db;

        public ShipperRepository(NorthwindContext db)
        {
            this.db = db;

            if (shippersCache == null)
            {
                //ключи - CustomerId
                shippersCache = new ConcurrentDictionary<int, Shipper>(db.Shippers.ToDictionary(c => c.ShipperId));
            }
        }
        private Shipper UpdateCache(int id, Shipper shipper)
        {
            //TryGetValue - приставка "Try", concurrentDictionary многопоточная коллекция
            Shipper old;
            if (shippersCache.TryGetValue(id, out old))
            {
                if (shippersCache.TryUpdate(id, shipper, old))
                {
                    return shipper;
                }
            }
            return null;
        }

        public async Task<Shipper> CreateAsync(Shipper shipper)
        {

            EntityEntry<Shipper> added = await db.Shippers.AddAsync(shipper);
            int affectedRows = await db.SaveChangesAsync();
            if (affectedRows > 0)
            {
                return shippersCache.AddOrUpdate(shipper.ShipperId, shipper, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int shipperId)
        {
            // удаление из базы данных
            Shipper? c = db.Shippers.Find(shipperId);
            db.Shippers.Remove(c);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // удаление из кэша
                return shippersCache.TryRemove(shipperId, out c);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<Shipper>> GetAllAsync()
        {
            // извлечение из кэша для производительности
            return Task.Run<IEnumerable<Shipper>>(() => shippersCache.Values);
        }

        public Task<Shipper?> GetAsync(int shipperId)
        {
            return Task.Run(() =>
            {
                // извлечение из кэша для производительности
                shippersCache.TryGetValue(shipperId, out Shipper? c);
                return c;
            });
        }

        public async Task<Shipper> UpdateAsync(int shipperId, Shipper shipper)
        {

            // обновление в базе данных
            db.Shippers.Update(shipper);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // обновление в кэше
                return UpdateCache(shipperId, shipper);
            }
            return null;
        }
    }
}