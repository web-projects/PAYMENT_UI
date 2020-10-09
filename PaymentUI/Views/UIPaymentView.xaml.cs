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

        public System.Windows.Controls.Button ButtonPressed { get; private set; }

        public bool CancelTransactionSelected { get; private set; }
        public bool ProcessRequestSelected { get; private set; }
        public bool PrintReceiptSelected { get; private set; }
        public bool VoidPaymentSelected { get; private set; }

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
        }

        void DialogTimerDispose()
        {
            dialogTimer?.Stop();
            dialogTimer?.Dispose();
            dialogTimer = null;
        }

        void Dialog_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DialogTimerDispose();

            this.Dispatcher.Invoke((Action)delegate ()
            {
                this.Close();
            });
        }

        void CancelTransactionBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogTimerDispose();

            CancelTransactionSelected = true;
            ButtonPressed = CancelTransactionBtn;

            this.Dispatcher.Invoke((Action)delegate ()
            {
                SetAllButtonsEnabledState(false);
                this.Close();
            });
        }

        void ProcessRequestBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogTimerDispose();

            ProcessRequestSelected = true;
            ButtonPressed = ProcessRequestBtn;

            this.Dispatcher.Invoke((Action)delegate ()
            {
                SetAllButtonsEnabledState(false);
                this.Close();
            });
        }

        void PrintReceiptBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogTimerDispose();

            PrintReceiptSelected = true;
            ButtonPressed = PrintReceiptBtn;

            this.Dispatcher.Invoke((Action)delegate ()
            {
                SetAllButtonsEnabledState(false);
                this.Close();
            });
        }
        void VoidPaymentBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogTimerDispose();

            VoidPaymentSelected = true;
            ButtonPressed = VoidPaymentBtn;

            this.Dispatcher.Invoke((Action)delegate ()
            {
                SetAllButtonsEnabledState(false);
                this.Close();
            });
        }

        public void SetDialogTimer()
        {
            DialogTimerDispose();

            bool setDialogTimer = (bool)this.DataContext.GetType().GetProperty("ButtonTxt").GetValue(this.DataContext, null).ToString().Equals("Cancel Payment", StringComparison.OrdinalIgnoreCase);
            displayTimeout = (int)this.DataContext.GetType().GetProperty("Timeout").GetValue(this.DataContext, null);

            if (setDialogTimer && displayTimeout > 0)
            {
                dialogTimer = new Timer();
                dialogTimer.Interval = displayTimeout;
                dialogTimer.Elapsed += new ElapsedEventHandler(Dialog_Timer_Elapsed);
                dialogTimer.Start();
            }
        }

        public void SetAllButtonsEnabledState(bool state)
        {
            this.CancelTransactionBtn.IsEnabled = state;
            this.ProcessRequestBtn.IsEnabled = state;
            this.PrintReceiptBtn.IsEnabled = state;
            this.VoidPaymentBtn.IsEnabled = state;
        }
    }
}
