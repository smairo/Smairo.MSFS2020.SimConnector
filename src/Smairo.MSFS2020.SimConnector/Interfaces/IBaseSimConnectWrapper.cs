using System;
namespace Smairo.MSFS2020.SimConnector.Interfaces
{
    public interface IBaseSimConnectWrapper
    {
        int GetUserSimConnectWinEvent();
        void ReceiveSimConnectMessage();
        void SetWindowHandle(IntPtr hWnd);
        void Disconnect();
    }
}