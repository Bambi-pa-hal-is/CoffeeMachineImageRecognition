using Emgu.CV.CvEnum;
using Emgu.CV;
using Microsoft.Extensions.DependencyInjection;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using System;

namespace CoffeeMachineImageRecognition
{
    internal class Program
    {
        private static List<string> _labels = new List<string>() {
            "cafeaulait",
            "caffelatte",
            "cappuccino",
            "chocodream",
            "coffee",
            "espresso",
            "hotchocolate",
            "hotwater",
            "lattemachiato",
            "lungo",
            "menu",
            "unknown",
        };
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
            // Initialize ONNX runtime
            while (true)
            {
                // Capture frame
                using Mat frame = camera.CaptureFrame();
                if (!frame.IsEmpty)
                {
                    var (classifiedImage, confidence) = yoloDetector.ClassifyImage(frame);
                    Console.WriteLine(classifiedImage + " confidence: " + confidence);
                }
                else
                {
                    Console.WriteLine("Camera is empty");
                }

                if (CvInvoke.WaitKey(1) == 27) // Escape key to exit
                {
                    break;
                }
            }
        }
    }
}
