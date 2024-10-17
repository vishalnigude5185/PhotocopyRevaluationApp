using Microsoft.ML.Data;

namespace PhotocopyRevaluationApp.ML {
    public class ChatData {
        [LoadColumn(0)]
        public string Message;

        [LoadColumn(1)]
        public string Intent;
    }
}
