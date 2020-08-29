using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Smairo.MSFS2020.Model;
using Smairo.MSFS2020.Model.Structs;
using Smairo.MSFS2020.SimConnector.Interfaces;
namespace Smairo.MSFS2020.SimConnector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly SimvarsViewModel _model;
        public MainWindow()
        {
            _model = new SimvarsViewModel();
            InitializeComponent();

            _timer.Interval = new TimeSpan(0, 0, 0, 10, 0);
            _timer.Tick += OnTick;
            _timer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!_model.Connected)
            {
                _model.Connect();
            }
        }

        protected HwndSource GetHWinSource()
        {
            return PresentationSource.FromVisual(this) as HwndSource;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            GetHWinSource().AddHook(WndProc);
            if (_model is IBaseSimConnectWrapper oBaseSimConnectWrapper)
            {
                oBaseSimConnectWrapper.SetWindowHandle(GetHWinSource().Handle);
            }
        }

        private IntPtr WndProc(IntPtr hWnd, int iMsg, IntPtr hWParam, IntPtr hLParam, ref bool bHandled)
        {
            if (_model is IBaseSimConnectWrapper oBaseSimConnectWrapper)
            {
                try
                {
                    if (iMsg == oBaseSimConnectWrapper.GetUserSimConnectWinEvent())
                    {
                        oBaseSimConnectWrapper.ReceiveSimConnectMessage();
                    }
                }
                catch
                {
                    oBaseSimConnectWrapper.Disconnect();
                }
            }

            return IntPtr.Zero;
        }
    }
}