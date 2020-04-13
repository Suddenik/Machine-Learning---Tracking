using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.ML.Data;

namespace OgarniatorDanych
{
    class Data
    {

        //[LoadColumn(0)]
        //public string SHIPMENT_IDENTCODE;

        [LoadColumn(0)]
        public string SHIPMENT_CREATEDATE;

        [LoadColumn(1)]
        public string FIRST_EVENT;

        [LoadColumn(2)]
        public string LAST_EVENT;

        [LoadColumn(3)]
        public string RECEIVER_ZIP;

        
        [LoadColumn(4)]
        public string RECEIVER_COUNTRY_IOS2;

        [LoadColumn(5)]
        public string SENDER_ZIP;

        [LoadColumn(6)]
        public string SENDER_COUNTRY_IOS2;

        [LoadColumn(7)]
        public string SHIPMENT_WEIGHT;

        [LoadColumn(8)]
        public string CONTRACT_TYPE;

        [LoadColumn(9)]
        public string XLIDENTIFIER;
        

    }

    public class Output
    {
        [ColumnName("PredictedLabel")]
        public string LAST_EVENT;
    }
}
