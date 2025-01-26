using System.Text;
using ImgToAscii.Processing;
using ImgToAscii.UX;
using OpenCvSharp;

namespace imgtoascii
{
    class Program
    {
        private static readonly string Density = "                ..'`^\",:;lIl!i><~+_-?][}{1)(|/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";

        static void Main(string[] args)
        {
            ColorService.ConsoleColors();
            bool colorful = ConsoleService.ConsoleStartup();
            int resolution = ConsoleService.ConsoleResolution();
            StringBuilder currentBuffer = new StringBuilder();

            using (var capture = new VideoCapture(0))
            {
                if (!capture.IsOpened())
                {
                    Console.WriteLine("Error: Camera not found!");
                    return;
                }

                Console.CursorVisible = false;

                while (true)
                {
                    Mat frame = new Mat();
                    capture.Read(frame);
                    Mat grayFrame = frame.Clone();
                    if(!colorful) 
                        Cv2.CvtColor(frame, grayFrame, ColorConversionCodes.BGR2GRAY);
                    Cv2.Flip(grayFrame, grayFrame, FlipMode.Y);
                    grayFrame = ColorService.AdjustContrast(grayFrame, contrast: 1f, brightness: 1.4f);

                    if (frame.Empty())
                        break;

                    UpdateBuffer(currentBuffer, grayFrame,resolution, colorful);
                    ClearConsole(currentBuffer.ToString());
                    Console.Write(currentBuffer.ToString());
                }
            }
        }

        static void UpdateBuffer(StringBuilder buffer, Mat frame, int resolutionWidth, bool colorful)
        {
            buffer.Clear();
            var resolutionHeight = resolutionWidth / 2;

            for (int y = 0; y < frame.Rows; y += resolutionWidth)
            {
                for (int x = 0; x < frame.Cols; x += resolutionHeight)
                {
                    Vec3b color = frame.At<Vec3b>(y, x);
                    int gray = (color.Item0 + color.Item1 + color.Item2) / 3;
                    int index = gray * (Density.Length - 1) / 255;
                    char asciiChar = Density[index];

                    if (colorful)
                    {
                        string coloredAscii = $"\x1b[38;2;{color.Item2};{color.Item1};{color.Item0}m{asciiChar}\x1b[0m";
                        buffer.Append(coloredAscii);
                    }
                    else buffer.Append(asciiChar);

                }
                buffer.AppendLine();
            }
        }
        protected static void ClearConsole(string? clearBuffer)
        {
            if (clearBuffer == null)
            {
                var line = "".PadLeft(Console.WindowWidth, ' ');
                var lines = new StringBuilder();

                for (var i = 0; i < Console.WindowHeight; i++)
                {
                    lines.AppendLine(line);
                }

                clearBuffer = lines.ToString();
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(clearBuffer);
            Console.SetCursorPosition(0, 0);
        }
    }
}