// This file was auto-generated by ML.NET Model Builder. 

using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using ML_TrekkingTeamML.Model;

namespace ML_TrekkingTeamML.ConsoleApp
{
    class Program
    {
        //Dataset to use for predictions 
        private const string DATA_FILEPATH = @"C:\Users\Patryk\Desktop\ML_DATABASE\tracking_dataOut_TEST.csv";

        static void Main(string[] args)
        {
            // Create single instance of sample data from first line of dataset for model input
            ModelInput sampleData = CreateSingleDataSample(DATA_FILEPATH);

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            Console.WriteLine("Using model to make single prediction -- Comparing actual LAST_EVENT with predicted LAST_EVENT from sample data...\n\n");
            Console.WriteLine($"SHIPMENT_CREATEDATE: {sampleData.SHIPMENT_CREATEDATE}");
            Console.WriteLine($"FIRST_EVENT: {sampleData.FIRST_EVENT}");
            Console.WriteLine($"RECEIVER_ZIP: {sampleData.RECEIVER_ZIP}");
            Console.WriteLine($"RECEIVER_COUNTRY_IOS2: {sampleData.RECEIVER_COUNTRY_IOS2}");
            Console.WriteLine($"SENDER_ZIP: {sampleData.SENDER_ZIP}");
            Console.WriteLine($"SENDER_COUNTRY_IOS2: {sampleData.SENDER_COUNTRY_IOS2}");
            Console.WriteLine($"SHIPMENT_WEIGHT: {sampleData.SHIPMENT_WEIGHT}");
            Console.WriteLine($"\n\nActual LAST_EVENT: {sampleData.LAST_EVENT} \nPredicted LAST_EVENT: {predictionResult.Score}\n\n");
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }

        // Change this code to create your own sample data
        #region CreateSingleDataSample
        // Method to load single row of dataset to try a single prediction
        private static ModelInput CreateSingleDataSample(string dataFilePath)
        {
            // Create MLContext
            MLContext mlContext = new MLContext();

            // Load dataset
            IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                                            path: dataFilePath,
                                            hasHeader: true,
                                            separatorChar: ';',
                                            allowQuoting: true,
                                            allowSparse: false);

            // Use first line of dataset as model input
            // You can replace this with new test data (hardcoded or from end-user application)
            ModelInput sampleForPrediction = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false)
                                                                        .First();
            return sampleForPrediction;
        }
        #endregion
    }
}