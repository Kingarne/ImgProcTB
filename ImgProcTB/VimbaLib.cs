using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AVT.VmbAPINET;

namespace ImgProcTB
{
    public delegate void FrameCB(Frame f);

    public class VimbaLib
    {
        public FrameCB OnNewFrame;

        Camera m_cam;
        Vimba sys = new Vimba();
        CameraCollection cameras = null;
        FeatureCollection features = null;
        Feature feature = null;
        long payloadSize;
        Frame[] frameArray = new Frame[3];

        public VimbaLib()
        {

        }

        public int Open()
        {
            string strName;
            Vimba sys = new Vimba();
            CameraCollection cameras = null;

            try
            {
                sys.Startup();
                cameras = sys.Cameras;

                Console.WriteLine("Cameras found: " + cameras.Count);
                Console.WriteLine();
                

                foreach (Camera camera in cameras)
                {
                    try
                    {
                        strName = camera.Name;
                    }
                    catch (VimbaException ve)
                    {
                        strName = ve.Message;
                    }
                    Console.WriteLine("/// Camera Name: " + strName);
                }
            }
            catch(Exception e)
            {
                //sys.Shutdown();
            }
            StartCamera(cameras[1]);
            return 0;

        }

        public void StartCamera(Camera cam)
        {
            m_cam = cam;

            m_cam.Open(VmbAccessModeType.VmbAccessModeFull);
            m_cam.OnFrameReceived += new Camera.OnFrameReceivedHandler(OnFrameReceived);

            features = m_cam.Features;
            feature = features["PayloadSize"];

            payloadSize = feature.IntValue;
            for (int index = 0; index < frameArray.Length; ++index)
            {
                frameArray[index] = new Frame(payloadSize); 
              m_cam.AnnounceFrame(frameArray[index]); 
            }
           // 

            for (int index = 0; index < frameArray.Length; ++index)
            {
                m_cam.QueueFrame(frameArray[index]);

            }

            feature = features["PixelFormat"];
            feature.EnumValue = "RGB8Packed";

            feature = features["AcquisitionFrameRateLimit"];
            double max = feature.FloatValue;
            feature = features["AcquisitionFrameRate"];
            feature.FloatValue = max;// -0.5f;

            Console.WriteLine("max rate = " + max.ToString());

            feature = features["AcquisitionMode"];

            feature.EnumValue = "Continuous";
            feature = features["AcquisitionStart"];


            feature.RunCommand();
           
            m_cam.StartCapture();
        }

        private void OnFrameReceived(Frame frame)
        {
            //if (InvokeRequired) // if not from this thread invoke it in our context
            {
            // In case of a separate thread (e.g. GUI ) use BeginInvoke to avoid a deadlock
            //Invoke(new Camera.OnFrameReceivedHandler(OnFrameReceived), frame );
            }

           
            if (VmbFrameStatusType.VmbFrameStatusComplete == frame.ReceiveStatus)
            {
                OnNewFrame(frame);

            }
            m_cam.QueueFrame(frame );
        }

        public void StopCamera()
        {
            FeatureCollection features = m_cam.Features;
            Feature feature = features["AcquisitionStop"];
            feature.RunCommand();

            m_cam.EndCapture();
            //m_cam.FlushQueue();
            //m_cam.RevokeAllFrames();
            m_cam.Close();

            sys.Shutdown();
        }
    }
}
