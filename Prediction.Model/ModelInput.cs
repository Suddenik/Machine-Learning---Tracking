using Microsoft.ML.Data;

namespace Prediction.Model
{
    public class ModelInput
    {
        [ColumnName("TIME_BEFORE_COURIER_GET_PACKAGE"), LoadColumn(0)]
        public float TIME_BEFORE_COURIER_GET_PACKAGE { get; set; }


        [ColumnName("TIME_TO_DELIVER"), LoadColumn(1)]
        public float TIME_TO_DELIVER { get; set; }


        [ColumnName("ZIP_RECEIVER"), LoadColumn(2)]
        public float ZIP_RECEIVER { get; set; }


        [ColumnName("ZIP_SENDER"), LoadColumn(3)]
        public float ZIP_SENDER { get; set; }


        [ColumnName("DISTANCE"), LoadColumn(4)]
        public float DISTANCE { get; set; }


    }
}
