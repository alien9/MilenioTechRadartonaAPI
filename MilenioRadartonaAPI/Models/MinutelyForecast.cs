﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AWSLambdaGetTempo.Model
{
    [DataContract]
    public class MinutelyForecast
    {
        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "data")]
        public IList<MinuteDataPoint> Minutes { get; set; }
    }
}
