using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachineImageRecognition
{
    public class YoloDetector
    {
        private Net _net;
        private List<string> _labels = new List<string>() {
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

        public YoloDetector(byte[] model)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".onnx");
            try
            {
                // Write the byte array to the temporary file
                File.WriteAllBytes(tempFilePath, model);

                // Load the ONNX model from the temporary file

                // Load the pretrained model
                _net = DnnInvoke.ReadNetFromONNX(tempFilePath);
                _net.SetPreferableBackend(Emgu.CV.Dnn.Backend.Default);
                _net.SetPreferableTarget(Target.Cpu);
            }
            finally
            {
                // Clean up the temporary file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        // Method to detect objects
        public (string, float) ClassifyImage(Mat image)
        {
            // Convert image to blob
            Mat blob = DnnInvoke.BlobFromImage(image, 1.0 / 255.0, size: new Size(640, 640), mean: new MCvScalar(0, 0, 0), swapRB: true, crop: false);

            // Set the input to the network
            _net.SetInput(blob);

            // Forward pass to get output
            Mat output = _net.Forward();

            // The output is typically a 1D tensor with probabilities for each class
            var (predictedClass, confidence) = GetPredictedClass(output);
            return (IndexToLabel(predictedClass), confidence);
        }

        public string IndexToLabel(int index)
        {
            return _labels.ElementAtOrDefault(index) ?? "Error";
        }


        public (int, float) GetPredictedClass(Mat output)
        {
            var data = output.GetData() as float[,];

            if (data == null)
            {
                Console.WriteLine("Failed to cast data to float[,]");
                return (-1, 0f);
            }

            float maxConfidence = 0;
            int classId = -1;

            // As we have a single row, iterate over each column
            for (int i = 0; i < data.GetLength(1); i++)
            {
                if (data[0, i] > maxConfidence)
                {
                    maxConfidence = data[0, i];
                    classId = i;
                }
            }

            return (classId, maxConfidence);
        }
    }
}
