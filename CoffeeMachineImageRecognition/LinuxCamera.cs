using Emgu.CV.CvEnum;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CoffeeMachineImageRecognition
{
    public class LinuxCamera : ICamera
    {
        public LinuxCamera()
        {
            try
            {
                // Initialize the camera
                Console.WriteLine("Camera initialized successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Camera initialization failed: {e.Message}");
                throw;
            }
        }


        public Mat CaptureFrame()
        {
            byte[] imageData = CaptureImageFromLibCamera();

            // Convert the image data to a Mat object in Emgu CV
            Mat frame = new Mat();
            CvInvoke.Imdecode(imageData, ImreadModes.Color, frame);
            return frame;
        }

        private byte[] CaptureImageFromLibCamera()
        {
            // Command to capture image with libcamera-still
            string command = "libcamera-still";
            string args = "--width 320 --height 320 -o -"; // Output to stdout

            // Start the process
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();

                // Read the output stream to get the image data
                using (var ms = new MemoryStream())
                {
                    process.StandardOutput.BaseStream.CopyTo(ms);
                    process.WaitForExit();
                    return ms.ToArray();
                }
            }
        }
    }
}
