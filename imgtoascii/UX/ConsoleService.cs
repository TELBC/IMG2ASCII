namespace ImgToAscii.UX;

public class ConsoleService
{
    public static bool ConsoleStartup()
    {
        Console.CursorVisible = false;
        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        Console.Title = "ASCII Camera. Video to ASCII Video converter";
        Console.WriteLine("Welcome to the ASCII Camera \nA Pixel-video to ASCII-video converter");
        Console.WriteLine("Colored, but much slower -> y");
        Console.WriteLine("Colourless, but faster -> n");
        Console.Write("Please input (y/n):");
        while (true)
        {
            string? colorsInput = Console.ReadLine();
            if (colorsInput == "y" || colorsInput == "n")
            {
                if (colorsInput == "n") return false;
                else if (colorsInput == "y") return true;
            }
            else
            {
                Console.WriteLine("Wrong input!");
            }
        }
    }

    public static int ConsoleResolution()
    {
        Console.WriteLine("1(inefficient, high res)");
        Console.WriteLine("18(efficient, low res)");
        Console.Write("Resolution amount: ");
        while (true)
        {
            string? resolutionInput = Console.ReadLine();

            if (int.TryParse(resolutionInput, out int result))
            {
                return result;
            }
            Console.WriteLine("Invalid input. Please enter a number.");
        }
    }

    public static void ConsoleInit()
    {
        Console.Clear();
        Console.WriteLine("If you are on laptop it is recommended to plug it in for better performance");
        Console.WriteLine("Press any key to stop program");
        Console.WriteLine("Initializing...");
        Console.CursorVisible = false;
    }

    public static void ConsoleEnd()
    {
        Console.Clear();
        Console.WriteLine("Made by TELBC");
        Console.WriteLine("Thank you for using the ASCII Camera!");
        Console.WriteLine("Please Check out my Github Page!");
        Console.WriteLine("https://github.com/TELBC");
        Thread.Sleep(2500);
        Console.WriteLine("Closing...");
        Thread.Sleep(200);
    }
}