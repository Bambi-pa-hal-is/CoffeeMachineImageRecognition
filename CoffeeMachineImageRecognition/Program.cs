using Emgu.CV.CvEnum;
using Emgu.CV;
using Microsoft.Extensions.DependencyInjection;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using System;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace CoffeeMachineImageRecognition
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("starting!");

            // Setup dependency injection
            var serviceCollection = new ServiceCollection()
                .AddSingleton<ICamera>(provider =>
                {
                    if (OperatingSystem.IsLinux())
                    {
                        return new LinuxCamera();
                    }
                    else if (OperatingSystem.IsWindows())
                    {
                        return new WindowsCamera();
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported OS");
                    }
                });
            serviceCollection.AddHttpClient();

            var services = serviceCollection.BuildServiceProvider();
            var httpClient = services.GetService<HttpClient>();
            Console.WriteLine("loading yolomodel...");
            var yoloModel = await httpClient!.GetByteArrayAsync("https://kosatuppspaces.fra1.cdn.digitaloceanspaces.com/Gb/Static/best.onnx");
            var camera = services.GetService<ICamera>();
            if (camera == null)
            {
                throw new ArgumentNullException("Missing camera");
            }
            Console.WriteLine("yolo starting");
            var yoloDetector = new YoloDetector(yoloModel);
            Console.WriteLine("detecting...");

            var frameQueue = new ConcurrentQueue<Mat>();
            var cts = new CancellationTokenSource();

            // Start frame capturing in a separate thread
            var captureTask = Task.Run(() => CaptureFrames(camera, frameQueue, cts.Token));

            // Start image classification in the main thread
            await ClassifyFrames(yoloDetector, frameQueue, cts.Token);

            // Stop capturing frames when done
            cts.Cancel();
            await captureTask;
        }

        static async Task CaptureFrames(ICamera camera, ConcurrentQueue<Mat> frameQueue, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Mat frame = camera.CaptureFrame();
                    if (frame != null && !frame.IsEmpty)
                    {
                        frameQueue.Enqueue(frame);
                    }
                    await Task.Delay(10); // Adjust delay as necessary
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing frames: {ex.Message}");
            }
        }

        static async Task ClassifyFrames(YoloDetector yoloDetector, ConcurrentQueue<Mat> frameQueue, CancellationToken token)
        {
            Stopwatch stopwatch = new Stopwatch();
            while (!token.IsCancellationRequested)
            {
                if (frameQueue.TryDequeue(out Mat frame))
                {
                    stopwatch.Restart();
                    var (classifiedImage, confidence) = yoloDetector.ClassifyImage(frame);
                    stopwatch.Stop();

                    // Print the classified image, confidence, and elapsed time
                    Console.WriteLine($"{classifiedImage} confidence: {confidence} elapsed time: {stopwatch.Elapsed.TotalSeconds} s");

                    frame.Dispose();
                }
                else
                {
                    await Task.Delay(10); // Adjust delay as necessary
                }

                if (CvInvoke.WaitKey(1) == 27) // Escape key to exit
                {
                    token.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
