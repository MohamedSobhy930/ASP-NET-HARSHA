namespace Configuration.IServices
{
    public interface IFinnhubService
    {
        public  Task<Dictionary<string, object>?> GetStockPrice(string stockName);
    }
}
