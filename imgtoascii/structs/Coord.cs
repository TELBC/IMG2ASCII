using System.Runtime.InteropServices;

namespace ImgToAscii;

[StructLayout(LayoutKind.Sequential)]
public struct Coord
{
    public short X;
    public short Y;

    public Coord(short x, short y)
    {
        this.X = x;
        this.Y = y;
    }
};