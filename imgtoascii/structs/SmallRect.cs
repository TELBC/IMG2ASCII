﻿using System.Runtime.InteropServices;

namespace ImgToAscii;

[StructLayout(LayoutKind.Sequential)]
public struct SmallRect
{
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;
}