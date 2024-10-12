using Microsoft.ML.Data;

namespace PhotocopyRevaluationAppMVC.ML
{
    public class ChatbotPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedIntent;

        [ColumnName("Probability")]
        public float[] Probability;

        [ColumnName("Score")]
        public int Score { get; internal set; }

        public string Intent { get; internal set; }
    }
}
