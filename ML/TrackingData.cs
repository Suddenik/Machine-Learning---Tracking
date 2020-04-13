using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace ML
{//SHIPMENT_CREATEDATE;FIRST_EVENT;LAST_EVENT;RECEIVER_ZIP;RECEIVER_COUNTRY_IOS2;SENDER_ZIP;SENDER_COUNTRY_IOS2;SHIPMENT_WEIGHT;CONTRACT_TYPE;XLIDENTIFIER
    class TrackingData
    {
        [LoadColumn(0)]
        public long SHIPMENT_CREATEDATE { get; set; }

        [LoadColumn(1)]
        public long FIRST_EVENT { get; set; }

        [LoadColumn(2)]
        public long LAST_EVENT { get; set; }

        [LoadColumn(3)]
        public string RECEIVER_ZIP { get; set; }

        [LoadColumn(4)]
        public string RECEIVER_COUNTRY_IOS2 { get; set; }

        [LoadColumn(5)]
        public string SENDER_ZIP { get; set; }

        [LoadColumn(6)]
        public string SENDER_COUNTRY_IOS2 { get; set; }

        [LoadColumn(7)]
        public string SHIPMENT_WEIGHT { get; set; }

        [LoadColumn(8)]
        public string CONTRACT_TYPE { get; set; }
        
        [LoadColumn(9)]
        public string XLIDENTIFIER { get; set; }
    }
}
