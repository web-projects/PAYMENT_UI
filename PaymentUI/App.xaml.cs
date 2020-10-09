using PaymentUI.Helpers;
using PaymentUI.Models;
using PaymentUI.ViewModels;
using PaymentUI.Views;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Timer = System.Timers.Timer;

namespace PaymentUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private UIView TransactionView;
        private UIPaymentView PaymentView;
        private Timer appStartupTimer;
        private TaskScheduler syncContextScheduler;
        private PaymentActionEventArgs PaymentActionEventArgs;
        private CancellationTokenSource cancellationTokenSource;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            // Create a single instance of the transaction view
            TransactionView = new UIView();

            // Payment View
            PaymentView = new UIPaymentView();

            SetStartupTimer(500);
        }

        private void SetStartupTimer(int timeout)
        {
            appStartupTimer = new Timer();
            appStartupTimer.Interval = timeout;
            appStartupTimer.Elapsed += new ElapsedEventHandler(Startup_Timer_Elapsed);
            appStartupTimer.Start();
        }

        private void Startup_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            appStartupTimer?.Stop();
            appStartupTimer?.Dispose();
            appStartupTimer = null;

            this.Dispatcher.Invoke((Action)delegate ()
            {
                PaymentActionEventArgs ePayment = new PaymentActionEventArgs()
                {
                    SessionId = "DEADBEEF-BADDCACA",
                    Message = "Payment",
                    ButtonText = "Cancel Payment",
                    Timeout = 5000
                };
                DalMonitorManager_OnTransactionRequested(sender, ePayment);
            });
        }

        private void DalMonitorManager_OnTransactionRequested(object sender, PaymentActionEventArgs e)
        {
            try
            {
                //SetRequestHeader(e.Header);

                // TODO: change payment action when request is from receiver only
                //if (commHeader.CommIdentifiers[0].Service == ServiceType.Receiver)
                //if (e.LinkRequest.TCCustID != 0)
                //{
                //    PaymentActionEventArgs = e;
                //}

                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource?.Cancel();
                    cancellationTokenSource?.Dispose();
                }

                cancellationTokenSource = new CancellationTokenSource();

                _ = Task.Run(async () => { await Task.Delay(1); })
                .ContinueWith(antecedent =>
                {
                    PaymentScreen(e);
                }, cancellationTokenSource.Token, TaskContinuationOptions.None, syncContextScheduler);
            }
            catch (Exception ex)
            {
                //_ = loggingClient.LogErrorAsync("Exception occurred in DalMonitorManager_OnTransactionRequested.", ex);
            }
        }

        private async Task ProcessManualPayment(PaymentActionEventArgs e)
        {
            await Task.Delay(1);

            //LinkRequest linkRequest = e.LinkRequest;
            //linkRequest.Actions = new List<LinkActionRequest>()
            //{
            //    new LinkActionRequest()
            //    {
            //        MessageID = e.LinkRequest.Actions[0]?.MessageID,
            //        SessionID = e.LinkRequest.Actions[0]?.SessionID,
            //        Action = LinkAction.PaymentUpdate,
            //        DALRequest = e.LinkRequest.Actions[0]?.DALRequest,
            //        DALActionRequest = new LinkDALActionRequest()
            //        {
            //            DALAction = LinkDALActionType.GetManualPAN
            //        },
            //        PaymentUpdateRequest = new XO.Requests.Payment.LinkPaymentUpdateRequest()
            //        {
            //            ManualPayment = true,
            //            ReferenceInformation = e.LinkRequest.Actions[0]?.PaymentRequest?.ReferenceInformation
            //        }
            //    }
            //};

            //_ = loggingClient.LogInfoAsync($"Monitor Sending Event {LinkAction.PaymentUpdate} to {ServiceType.Servicer}");
            //string jsonToPublish = JsonConvert.SerializeObject(linkRequest);
            //_ = loggingClient.LogInfoAsync($"Sending to Listener: {jsonToPublish}").ConfigureAwait(false);
            //await brokerConnector.PublishAsync(jsonToPublish, commHeader, ServiceType.Servicer);
        }

        private void PaymentScreen(PaymentActionEventArgs e)
        {
            bool isManualEntry = ShowManualEntryButton(e);
            PaymentView.DataContext = new UIViewModel(e, isManualEntry);
            PaymentView.Topmost = true;
            PaymentView.Closing += TransactionView_Closing;
            PaymentView.CancelTransactionBtn.IsEnabled = true;

            PaymentView.SetDialogTimer();
            PaymentView.SetAllButtonsEnabledState(true);

            if (PaymentView.Visibility != Visibility.Visible)
            {
                PaymentView.ShowDialog();
            }
        }

        private async Task TransactionScreen(PaymentActionEventArgs e)
        {
            bool isManualEntry = ShowManualEntryButton(e);
            TransactionView.DataContext = new UIViewModel(e, isManualEntry);
            TransactionView.Topmost = true;
            TransactionView.Closing += TransactionView_Closing;
            TransactionView.CancelTransactionBtn.IsEnabled = true;

            // only engage devices support manual entry
            if (isManualEntry)
            {
                TransactionView.ManualEntryBtn.IsEnabled = true;
            }
            else
            {
                TransactionView.CancelTransactionBtn.HorizontalAlignment = HorizontalAlignment.Center;
            }

            await Task.Delay(1);
            TransactionView.ShowDialog();
        }

        private bool ShowManualEntryButton(PaymentActionEventArgs e)
            => false;

        private void TransactionView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                e.Cancel = true;

                System.Windows.Controls.TextBlock message = TreeViewHelper.GetChildOfType<System.Windows.Controls.TextBlock>(PaymentView.ButtonPressed);
                Debug.WriteLine($"BUTTON ACTION='{message.Text}'");

                if (TransactionView.PaymentCancelled)
                {
                    TransactionView.Topmost = false;
                    TransactionView.Visibility = Visibility.Hidden;
                    //TransactionView.Hide();
                }
                //else if (TransactionView.ManualEntrySelected)
                //{
                //    _ = ProcessManualPayment(PaymentActionEventArgs);
                //}
                //else if (TransactionView.PaymentCancelled)
                //{
                //    if (TransactionView.CancelTransactionBtn.Content.ToString().Equals("Cancel", StringComparison.OrdinalIgnoreCase))
                //    {
                //        _ = SetCancelationResponse(PaymentActionEventArgs.LinkRequest);
                //    }
                //}

                SetStartupTimer(500);
            }
        }
    }
}
