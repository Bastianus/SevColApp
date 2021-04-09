using System.Threading;
using System.Threading.Tasks;

namespace SevColApp.Hosted_service
{
    public interface IStockExchange
    {
        Task ExchangeStocks(CancellationToken token);
    }
}
