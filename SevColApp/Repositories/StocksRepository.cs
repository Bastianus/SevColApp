using SevColApp.Context;
using SevColApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public class StocksRepository : IStocksRepository
    {
        private readonly SevColContext _context;

        public StocksRepository(SevColContext context)
        {
            _context = context;
        }

        public List<Company> GetAllCompanies()
        {
            return _context.Companies.ToList();
        }

        public Company GetCompanyByName(string companyName)
        {
            return _context.Companies.FirstOrDefault(company => company.Name == companyName);
        }

        public StockExchangeBuyRequest AddBuyRequest(StockExchangeBuyRequest request)
        {
            if (request.NumberOfStocks < 1) request.Errors.Add($"The number of stocks requested ({request.NumberOfStocks}) has to be greater than zero.");

            if (request.OfferPerStock < 0) request.Errors.Add($"The offer per stock ({request.OfferPerStock}) cannot be negative.");

            if (!CompanyExists(request.CompanyName)) request.Errors.Add($"The company {request.CompanyName} does not exist.");

            if (!BankAccountHasEnoughMoney(request.AccountNumber, request.NumberOfStocks, request.OfferPerStock)) request.Errors.Add($"You do not have {request.NumberOfStocks * request.OfferPerStock} credits on bank account with account number {request.AccountNumber} to pay for this request.");

            if(request.Errors.Count == 0)
            {
                _context.StockExchangeBuyRequests.Add(request);
            }

            return request;
        }

        public StockExchangeSellRequest AddSellRequest(StockExchangeSellRequest request)
        {
            if (!CompanyExists(request.CompanyName)) request.Errors.Add($"The company {request.CompanyName} does not exist.");
            else if (!UserHasEnoughStocksFromCompany(request.CompanyName, request.NumberOfStocks)) request.Errors.Add($"You do not have enough stocks from {request.CompanyName}.");


            if(request.Errors.Count == 0)
            {
                _context.StockExchangeSellRequests.Add(request);
            }

            return request;
        }

        public List<StockExchangeBuyRequest> GetBuyRequests(Company company)
        {
            return _context.StockExchangeBuyRequests.Where(br => br.companyId == company.Id).OrderByDescending(br => br.OfferPerStock).ThenBy(br => br.NumberOfStocks).ToList();
        }

        public List<StockExchangeSellRequest> GetSellRequests(Company company)
        {
            return _context.StockExchangeSellRequests.Where(br => br.companyId == company.Id).OrderBy(br => br.MinimumPerStock).ThenBy(br => br.NumberOfStocks).ToList();
        }

        public long UserTotalCredits(int userId)
        {
            return _context.BankAccounts.Where(ba => ba.userId == userId)?.Select(ba => ba.Credit).Sum() ?? 0;
        }

        public uint AmountOfSellerStocks(int sellerId, Company company)
        {
            if (!_context.UserCompanyStocks.Any(uc => uc.companyId == company.Id && uc.userId == sellerId)) return 0;

            return _context.UserCompanyStocks.Where(uc => uc.companyId == company.Id && uc.userId == sellerId).Single().NumberOfStocks;
        }

        public bool UserHasNoBankAccount(int userId)
        {
            return !_context.BankAccounts.Any(ba => ba.userId == userId);
        }

        public void TransferStocks(int buyerId, int sellerId, int companyId, uint numberOfStocksTransferred)
        {
            var sellerStocks = _context.UserCompanyStocks.Where(uc => uc.userId == sellerId && uc.companyId == companyId).First();

            sellerStocks.NumberOfStocks -= numberOfStocksTransferred;

            _context.UserCompanyStocks.Update(sellerStocks);

            var buyerStocks = _context.UserCompanyStocks.Where(uc => uc.userId == buyerId && uc.companyId == companyId).FirstOrDefault();

            if (buyerStocks == null)
            {
                buyerStocks = new UserCompanyStocks
                {
                    userId = buyerId,
                    companyId = companyId,
                    NumberOfStocks = numberOfStocksTransferred
                };

                _context.UserCompanyStocks.Add(buyerStocks);
            }
            else
            {
                buyerStocks.NumberOfStocks += numberOfStocksTransferred;

                _context.UserCompanyStocks.Update(buyerStocks);
            }
        }

        public void ArrangePayment(string buyerAccountNumber, string sellerAccountNumber, long priceToPay)
        {
            var receivingAccount = _context.BankAccounts.Single(ba => ba.AccountNumber == sellerAccountNumber);

            var payingAccount = _context.BankAccounts.Single(ba => ba.AccountNumber == buyerAccountNumber);

            if (payingAccount.Credit < priceToPay)
            {
                throw new Exception($"Bank account {payingAccount} does not have the required {priceToPay} credits.");
            }

            payingAccount.Credit -= priceToPay;
            receivingAccount.Credit += priceToPay;

            _context.BankAccounts.Update(payingAccount);
            _context.BankAccounts.Update(receivingAccount);
        }

        public void RemoveSellRequest(StockExchangeSellRequest sellRequest)
        {
            _context.StockExchangeSellRequests.Remove(sellRequest);
        }

        public void RemoveBuyRequest(StockExchangeBuyRequest buyRequest)
        {
            _context.StockExchangeBuyRequests.Remove(buyRequest);
        }

        public void UpdateSellRequest(StockExchangeSellRequest sellRequest)
        {
            _context.StockExchangeSellRequests.Update(sellRequest);
        }
        public void UpdateBuyRequest(StockExchangeBuyRequest buyRequest)
        {
            _context.StockExchangeBuyRequests.Update(buyRequest);
        }
        public void AddCompletedExchange(StockExchangeCompleted exchange)
        {
            _context.StockExchangesCompleted.Add(exchange);
        }

        public UsersCurrentStocks GetStocksFromUser(int userId)
        {
            var answer = new UsersCurrentStocks();

            answer.UserStocks.AddRange(_context.UserCompanyStocks.Where(ucs => ucs.userId == userId).Select(ucs => new UserStocks { Company = ucs.Company, NumberofStocks = ucs.NumberOfStocks }));

            answer.UserStocks = answer.UserStocks.OrderByDescending(us => us.NumberofStocks).ThenBy(us => us.Company.Name).ToList();

            return answer;
        }

        public List<BankAccount> GetBankAccountsFromUser(int userId)
        {
            return _context.BankAccounts.Where(ba => ba.userId == userId).OrderByDescending(ba => ba.Credit).ToList();
        }

        public void RemoveAllRemainingRequests()
        {
            _context.StockExchangeBuyRequests.RemoveRange(_context.StockExchangeBuyRequests.ToList());
            _context.StockExchangeSellRequests.RemoveRange(_context.StockExchangeSellRequests.ToList());

            _context.SaveChanges();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool CompanyExists(string companyName)
        {
            return _context.Companies.Any(c => c.Name == companyName);
        }

        private bool BankAccountHasEnoughMoney(string accountNumber, uint numberOfStocks, uint offerPerStock)
        {
            return _context.BankAccounts.First(ba => ba.AccountNumber == accountNumber).Credit >= numberOfStocks * offerPerStock;
        }

        private bool UserHasEnoughStocksFromCompany(string companyName, uint numberOfStocksOffered)
        {
            var companyId = _context.Companies.First(c => c.Name == companyName).Id;

            var userStocksOfCompany = _context.UserCompanyStocks.FirstOrDefault(ucs => ucs.companyId == companyId);

            if (userStocksOfCompany == null) return false;

            return userStocksOfCompany.NumberOfStocks >= numberOfStocksOffered;
        }
    }
}
