using System.Runtime.InteropServices;

namespace Smairo.MSFS2020.Model.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PlaneMetadatas
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string TITLE;
    }
}