using WebAPIModels.Models;

namespace SQLServerWebApiConsoleClient.Repositories
{
    public interface IShipperRepository
    {
        Task<Shipper?> GetAsync(int shipperId);
        Task<Shipper> UpdateAsync(int shipperId, Shipper shipper);
        Task<bool?> DeleteAsync(int shipperId);
        Task<IEnumerable<Shipper>> GetAllAsync();
        Task<Shipper> CreateAsync(Shipper shipper);
    }
}
