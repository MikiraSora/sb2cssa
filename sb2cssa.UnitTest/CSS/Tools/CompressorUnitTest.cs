using Microsoft.VisualStudio.TestTools.UnitTesting;
using sb2cssa.CSS;
using sb2cssa.CSS.Animation;
using sb2cssa.CSS.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.UnitTest.CSS.Tools
{
    [TestClass]
    [TestCategory(nameof(CompressorUnitTest))]
    public class CompressorUnitTest
    {
        [TestMethod]
        public void Test()
        {
            var t1 = new ProgressiveFrame(0f, new List<Property>() {
                new Property("top","200"),
                new Property("left","100"),
            });

            var t2 = new ProgressiveFrame(0f, new List<Property>() {
                new Property("top","200"),
                new Property("left","100"),
            });

            var t3 = new ProgressiveFrame(1f, new List<Property>() {
                new Property("top","200"),
                new Property("left","100"),
            });

            Assert.IsTrue(Compressor.ProgressiveFrameValueComparer.Default.Equals(t1, t2));
            Assert.IsFalse(Compressor.ProgressiveFrameValueComparer.Default.Equals(t3, t2));

            ProgressiveKeyFrames frames = new ProgressiveKeyFrames("test");

            BuildTestTimeline(frames);

            var before_count = frames.Timeline.Count;

            Compressor.Compress(frames);

            var after_count = frames.Timeline.Count;

            Assert.IsTrue(after_count < before_count);
        }

        private void BuildTestTimeline(ProgressiveKeyFrames frames)
        {
            frames.Timeline.Add(new ProgressiveFrame(0f, new List<Property>() {
                new Property("top","200"),
                new Property("left","100"),
            }));

            frames.Timeline.Add(new ProgressiveFrame(0.2f, new List<Property>() {
                new Property("top","0"),
                new Property("left","100"),
            }));

            frames.Timeline.Add(new ProgressiveFrame(0.4f, new List<Property>() {
                new Property("top","100"),
                new Property("left","200"),
            }));

            frames.Timeline.Add(new ProgressiveFrame(0.4f, new List<Property>() {
                new Property("top","100"),
                new Property("left","200"),
            }));

            frames.Timeline.Add(new ProgressiveFrame(0.4f, new List<Property>() {
                new Property("top","100"),
                new Property("left","200"),
            }));

            frames.Timeline.Add(new ProgressiveFrame(0.6f, new List<Property>() {
                new Property("top","100"),
                new Property("left","1000"),
            }));

            frames.Timeline.Add(new ProgressiveFrame(0.6f, new List<Property>() {
                new Property("top","100"),
                new Property("left","100"),
            }));

            frames.Timeline.Add(new ProgressiveFrame(0.8f, new List<Property>() {
                new Property("top","100"),
                new Property("left","100"),
            }));

            frames.Timeline.Add(new ProgressiveFrame(1f, new List<Property>() {
                new Property("top","0"),
                new Property("left","0"),
            }));
        }
    }
}
