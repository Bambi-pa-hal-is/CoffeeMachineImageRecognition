﻿using Emgu.CV.CvEnum;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachineImageRecognition
{
    public class WindowsCamera : ICamera
    {
        private VideoCapture _capture;

        public WindowsCamera()
        {
            try
            {
                _capture = new VideoCapture(0, VideoCapture.API.Msmf);
                _capture.Set(CapProp.FrameWidth, 320);
                _capture.Set(CapProp.FrameHeight, 320);
                _capture.Set(CapProp.ConvertRgb, 1);
                if (!_capture.IsOpened)
                {
                    throw new Exception("Camera could not be opened.");
                }
            }
            catch(Exception e)
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
