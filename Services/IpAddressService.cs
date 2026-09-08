using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Services
{
    public class IpAddressService : IIpAddress
    {
        private readonly AppDbContext _dbContext;

        public IpAddressService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IpAddress? Get(string address)
        {
            return _dbContext.IpAddresses.FirstOrDefault(i => i.Address == address);
        }

        public void Add(IpAddress ipAddress)
        {
            _dbContext.IpAddresses.Add(ipAddress);
            _dbContext.SaveChanges();
        }
    }
}
