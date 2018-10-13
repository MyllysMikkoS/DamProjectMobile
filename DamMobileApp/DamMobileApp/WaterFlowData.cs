using System;
using System.Collections.Generic;
using System.Text;

namespace DamMobileApp
{
    public class WaterFlowData
    {
        public DateTime Timestamp { get; set; }
        public float WaterFlow { get; set; }

        public WaterFlowData(DateTime stamp, float flow)
        {
            Timestamp = stamp;
            WaterFlow = flow;
        }
    }
}
