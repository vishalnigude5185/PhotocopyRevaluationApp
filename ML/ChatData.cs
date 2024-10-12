using Microsoft.ML.Data;

namespace PhotocopyRevaluationAppMVC.ML
{
    public class ChatData
    {
        [LoadColumn(0)]
        public string Message;

        [LoadColumn(1)]
        public string Intent;
    }
 }
