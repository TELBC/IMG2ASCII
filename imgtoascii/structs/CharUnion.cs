using System.Runtime.InteropServices;

namespace ImgToAscii;

[StructLayout(LayoutKind.Explicit)]
public struct CharUnion
{
    [FieldOffset(0)] public ushort UnicodeChar;
    [FieldOffset(0)] private readonly byte AsciiChar;
}