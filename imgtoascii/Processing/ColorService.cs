using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp;

namespace ImgToAscii.Processing;

public class ColorService
{
    public static void ConsoleColors()
    {
        //makes sure windows console can process the ansi colors
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments =
            "/C reg add HKEY_CURRENT_USER\\Console /v VirtualTerminalLevel /t REG_DWORD /d 0x00000001 /f";
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
        Console.Clear();
    }
    
    public static Mat AdjustContrast(Mat image, float contrast, float brightness)
    {
        Mat adjustedImage = image.Clone();

        // Adjust contrast and brightness using the pixel-wise operation
        for (int y = 0; y < image.Rows; y++)
        {
            for (int x = 0; x < image.Cols; x++)
            {
                Vec3b color = image.At<Vec3b>(y, x);

                for (int c = 0; c < 3; c++)
                {
                    float newValue = contrast * color[c] + brightness;
                    color[c] = (byte)Math.Max(0, Math.Min(255, newValue));
                }

                adjustedImage.Set(y, x, color);
            }
        }

        return adjustedImage;
    }
}