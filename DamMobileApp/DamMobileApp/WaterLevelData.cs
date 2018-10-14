using System;
using System.Collections.Generic;
using System.Text;

namespace DamMobileApp
{
    class WaterLevelData
    {
        public DateTime Timestamp { get; set; }
        public float WaterLevel { get; set; }

        public WaterLevelData(DateTime stamp, float level)
        {
            Timestamp = stamp;
            WaterLevel = level;
        }
    }
}
