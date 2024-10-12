using Microsoft.ML;

namespace PhotocopyRevaluationAppMVC.ML
{
    public class ChatBotModelTrainer
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private readonly string _modelPath;

        public ChatBotModelTrainer(MLContext mlContext, ITransformer model, string modelPath)
        {
            _mlContext = mlContext;
            _model = model;
            _modelPath = modelPath;
        }
        public ChatBotModelTrainer(string dataPath)
        {

        }
        public ChatBotModelTrainer(string dataPath, string modelPath)
        {
            _mlContext = new MLContext();
            _modelPath = modelPath;

            // Load model if exists, else train
            if (File.Exists(_modelPath))
            {
                LoadModel();
            }
            else
            {
                TrainModel(dataPath, Get_mlContext());
                SaveModel();
            }
        }
        //Integrate the Model into Your Chatbot
        //public string PredictIntent(string userMessage, float confidenceThreshold = 0.5f)
        //{
        //    //var predictionEngine = _mlContext.Model.CreatePredictionEngine<ChatbotInput, ChatbotPrediction>(LoadModelAsync());
        //    //var input = new ChatbotInput { Message = message };
        //    //var result = predictionEngine.Predict(input);

        //    //return result.PredictedIntent; // Return the predicted intent

        //    // Create a prediction engine
        //    var predictionEngine = _mlContext.Model.CreatePredictionEngine<ChatbotInput, ChatbotPrediction>(LoadModelAsync());

        //    // Create input data
        //    var inputData = new ChatbotInput { InputText = userMessage };

        //    // Make prediction
        //    var prediction = predictionEngine.Predict(inputData);

        //    // Confidence handling
        //    var score = prediction.Score.Max();
        //    if (score >= confidenceThreshold)
        //    {
        //        return prediction.Intent;
        //    }
        //    else
        //    {
        //        return "fallback"; // handle unclear intent
        //    }
        //}
      
        public ITransformer LoadModelAsync()
        {
            //To load the model later:
            ITransformer loadedModel;
            loadedModel = _mlContext.Model.Load("path/to/save/model.zip", out var inputSchema);
            return loadedModel;
        }
        private MLContext Get_mlContext()
        {
            return _mlContext;
        }
        private ITransformer TrainModel(string dataPath, MLContext _mlContext)
        {
            //// Load the training data
            //IDataView dataView = _mlContext.Data.LoadFromTextFile<ChatbotInput>(dataPath, separatorChar: ',', hasHeader: true);

            //// Define the training pipeline
            //var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(ChatbotInput.Intent))
            //    .Append(_mlContext.Transforms.Text.FeaturizeText("TextFeatures", options: new Microsoft.ML.Transforms.Text.TextFeaturizingEstimator.Options
            //    {
            //        WordFeatureExtractor = new Microsoft.ML.Transforms.Text.WordBagEstimator.Options
            //        {
            //            NgramLength = 2, // Use bi-grams for more context
            //            UseAllLengths = true
            //        },
            //        CharFeatureExtractor = new Microsoft.ML.Transforms.Text.WordBagEstimator.Options
            //        {
            //            NgramLength = 3, // Use tri-grams
            //            UseAllLengths = true
            //        }
            //    }, inputColumnNames: nameof(ChatbotInput.InputText)))
            //    .Append(estimator: _mlContext.Transforms.Text.NormalizeLpNorm("Features", "TextFeatures"))
            //    .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            //    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "Label"))
            //    .Append(_mlContext.Transforms.CopyColumns("Score", "Features"))
            //    .Append(_mlContext.Transforms.NormalizeMinMax("Score"));

            //// Train the model
            //Console.WriteLine("Training the model...");
            //_model = pipeline.Fit(dataView);

            //Console.WriteLine("Model training completed.");

            var context = new MLContext();

            // Load data
            var trainingData = context.Data.LoadFromTextFile<ChatData>(dataPath, separatorChar: ',');

            // Split the data into training and testing sets
            var trainTestSplit = context.Data.TrainTestSplit(trainingData, testFraction: 0.2);

            // Define the pipeline
            var pipeline = context.Transforms.Text.FeaturizeText("Features", nameof(ChatData.Message))
                .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(context.Transforms.Conversion.MapValueToKey("PredictedLabel"));

            // Train the model
            var model = pipeline.Fit(trainTestSplit.TrainSet);

            // Evaluate the model
            var predictions = model.Transform(trainTestSplit.TestSet);
            var metrics = context.MulticlassClassification.Evaluate(predictions, "Label", "PredictedLabel");

            Console.WriteLine($"Log-loss: {metrics.LogLoss}");
            Console.WriteLine($"Micro-Accuracy: {metrics.MicroAccuracy}");
            Console.WriteLine($"Macro-Accuracy: {metrics.MacroAccuracy}");

            // Save the model
            context.Model.Save(model, trainTestSplit.TrainSet.Schema, "ChatBotModel.zip");

            //Save and Load the Model
            context.Model.Save(model, trainingData.Schema, "path/to/save/model.zip");

            return model;
        }

        // Evaluate model with cross-validation
        //public void EvaluateModel(string dataPath)
        //{
        //    IDataView dataView = _mlContext.Data.LoadFromTextFile<ChatbotInput>(dataPath, separatorChar: ',', hasHeader: true);
        //    var cvResults = _mlContext.MulticlassClassification.CrossValidate(dataView, pipeline, numberOfFolds: 5);

        //    foreach (var result in cvResults)
        //    {
        //        Console.WriteLine($"Fold: {result.Fold}, Metrics: Accuracy={result.Metrics.MacroAccuracy}");
        //    }
        //}

        private void SaveModel()
        {
            Console.WriteLine("Saving model...");
            using (var fs = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                _mlContext.Model.Save(_model, null, fs);
            }
        }

        private void LoadModel()
        {
            Console.WriteLine("Loading model from disk...");
            using (var fs = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _model = _mlContext.Model.Load(fs, out _);
            }
        }

        // Incremental retraining
        //public void RetrainModel(string newDataPath)
        //{
        //    Console.WriteLine("Retraining the model with new data...");

        //    // Load the new data
        //    IDataView newData = _mlContext.Data.LoadFromTextFile<ChatbotInput>(newDataPath, separatorChar: ',', hasHeader: true);

        //    // Apply the existing pipeline to the new data for incremental training
        //    var newModel = _model.Fit(newData);

        //    // Replace the old model with the updated one
        //    _model = newModel;
        //    SaveModel();
        //}
    }
}
