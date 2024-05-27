using Emgu.CV.CvEnum;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachineImageRecognition
{
    public class LinuxCamera : ICamera
    {
        private VideoCapture _capture;

        public LinuxCamera()
        {
            try
            {
                _capture = new VideoCapture(0, VideoCapture.API.OpencvMjpeg);

                if (!_capture.IsOpened)
                {
                    throw new Exception("Camera could not be opened.");
                }
                Console.WriteLine(_capture.BackendName);
                Console.WriteLine(_capture.CaptureSource);

                Console.WriteLine("Setting width");
                if (!_capture.Set(CapProp.FrameWidth, 640))
                {
                    Console.WriteLine("Failed to set frame width");
                }

                Console.WriteLine("Setting height");
                if (!_capture.Set(CapProp.FrameHeight, 480))
                {
                    Console.WriteLine("Failed to set frame height");
                }

                //Console.WriteLine("Setting RGB");
                //if (!_capture.Set(CapProp.ConvertRgb, 1))
                //{
                //    Console.WriteLine("Failed to set RGB conversion");
                //}

                Console.WriteLine("Camera initialized successfully");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Mat CaptureFrame()
        {
            Mat frame = new Mat();
            _capture.Read(frame);
            return frame;
        }
    }
}
