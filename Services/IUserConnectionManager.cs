namespace PhotocopyRevaluationAppMVC.Services
{
    public interface IUserConnectionManager
    {
        Task AddConnectionAsync(string userId, string connectionId);
        Task RemoveConnectionAsync(string userId);
        public Task<List<string>> GetConnectionIdsAsync(string userId);
    }
}
