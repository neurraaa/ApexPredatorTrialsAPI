using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IIpAddress
    {
        IpAddress? Get(string ipAddress);
        void Add(IpAddress address);
    }
}
