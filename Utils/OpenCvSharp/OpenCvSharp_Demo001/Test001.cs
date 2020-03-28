using OpenCvSharp;

namespace OpenCvSharp_Demo001
{
    public class Test001
    {
        public void test001()
        {
            Mat source = new Mat(@"1.jpg", ImreadModes.Color);
            Cv2.ImShow("Demo", source);
            Cv2.WaitKey(0);
        }
    }
}
