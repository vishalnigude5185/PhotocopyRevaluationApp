namespace PhotocopyRevaluationAppMVC.Services
{
    public interface IExportDataService
    {
        Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> Collection);
        Task<byte[]> ExportDataToPdfAsync();    
    }
}
