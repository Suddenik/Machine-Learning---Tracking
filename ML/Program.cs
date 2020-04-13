using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using static Microsoft.ML.Transforms.ValueToKeyMappingEstimator;

namespace ML
{
    class Program
    {
        public static void Main(string[] args)
        {
            var context = new MLContext(seed:1);

            var data = context.Data.LoadFromTextFile<TrackingData>
                ("C:\\Users\\Patryk\\Desktop\\ML_DATABASE\\tracking_dataOut_TEST.csv",
                hasHeader: true,
                separatorChar: ';');

            


            /*
            var context = new MLContext();

            var data = context.Data.LoadFromTextFile<TrackingData>
                ("C:\\Users\\Patryk\\Desktop\\ML_DATABASE\\tracking_dataOut_TEST.csv",
                hasHeader:true,
                separatorChar: ';');

            var pipeline = context.Transforms.Conversion.MapKeyToValue(new[] {
                new InputOutputColumnPair("RECEIVER_ZIP_Category","RECEIVER_ZIP"),
                new InputOutputColumnPair("RECEIVER_COUNTRY_IOS2_Category","RECEIVER_COUNTRY_IOS2"),
                new InputOutputColumnPair("SENDER_ZIP_Category","SENDER_ZIP"),
                new InputOutputColumnPair("SENDER_COUNTRY_IOS2_Category","SENDER_COUNTRY_IOS2"),
                new InputOutputColumnPair("SHIPMENT_WEIGHT_Category","SHIPMENT_WEIGHT"),
                new InputOutputColumnPair("CONTRACT_TYPE_Category","CONTRACT_TYPE"),
                new InputOutputColumnPair("XLIDENTIFIER_Category","XLIDENTIFIER"),
            });

            IDataView transformedData = pipeline.Fit(data).Transform(data);
            */
        }
    }
}
