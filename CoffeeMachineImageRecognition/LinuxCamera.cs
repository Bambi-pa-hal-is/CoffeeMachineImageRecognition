using System;
using System.Diagnostics;
using System.IO;
using CoffeeMachineImageRecognition;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

public class LinuxCamera : ICamera, IDisposable
{
    private Process _process;
    private Stream _outputStream;
    private readonly byte[] _buffer = new byte[1024 * 1024]; // 1 MB buffer for reading frames

    public LinuxCamera()
    {
        try
        {
            StartLibCameraVidProcess();

            Console.WriteLine("Camera initialized successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Camera initialization failed: {e.Message}");
            throw;
        }
    }

    private void StartLibCameraVidProcess()
    {
        string command = "libcamera-vid";
        string args = "--codec mjpeg --inline --width 320 --height 320 -o -"; // Set resolution and output to stdout

        var startInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process = new Process { StartInfo = startInfo };
        _process.Start();
        _outputStream = _process.StandardOutput.BaseStream;
    }

    public Mat CaptureFrame()
    {
        using (var ms = new MemoryStream())
        {
            int bytesRead;
            // Read enough data to form a frame
            while ((bytesRead = _outputStream.Read(_buffer, 0, _buffer.Length)) > 0)
            {
                ms.Write(_buffer, 0, bytesRead);
                if (IsCompleteFrame(ms.ToArray()))
                {
                    break;
                }
            }
            byte[] imageData = ms.ToArray();
            Mat frame = new Mat();
            CvInvoke.Imdecode(imageData, ImreadModes.Color, frame);
            return frame;
        }
    }

    private bool IsCompleteFrame(byte[] imageData)
    {
        // Check if the imageData represents a complete MJPEG frame
        // Simple check for the JPEG end marker (0xFFD9)
        if (imageData.Length < 2) return false;
        return imageData[^2] == 0xFF && imageData[^1] == 0xD9;
    }

    public void Dispose()
    {
        _process?.Kill();
        _process?.Dispose();
        _outputStream?.Dispose();
    }
}
