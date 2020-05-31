using Microsoft.ML.Data;

namespace Prediction.Model
{
    public class ModelInput2
    {

        public ModelInput2(string line)
        {
            string[] parts = line.Split(';');
            TIME_BEFORE_COURIER_GET_PACKAGE = float.Parse(parts[0]);
            TIME_TO_DELIVER = float.Parse(parts[1]);
            ZIP_RECEIVER = float.Parse(parts[2]);
            ZIP_SENDER = float.Parse(parts[3]);
            parts[4] = parts[4].Replace('.', ',');
            DISTANCE = float.Parse(parts[4]);

        }

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
