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
        //private const string DATA_FILEPATH = @"C:\Users\Patryk\Desktop\ML_DATABASE\tracking_dataOut_TEST.csv";
        private const string DATA_FILEPATH = @"C:\Users\Patryk\Desktop\ML_DATABASE\tracking_TEST_2.txt";

        static void Main(string[] args)
        {
            // Create single instance of sample data from first line of dataset for model input
            ModelInput sampleData = CreateSingleDataSample(DATA_FILEPATH);

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);
            
            //konwersja do double bo inaczej notacja wyk�adnicza
            double shipment_create = sampleData.SHIPMENT_CREATEDATE;
            double first_event = sampleData.FIRST_EVENT;
            double receiver_zip = sampleData.RECEIVER_ZIP;
            double receiver_country = sampleData.RECEIVER_COUNTRY_IOS2;
            double sender_zip= sampleData.SENDER_ZIP;
            double sender_country = sampleData.SENDER_COUNTRY_IOS2;
            //double weight= sampleData.SHIPMENT_WEIGHT;
            double actual_last_event = sampleData.LAST_EVENT;
            double predicted_last_event = predictionResult.Score;


            DateTime _zakup = UnixTimeStampToDateTime(shipment_create);
            DateTime _kurier_odebral = UnixTimeStampToDateTime(first_event);
            DateTime _kurier_dostarczyl = UnixTimeStampToDateTime(actual_last_event);
            DateTime _przewidywana_data_dostarczenia = UnixTimeStampToDateTime(predicted_last_event);
            string kraj_odbiorcy = ConvertNumberToCountry(receiver_country.ToString());
            string kraj_nadawcy = ConvertNumberToCountry(sender_country.ToString());

            Console.WriteLine("Using model to make single prediction -- Comparing actual LAST_EVENT with predicted LAST_EVENT from sample data...\n\n");
            Console.WriteLine($"Data zakupu: {_zakup}");
            Console.WriteLine($"Data odbioru przez kuriera: {_kurier_odebral}");
            Console.WriteLine($"Kod pocztowy odbiorcy: {receiver_zip}");
            Console.WriteLine($"Kraj odbiorcy: {kraj_odbiorcy}");
            Console.WriteLine($"Kod pocztowy nadawcy: {sender_zip}");
            Console.WriteLine($"Kraj nadawcy: {kraj_nadawcy}");
            Console.WriteLine($"\n\nPRAWDZIWA data dostarczenia: {_kurier_dostarczyl} \nPRZEWIDYWANA data dostarczenia: {_przewidywana_data_dostarczenia}\n\n");
            Console.WriteLine("=============== Naci�nij cokolwiek �eby wyj�� ===============");
            Console.ReadKey();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private static string ConvertNumberToCountry(string number)
        {
            string ASCII = "", first_part = "", second_part = "";
            int counter = 0, unicode;
            bool tryParse;
            char character;

            foreach (char c in number)
            {
                if (counter < 2)
                {
                    first_part += c;
                }
                else if (counter >= 2)
                {
                    second_part += c;
                }
                counter++;
            }
            tryParse = Int32.TryParse(first_part, out unicode);
            if (tryParse)
            {
                character = (char)unicode;
                ASCII += character.ToString();
            }
            else
                Console.WriteLine("B��d konwersji");

            tryParse = Int32.TryParse(second_part, out unicode);
            if (tryParse)
            {
                character = (char)unicode;
                ASCII += character.ToString();
            }
            else
                Console.WriteLine("B��d konwersji");

            return ASCII;
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
