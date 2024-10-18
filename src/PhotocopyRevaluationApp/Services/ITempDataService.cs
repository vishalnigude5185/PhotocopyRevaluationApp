namespace PhotocopyRevaluationApp.Services {
    public interface ITempDataService {
        string GetTempData(string key);
        void SetTempData(string key, string value);
    }
}
