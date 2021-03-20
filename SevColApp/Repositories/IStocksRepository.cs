using SevColApp.Models;
using System.Collections.Generic;

namespace SevColApp.Repositories
{
    public interface IStocksRepository
    {
        List<Company> GetAllCompanies();
        List<StockExchangeBuyRequest> GetBuyRequests(Company company);
        List<StockExchangeSellRequest> GetSellRequests(Company company);
        bool BuyerHasEnoughMoney(StockExchangeBuyRequest buyerRequest, uint numberOfStocks, uint pricePerStock);        
        bool SellerHasEnoughStocks(int sellerId, int companyId, uint numberOfStocks);
        bool SellerHasNoBankAccount(int sellerId);
        void TransferStocks(int buyerId, int sellerId, int companyId, uint numberOfStocksTransferred);
        void ArrangePayment(int buyerId, int sellerId, long priceToPay);
        void RemoveSellRequest(StockExchangeSellRequest sellRequest);
        void RemoveBuyRequest(StockExchangeBuyRequest buyRequest);
        void UpdateSellRequest(StockExchangeSellRequest sellRequest);
        void UpdateBuyRequest(StockExchangeBuyRequest buyRequest);
        void AddCompletedExchange(StockExchangeCompleted exchange);
        void Save();
    }
}
