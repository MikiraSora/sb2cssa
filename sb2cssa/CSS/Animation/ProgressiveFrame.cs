using System.Collections.Generic;

namespace sb2cssa.CSS.Animation
{
    public class ProgressiveFrame
    {
        public ProgressiveFrame() { }

        public ProgressiveFrame(float time,List<Property> props)
        {
            NormalizeTime = time;
            ChangedProperties = props;
        }

        public float NormalizeTime { get; set; }
        public List<Property> ChangedProperties { get; set; }

        public override string ToString()
        {
            return $"time : {NormalizeTime}";
        }
    }
}