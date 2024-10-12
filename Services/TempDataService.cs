using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace PhotocopyRevaluationAppMVC.Services
{
    public class TempDataService : ITempDataService
    {
        private readonly ITempDataDictionary _tempData;

        public TempDataService(ITempDataDictionary tempData)
        {
            _tempData = tempData;
        }

        public string GetTempData(string key)
        {
            return _tempData[key] as string;
        }

        public void SetTempData(string key, string value)
        {
            _tempData[key] = value;
        }
    }
}
