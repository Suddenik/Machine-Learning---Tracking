// This file was auto-generated by ML.NET Model Builder. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.ML;
using Prediction.Model;

namespace Prediction.Model
{
    public class ConsumeModel
    {
        public static ModelOutput Predict(ModelInput input)
        {
            MLContext mlContext = new MLContext();

            string modelPath = @"C:\Users\Patryk\Desktop\Machine Learning\FINAL_VER\Prediction\Prediction.Model\MLModel.zip";
            
            ITransformer mlModel = mlContext.Model.Load(modelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            //make prediction on input data
            ModelOutput result = predEngine.Predict(input);
            return result;
        }
    }
}
