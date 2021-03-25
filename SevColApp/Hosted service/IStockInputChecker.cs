namespace SevColApp.Hosted_service
{
    public interface IStockInputChecker
    {
        void CheckAllSellRequestsAndRemoveInvalids();

        void CheckAllBuyRequestsAndRemoveInvalids();
    }
}
