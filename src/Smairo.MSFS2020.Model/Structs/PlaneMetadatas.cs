using System.Runtime.InteropServices;

namespace Smairo.MSFS2020.Model.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PlaneMetadatas
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string AtcType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string AtcModel;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string AtcAirline;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string AtcId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string AtcFlightNumber;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Title;
    }
}