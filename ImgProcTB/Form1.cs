using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

using AVT.VmbAPINET;

namespace ImgProcTB
{
    public partial class Form1 : Form
    {
        ControlForm controlForm;
        public Tracker tracker;
        public TrackerTM trackerTM;

        VimbaLib vimba;
        VideoCapture capture;
        public bool m_init = true;

        public Form1()
        {
            InitializeComponent();

            tracker = new Tracker();
            trackerTM = new TrackerTM();



            controlForm = new ControlForm(this);
            controlForm.Show(this);
            controlForm.propertyGrid.SelectedObject = trackerTM;

           // capture = new VideoCapture(0, VideoCapture.API.Pvapi);// "K:\\Projects\\Util\\ImgProcTB\\video.avi"); //create a camera captue
           //            capture.ImageGrabbed += Capture_ImageGrabbed;
           //    capture.Start();
        }

        private void OnNewFrame(Frame frame)
        {
            Invoke(new Camera.OnFrameReceivedHandler(OnNewFrame2), frame);


        }

        private void OnNewFrame2(Frame frame)
        {
            



           // Console.WriteLine("Frame status complete");
            Bitmap bitmap = new Bitmap((int)frame.Width, (int)frame.Height, PixelFormat.Format24bppRgb);
            frame.Fill(ref bitmap);

            ImageProcess(frame, bitmap);

            imgBox.Image = bitmap;
        }

        private void ImageProcess(Frame frame, Bitmap bitmap)
        {
            if (m_init == true)
            {
                //tracker.Init(bitmap);
                trackerTM.Init(bitmap);
                m_init = false;
            }

            trackerTM.Track(bitmap);
            //tracker.Track(bitmap);
        }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Mat m = new Mat();
            capture.Retrieve(m);
            imgBox.Image = m.Bitmap;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            vimba.StopCamera();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            vimba = new VimbaLib();
            vimba.OnNewFrame += OnNewFrame;
            vimba.Open();
        }
    }
}
