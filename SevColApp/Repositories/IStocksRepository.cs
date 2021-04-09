using SevColApp.Models;
using System.Collections.Generic;

namespace SevColApp.Repositories
{
    public interface IStocksRepository
    {
        List<Company> GetAllCompanies();
        Company GetCompanyByName(string companyName);
        StockExchangeBuyRequest AddBuyRequest(StockExchangeBuyRequest request);
        StockExchangeSellRequest AddSellRequest(StockExchangeSellRequest request);
        List<StockExchangeBuyRequest> GetBuyRequests(Company company);
        List<StockExchangeSellRequest> GetSellRequests(Company company);
        long UserTotalCredits(int userId);
        uint AmountOfSellerStocks(int sellerId, Company company);
        bool UserHasNoBankAccount(int userId);
        void TransferStocks(int buyerId, int sellerId, int companyId, uint numberOfStocksTransferred);
        void ArrangePayment(string buyerAccountNumber, string sellerAccountNumber, long priceToPay);
        void RemoveSellRequest(StockExchangeSellRequest sellRequest);
        void RemoveBuyRequest(StockExchangeBuyRequest buyRequest);
        void UpdateSellRequest(StockExchangeSellRequest sellRequest);
        void UpdateBuyRequest(StockExchangeBuyRequest buyRequest);
        void AddCompletedExchange(StockExchangeCompleted exchange);
        UsersCurrentStocks GetStocksFromUser(int id);
        List<BankAccount> GetBankAccountsFromUser(int userId);        
        void Save();
    }
}
