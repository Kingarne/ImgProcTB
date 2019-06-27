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
    public class TrackerTM
    {
        [Flags]
        public enum OLFlags
        {
            Points = 0x01,
            Center = 0x02,
            Text = 0x04,
            All = 0xff
        }

        public bool Active { get; set; }
        public Point searchLocation { get; set; }
        public Size searchSize { get; set; }
        public int searchDist { get; set; }

        public OLFlags olflags { get; set; }

        //PointF[] prevPoints;        
        Mat TemplateImg;
        Rectangle TmplRoi;

        public TrackerTM()
        {
            Active = true;
            olflags = OLFlags.All;
            searchLocation = new Point(200, 200);
            searchSize = new Size(200, 200);
            searchDist = 20;
        }

        public int Track(Bitmap bm)
        {
            if (!Active)
                return 0;


            Rectangle rect = new Rectangle(0, 0, bm.Width, bm.Height);
            BitmapData bmpData = bm.LockBits(rect, ImageLockMode.ReadWrite, bm.PixelFormat);

            Mat m = new Mat(bm.Height, bm.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 3, bmpData.Scan0, bmpData.Stride);

            Rectangle searchRoi = new Rectangle();// TmplRoi.Location, TmplRoi.Size);
            searchRoi.Location = new Point(TmplRoi.X - searchDist, TmplRoi.Y - searchDist);
            searchRoi.Size = new Size(TmplRoi.Width+2*searchDist, TmplRoi.Height+2*searchDist);

            int resultCols = m.Cols - searchRoi.Width + 1;
            int resultRows = m.Rows - searchRoi.Height + 1;

            Mat res = new Mat(resultCols, resultRows, Emgu.CV.CvEnum.DepthType.Cv32S, 1);

            Mat t = new Mat(TemplateImg, TmplRoi);
            Mat s = new Mat(m, searchRoi);
            CvInvoke.Imshow("template", t);
            CvInvoke.Imshow("search", s);
            CvInvoke.MatchTemplate(s, t, res, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

            Point minPos = new Point();
            Point maxPos= new Point();
            double minVal=0, maxVal=0;
            CvInvoke.MinMaxLoc(res, ref minVal, ref maxVal, ref minPos, ref maxPos);
            CvInvoke.Imshow("res", res);
            maxPos.X += searchRoi.X;
            maxPos.Y += searchRoi.Y;

            TmplRoi.Location = new Point(maxPos.X, maxPos.Y);
            TemplateImg = m.Clone();

            CvInvoke.Circle(m, new Point((int)maxPos.X, (int)maxPos.Y), 7, new MCvScalar(255, 255, 100));

            CvInvoke.Rectangle(m, searchRoi, new MCvScalar(100, 255, 233));
            CvInvoke.Rectangle(m, TmplRoi, new MCvScalar(100, 255, 233));
            bm.UnlockBits(bmpData);


            return 0;
        }

        public int Init(Bitmap bm)
        {
            Rectangle rect = new Rectangle(0, 0, bm.Width, bm.Height);
            BitmapData bmpData = bm.LockBits(rect, ImageLockMode.ReadWrite, bm.PixelFormat);

            Mat m = new Mat(bm.Height, bm.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 3, bmpData.Scan0, bmpData.Stride);


            TemplateImg = m.Clone();
            TmplRoi = new Rectangle(searchLocation, searchSize);

            bm.UnlockBits(bmpData);


            return 0;            
        }

    }
}
