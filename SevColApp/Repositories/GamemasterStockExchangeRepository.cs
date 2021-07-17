using SevColApp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevColApp.Repositories
{
    public class GamemasterStockExchangeRepository : IGamemasterStockExchangeRepository
    {
        private readonly SevColContext _context;

        public GamemasterStockExchangeRepository(SevColContext context)
        {
            _context = context;
        }

        public float GetGlobalTrend()
        {
            return _context.Global.Find(1).MarketTrendFactor;
        }

        public float GetGlobalVolatility()
        {
            return _context.Global.Find(1).MarketVolatility;
        }

        public float GetCompanyTrend(int companyId)
        {
            return _context.Companies.Find(companyId).CompanyTrendFactor;
        }

        public float GetCompanyVolatility(int companyId)
        {
            return _context.Companies.Find(companyId).CompanyVolatility;
        }

        public void SetGlobalTrend(float value)
        {
            _context.Global.Find(1).MarketTrendFactor = value;

            _context.SaveChanges();
        }

        public void SetGlobalVolatility(float value)
        {
            _context.Global.Find(1).MarketVolatility = value;

            _context.SaveChanges();
        }

        public void SetCompanyTrend(int companyId, float value)
        {
            _context.Companies.Find(companyId).CompanyTrendFactor = value;

            _context.SaveChanges();
        }

        public void SetCompanyVolatility(int companyId, float value)
        {
            _context.Companies.Find(companyId).CompanyVolatility = value;

            _context.SaveChanges();
        }

        
    }
}
