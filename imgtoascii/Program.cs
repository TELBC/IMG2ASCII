using OpenCvSharp;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text;

namespace imgtoascii
{
    class Program
    {
        private static string density = "                ..'`^\",:; Il!i><~+_-?][}{1)(|/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";
        static void Main(string[] args)
        {
            Consolecolors();
            ConsoleColor[] colors = new ConsoleColor[160 * 60];
            //frames counter
            DateTime lastTime = DateTime.Now;
            int frameRendered = 0;
            int frames = 0;

            //UI & Stuff
            bool colorful = ConsoleStartup();
            Console.Clear();
            Console.WriteLine("If you are on laptop it is recommended to plug it in for better performance");
            Console.WriteLine("Press any key to stop program");
            Console.WriteLine("Initializing...");
            Console.CursorVisible = false;

            using (var capture = new VideoCapture(0))
            using (var frame = new Mat())
            {
                SafeFileHandle h = CreateFile("CONOUT$", 0x60000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
                capture.FrameWidth = 80;
                capture.FrameHeight = 60;
                while (true)
                {
                    capture.Read(frame);
                    string f = Imgtoascii(AdjustContrast((Bitmap)Image.FromStream(frame.ToMemoryStream()), 50), 80, colors, colorful);
                    ushort[] hexed = ToDec(f);

                    if (!h.IsInvalid)
                    {
                        CharInfo[] buf = new CharInfo[hexed.Length];
                        SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = 160, Bottom = 60 };
                        for (int i = 0; i < 160 * 60; i++)
                        {
                            buf[i].Char.UnicodeChar = hexed[i];
                            if (colorful) buf[i].Attributes = (short)colors[i];

                            else if (!colorful)
                            {//gray scaling with shadows
                                if (hexed[i] == '$' || hexed[i] == '@' || hexed[i] == 'B' || hexed[i] == '%' || hexed[i] == '8' || hexed[i] == '&' || hexed[i] == 'W')
                                    buf[i].Attributes = 15;
                                else if (hexed[i] == ' ' || hexed[i] == '.' || hexed[i] == '\'' || hexed[i] == '`' || hexed[i] == '^' || hexed[i] == '\"' || hexed[i] == ',')
                                    buf[i].Attributes = 8;
                                else
                                    buf[i].Attributes = 7;
                            }
                        }
                        WriteConsoleOutput(h, buf,
                              new Coord() { X = 160, Y = 60 },
                              new Coord() { X = 0, Y = 0 },
                              ref rect);
                        frameRendered++;
                    }
                    if (Console.KeyAvailable) break;
                    Process proc = Process.GetCurrentProcess();
                    double memory = Math.Round((double)(proc.PrivateMemorySize64 / Math.Pow(1024, 2)), 2);
                    proc.Dispose();
                    Console.Title = "ASCII Camera. Video to ASCII Video converter    Frames: " + frames + "FPS        RAM usage: " + memory + "MB";
                    if ((DateTime.Now - lastTime).TotalSeconds >= 1)
                    {
                        frames = frameRendered;
                        frameRendered = 0;
                        lastTime = DateTime.Now;
                    }
                }
            }
            Task.WaitAll();//quick fix for callback error
            Console.Clear();
            Console.Title = "ASCII Camera. Video to ASCII Video converter";
            Console.WriteLine("Made by TELBC");
            Console.WriteLine("Thank you for using the ASCII Camera!");
            Console.WriteLine("Please Check out my Github Page!");
            Console.WriteLine("https://github.com/TELBC");
            Thread.Sleep(5000);
            Console.WriteLine("Closing...");
        }
        private static bool ConsoleStartup()
        {
            Console.SetWindowSize(160, 60);
            Console.SetBufferSize(160, 60);
            Console.Title = "ASCII Camera. Video to ASCII Video converter";
            Console.WriteLine("Welcome to the ASCII Camera \nA Pixel-video to ASCII-video converter");
            Console.WriteLine("Colored, but much slower -> y");
            Console.WriteLine("Colourless, but faster -> n");
            Console.Write("Please input (y/n):");
            string? colors_input = null;
            while (true)
            {
                colors_input = Console.ReadLine();
                if (colors_input == "y" || colors_input == "n")
                {
                    if (colors_input == "n") return false;
                    else if (colors_input == "y") return true;
                }
                else
                {
                    Console.WriteLine("Wrong input!");
                }
            }
        }
        private static ushort[] ToDec(string input)
        {
            ushort[] value = new ushort[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                value[i] = (ushort)Convert.ToInt32(input[i]);
            }
            return value;
        }
        private static string Imgtoascii(Bitmap input, double imgWidth, ConsoleColor[] colorsOne, bool colorful)
        {
            double ratio = (double)(imgWidth / input.Width);
            input = new Bitmap(input, new System.Drawing.Size((int)imgWidth, (int)(input.Height * ratio)));
            input.RotateFlip(RotateFlipType.RotateNoneFlipX);
            var buffer = new string[input.Height];
            for (int i = 0; i < input.Height; i++)
            {
                var line = new string[2 * input.Width];
                for (int j = 0; j < input.Width; j++)
                {
                    var pixel = input.GetPixel(j, i);
                    string value = density[(pixel.R + pixel.G + pixel.B) / 9].ToString();
                    line[2 * j] = value;
                    line[2 * j + 1] = value;

                    if (colorful)
                    {
                        colorsOne[i * input.Width * 2 + 2 * j] = ClosestConsoleColor(pixel.R, pixel.G, pixel.B);
                        colorsOne[i * input.Width * 2 + 2 * j + 1] = ClosestConsoleColor(pixel.R, pixel.G, pixel.B);
                    }
                }
                buffer[i] = string.Join("", line);
            }
            StringBuilder output = new StringBuilder();
            foreach (string line in buffer)
            {
                output.Append(line);
            }
            return output.ToString();
        }
        private static void Consolecolors()
        {
            //makes sure windows console can process the ansi colors
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C reg add HKEY_CURRENT_USER\\Console /v VirtualTerminalLevel /t REG_DWORD /d 0x00000001 /f";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            Console.Clear();
        }
        private static ConsoleColor ClosestConsoleColor(byte r, byte g, byte b)
        {
            //method copied from Glenn Slayden
            ConsoleColor ret = 0;
            double rr = r, gg = g, bb = b, delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var n = Enum.GetName(typeof(ConsoleColor), cc);
                var c = Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
                var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                    return cc;
                if (t < delta)
                {
                    delta = t;
                    ret = cc;
                }
            }
            return ret;
        }
        private static Bitmap AdjustContrast(Bitmap image, float value)
        {
            value = (100.0f + value) / 100.0f;
            value *= value;
            Bitmap NewBitmap = (Bitmap)image.Clone();
            BitmapData data = NewBitmap.LockBits(
                new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }

            NewBitmap.UnlockBits(data);

            return NewBitmap;
        }

        //DLLimports 
        //DO NOT TOUCH
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(string fileName, [MarshalAs(UnmanagedType.U4)] uint fileAccess, [MarshalAs(UnmanagedType.U4)] uint fileShare, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] int flags, IntPtr template);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(SafeFileHandle hConsoleOutput, CharInfo[] lpBuffer, Coord dwBufferSize, Coord dwBufferCoord, ref SmallRect lpWriteRegion);
        [StructLayout(LayoutKind.Sequential)]
        private struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };
        [StructLayout(LayoutKind.Explicit)]
        private struct CharUnion
        {
            [FieldOffset(0)] public ushort UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }
        [StructLayout(LayoutKind.Explicit)]
        private struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }
    }
}