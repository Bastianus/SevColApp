namespace SevColApp.Repositories
{
    public interface IGamemasterStockExchangeRepository
    {
        float GetGlobalTrend();
        float GetGlobalVolatility();
        float GetCompanyTrend(int companyId);
        float GetCompanyVolatility(int companyId);
        void SetGlobalTrend(float value);
        void SetGlobalVolatility(float value);
        void SetCompanyTrend(int companyId, float value);
        void SetCompanyVolatility(int companyId, float value);
    }
}
