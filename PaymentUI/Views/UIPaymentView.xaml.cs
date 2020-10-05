using System;
using System.Timers;
using System.Windows;

namespace PaymentUI.Views
{
    /// <summary>
    /// Interaction logic for UIView.xaml
    /// </summary>
    public partial class UIPaymentView : Window
    {
        // Prep stuff needed to remove close button on window
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        //TODO retrieve value from configuration: default is 300 seconds
        private int displayTimeout = 5000;
        private Timer dialogTimer = null;

        public bool ManualEntrySelected { get; private set; }
        public bool PaymentCancelled { get; private set; }

        public UIPaymentView()
        {
            InitializeComponent();

            this.Loaded += OnWindowLoaded;
        }

        void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Code to remove close box from window
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            if (dialogTimer is null)
            {
                bool setDialogTimer = CancelTransactionBtn.Content.ToString().Equals("Cancel", StringComparison.OrdinalIgnoreCase);

                if (setDialogTimer)
                {
                    dialogTimer = new Timer();
                    displayTimeout = (int)this.DataContext.GetType().GetProperty("Timeout").GetValue(this.DataContext, null);
                    dialogTimer.Interval = displayTimeout;
                    dialogTimer.Elapsed += new ElapsedEventHandler(Dialog_Timer_Elapsed);
                    dialogTimer.Start();
                }
            }
        }

        private void Dialog_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            dialogTimer?.Stop();
            dialogTimer?.Dispose();

            this.Dispatcher.Invoke((Action)delegate ()
            {
                this.Close();
            });
        }

        private void ManualEntryBtn_Click(object sender, RoutedEventArgs e)
        {
            ManualEntrySelected = true;
            dialogTimer?.Stop();
            dialogTimer?.Dispose();

            this.Dispatcher.Invoke((Action)delegate ()
            {
                //this.ManualEntryBtn.IsEnabled = false;
                this.CancelTransactionBtn.IsEnabled = false;
                this.Close();
            });
        }

        private void CancelTransactionBtn_Click(object sender, RoutedEventArgs e)
        {
            PaymentCancelled = true;
            dialogTimer?.Stop();
            dialogTimer?.Dispose();

            this.Dispatcher.Invoke((Action)delegate ()
            {
                //this.ManualEntryBtn.IsEnabled = false;
                this.CancelTransactionBtn.IsEnabled = false;
                this.Close();
            });
        }
    }
}
