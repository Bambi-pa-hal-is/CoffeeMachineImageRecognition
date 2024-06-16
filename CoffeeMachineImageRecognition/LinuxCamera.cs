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
        InitializeCamera();
    }

    private void InitializeCamera()
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
        string args = "--codec mjpeg --inline --width 320 --height 320 -t 0 -o -"; // Set resolution and output to stdout

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

        // Log any errors from the process
        _process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"libcamera-vid error: {e.Data}");
            }
        };
        _process.BeginErrorReadLine();
    }

    public Mat CaptureFrame()
    {
        try
        {
            using (var ms = new MemoryStream())
            {
                int bytesRead;
                while ((bytesRead = _outputStream.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    ms.Write(_buffer, 0, bytesRead);
                    if (IsCompleteFrame(ms.ToArray()))
                    {
                        break;
                    }
                }
                byte[] imageData = ms.ToArray();
                if (imageData.Length > 0 && IsCompleteFrame(imageData))
                {
                    Mat frame = new Mat();
                    CvInvoke.Imdecode(imageData, ImreadModes.Color, frame);
                    return frame;
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error capturing frame: {e.Message}");
            RestartCameraProcess();
            return null;
        }
    }

    private bool IsCompleteFrame(byte[] imageData)
    {
        // Check if the imageData represents a complete MJPEG frame
        if (imageData.Length < 2) return false;
        return imageData[^2] == 0xFF && imageData[^1] == 0xD9;
    }

    private void RestartCameraProcess()
    {
        DisposeProcess();
        InitializeCamera();
    }

    private void DisposeProcess()
    {
        try
        {
            _process?.Kill();
            _process?.Dispose();
            _outputStream?.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error disposing process: {e.Message}");
        }
    }

    public void Dispose()
    {
        DisposeProcess();
    }
}
