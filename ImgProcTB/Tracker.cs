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
        [Flags]
        public enum OLFlags
        {
            Points = 0x01,
            Center = 0x02,
            Text = 0x04,
            All = 0xff
        }

        public int maxPoints { get; set; }
        public Size pyrWinSize { get; set; }
        public int pyrLevel { get; set; }
        public int maxIter { get; set; }
        public double eps { get; set; }
        public bool Active{ get; set; }

        public Point searchLocation { get; set; }
        public Size searchSize { get; set; }

        public OLFlags olflags { get; set; }

        PointF[] prevPoints;
        private MKeyPoint[] keyPoints;
        Mat PrevImg;
        public Tracker()
        {
            Active = true;
            olflags = OLFlags.All;
            maxPoints = 50;
            pyrLevel = 3;
            maxIter = 20;
            eps = 1;
            pyrWinSize = new Size(10, 10);
            searchLocation = new Point(200, 200);
            searchSize = new Size(600, 600);
        }

        public int Track(Bitmap bm)
        {
            if (!Active)
                return 0;


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

            PrevImg = m.Clone();

            CvInvoke.Rectangle(m, new Rectangle(searchLocation, searchSize), new MCvScalar(100, 100, 255, 233));
             
            prevPoints = prevPoints.Where((p, idx) => status[idx] == 1).ToArray();
            nextPts = nextPts.Where((p, idx) => status[idx] == 1).ToArray();

            double meanX = nextPts.Sum(p => p.X)/nextPts.Count();
            double meanY = nextPts.Sum(p => p.Y)/nextPts.Count();

            for (int i=0; i< nextPts.Count(); i++)
            //foreach (var np in nextPts)
            {
                PointF pp = prevPoints[i];
                PointF np = nextPts[i];
                if (olflags.HasFlag(OLFlags.Points))
                {
                    CvInvoke.Circle(m, new Point((int)np.X, (int)np.Y), 5, new MCvScalar(100, 100, 255));
                    CvInvoke.Line(m, new Point((int)pp.X, (int)pp.Y), new Point((int)np.X, (int)np.Y), new MCvScalar(100, 255, 100), 1);
                }
                double dist = Math.Sqrt(Math.Pow(np.X - pp.X, 2) + Math.Pow(np.Y - pp.Y, 2));

                if (olflags.HasFlag(OLFlags.Text))
                {
                    CvInvoke.PutText(m, i.ToString(), new Point((int)np.X, (int)np.Y), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 1, new MCvScalar(100, 100, 255), 1);
                    string text = i.ToString() + ": " + dist.ToString("0.00");// + ", " + err[i].ToString("#.##");

                    CvInvoke.PutText(m, text, new Point(0, i * 15), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 1, new MCvScalar(100, 100, 255));
                }
            }

            if (olflags.HasFlag(OLFlags.Center))
            {
                CvInvoke.Circle(m, new Point((int)meanX, (int)meanY), 7, new MCvScalar(255, 255, 100));
            }

            

            bm.UnlockBits(bmpData);

            prevPoints = nextPts;
            
            
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
