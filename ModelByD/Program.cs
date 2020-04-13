using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace OgarniatorDanych
{
    class Program
    {

        private static string _appPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        private static string _trainDataPath => Path.Combine(_appPath, "Data", "tracking_dataOut3100.csv");
        private static string _testDataPath => Path.Combine(_appPath, "Data", "tracking_dataOut3100.csv");

        public static IEstimator<ITransformer> ProcessData()
        {
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "LAST_EVENT", outputColumnName: "Label")
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "SHIPMENT_CREATEDATE", outputColumnName: "SHIPMENT_CREATEDATEFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "FIRST_EVENT", outputColumnName: "FIRST_EVENTFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "RECEIVER_ZIP", outputColumnName: "RECEIVER_ZIPFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "RECEIVER_COUNTRY_IOS2", outputColumnName: "RECEIVER_COUNTRY_IOS2Featurized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "SENDER_ZIP", outputColumnName: "SENDER_ZIPFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "SENDER_COUNTRY_IOS2", outputColumnName: "SENDER_COUNTRY_IOS2Featurized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "SHIPMENT_WEIGHT", outputColumnName: "SHIPMENT_WEIGHTFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "CONTRACT_TYPE", outputColumnName: "CONTRACT_TYPEFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "XLIDENTIFIER", outputColumnName: "XLIDENTIFIERFeaturized"))
                .Append(_mlContext.Transforms.Concatenate("Features", "SHIPMENT_CREATEDATEFeaturized", "FIRST_EVENTFeaturized", 
                "RECEIVER_ZIPFeaturized", "RECEIVER_COUNTRY_IOS2Featurized", "SENDER_ZIPFeaturized", "SENDER_COUNTRY_IOS2Featurized", 
                "SHIPMENT_WEIGHTFeaturized", "CONTRACT_TYPEFeaturized","XLIDENTIFIERFeaturized"))
                .AppendCacheCheckpoint(_mlContext);

            return pipeline;
        }

        public static IEstimator<ITransformer> BuildAndTrainModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
        {
            var trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            _trainedModel = trainingPipeline.Fit(trainingDataView);
            _predEngine = _mlContext.Model.CreatePredictionEngine<Data, Output>(_trainedModel);

            Data issue = new Data()
            {
                SHIPMENT_CREATEDATE = "2020-02-17 07:14:08",
                FIRST_EVENT = "2020-02-17 21:46:00",
                RECEIVER_ZIP = "60035",
                RECEIVER_COUNTRY_IOS2 = "IT",
                SENDER_ZIP = "27049",
                SENDER_COUNTRY_IOS2 = "IT",
                SHIPMENT_WEIGHT = "0.422",
                CONTRACT_TYPE = "SDA Express Courier S.p.A.",
                XLIDENTIFIER = "SDA-01"
                
            };




            var prediction = _predEngine.Predict(issue);

            Console.WriteLine($"=============== Single Prediction just-trained-model - Result: {prediction.LAST_EVENT} ===============");

            return trainingPipeline;


        }

        public static void Evaluate(DataViewSchema trainingDataViewSchema)
        {
            var testDataView = _mlContext.Data.LoadFromTextFile<Data>(_testDataPath, hasHeader: true,separatorChar: ';');

            var testMetrics = _mlContext.MulticlassClassification.Evaluate(_trainedModel.Transform(testDataView));

            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Multi-class Classification model - Test Data     ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       MicroAccuracy:    {testMetrics.MicroAccuracy:0.###}");
            Console.WriteLine($"*       MacroAccuracy:    {testMetrics.MacroAccuracy:0.###}");
            Console.WriteLine($"*       LogLoss:          {testMetrics.LogLoss:#.###}");
            Console.WriteLine($"*       LogLossReduction: {testMetrics.LogLossReduction:#.###}");
            Console.WriteLine($"*************************************************************************************************************");

            SaveModelAsFile(_mlContext, trainingDataViewSchema, _trainedModel);

        }

        private static void SaveModelAsFile(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            mlContext.Model.Save(model, trainingDataViewSchema, _modelPath);
        }

        private static void PredictIssue()
        {
            ITransformer loadedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);

            Data singleIssue = new Data() {
                SHIPMENT_CREATEDATE = "2020-02-14 21:08:08",
                FIRST_EVENT = "2020-02-17 04:32:00",
                RECEIVER_ZIP = "GU167UJ",
                RECEIVER_COUNTRY_IOS2 = "GB",
                SENDER_ZIP = "ST44EX",
                SENDER_COUNTRY_IOS2 = "GB",
                SHIPMENT_WEIGHT = "1.030",
                CONTRACT_TYPE = "Yodel UK",
                XLIDENTIFIER = "YOD-02"
            };

            _predEngine = _mlContext.Model.CreatePredictionEngine<Data, Output>(loadedModel);

            var prediction = _predEngine.Predict(singleIssue);

            Console.WriteLine($"=============== Single Prediction - Result: {prediction.LAST_EVENT} ===============");

        }

        public static void Example()
        {
            _mlContext = new MLContext(seed: 0);

            _trainingDataView = _mlContext.Data.LoadFromTextFile<Data>(_trainDataPath, hasHeader: true, separatorChar: ';');

            var pipeline = ProcessData();

            var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline);

            Evaluate(_trainingDataView.Schema);

            PredictIssue();


        }


        private static string _modelPath => Path.Combine(_appPath, "Models", "model.zip");

        private static MLContext _mlContext;
        private static PredictionEngine<Data, Output> _predEngine;
        private static ITransformer _trainedModel;
        static IDataView _trainingDataView;


        static void Main(string[] args)
        {   
            Example();
        }
    }
}
