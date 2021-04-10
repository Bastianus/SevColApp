using SevColApp.Models;
using System;

namespace SevColApp.Hosted_service
{
    public interface IStocksExchanger
    {
        void ExchangeStocksForCompany(Company company, DateTime time);
        void RemoveAllRemainingRequests();
    }
}
