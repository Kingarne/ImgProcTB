using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.Features2D;

using AVT.VmbAPINET;
 
namespace ImgProcTB
{
    public class Tracker
    {
        public int maxPoints { get; set; }
        public Size pyrWinSize { get; set; }
        public int pyrLevel { get; set; }
        public int maxIter { get; set; }
        public double eps { get; set; }

        public Point searchLocation { get; set; }
        public Size searchSize { get; set; }

        PointF[] prevPoints;
        private MKeyPoint[] keyPoints;
        Mat PrevImg;
        public Tracker()
        {
            maxPoints = 50;
            pyrLevel = 3;
            maxIter = 20;
            eps = 1;
            pyrWinSize = new Size(10, 10);
            searchLocation = new Point(200, 200);
            searchSize = new Size(200, 200);
        }

        public int Track(Bitmap bm)
        {

            Rectangle rect = new Rectangle(0, 0, bm.Width, bm.Height);
            BitmapData bmpData = bm.LockBits(rect, ImageLockMode.ReadWrite, bm.PixelFormat);
           
            Mat m = new Mat(bm.Height, bm.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 3, bmpData.Scan0, bmpData.Stride);


            PointF[] nextPts = new PointF[prevPoints.Count()];
            byte[] status = new byte[prevPoints.Count()];
            float[] err = new float[prevPoints.Count()];
            CvInvoke.CalcOpticalFlowPyrLK(PrevImg, m, prevPoints, pyrWinSize, pyrLevel, new MCvTermCriteria(maxIter, eps),out nextPts, out status, out err);


            // foreach (var kp in keyPoints)
            //{
            //     CvInvoke.Circle(m, new Point((int)kp.Point.X+searchROI.Left, (int)kp.Point.Y+searchROI.Top), 5, new MCvScalar(100, 100, 255));
            //}            

            CvInvoke.Rectangle(m, new Rectangle(searchLocation, searchSize), new MCvScalar(100, 100, 255, 233));
            int i = 0;
            foreach (var np in nextPts)
            {
                CvInvoke.Circle(m, new Point((int)np.X, (int)np.Y), 5, new MCvScalar(100, 100, 255));

                
            }


            bm.UnlockBits(bmpData);

            prevPoints = nextPts;
            PrevImg = m;
            
            return 0;
        }

        public int Init(Bitmap bm)
        {
            GFTTDetector gftt = new GFTTDetector(maxPoints, 0.1, 1, 3, false, 0.04);
            Rectangle rect = new Rectangle(0, 0, bm.Width, bm.Height);
            BitmapData bmpData = bm.LockBits(rect, ImageLockMode.ReadWrite, bm.PixelFormat);

            Mat m = new Mat(bm.Height, bm.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 3, bmpData.Scan0, bmpData.Stride);

            Rectangle roi = new Rectangle(searchLocation, searchSize);
           // MKeyPoint[] keyPoints;
            keyPoints = gftt.Detect(new Mat(m, roi));
            bm.UnlockBits(bmpData);
            PrevImg = m;

            prevPoints= new PointF[keyPoints.Count()];
            for (int j = 0; j < keyPoints.Count(); j++)
            {
                prevPoints[j] = new PointF(keyPoints[j].Point.X + roi.Left, keyPoints[j].Point.Y + roi.Top);
            }

            /*            unsafe
                        {
                            fixed (byte* pArray = fr.Buffer)
                            {
                                //IntPtr = pArray;
                                Mat m = new Mat((int)fr.Height, (int)fr.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 3, (IntPtr)pArray, (int)fr.Width*3);
                                keyPoints = gftt.Detect(m);
                                foreach (var kp in keyPoints)
                                {
                                    CvInvoke.Circle(m, new Point((int)kp.Point.X, (int)kp.Point.Y), 5, new MCvScalar(100, 200, 200));

                                }

                                }
                            }
                            */





            //CvInvoke.CalcOpticalFlowPyrLK()
            return 0;
        }

    }

}
