using PaymentUI.Models;

namespace PaymentUI.ViewModels
{
    public class UIViewModel
    {
        private string _sessionId;
        private string _message;
        private string _buttonText;
        private int _timeout;
        private bool _showManualEntryButton;

        // Needed to support BooleanToVisibilityConverter n UIView.xaml
        public UIViewModel()
        {

        }

        public UIViewModel(PaymentActionEventArgs e, bool manualEntry)
            => (_sessionId, _message, _buttonText, _timeout, _showManualEntryButton) = (e.SessionId, e.Message, e.ButtonText, e.Timeout, manualEntry);

        public string SessionTxt
        {
            get
            {
                return _sessionId;
            }
            set
            {
                _sessionId = value;
            }
        }

        public string MessageTxt
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        public string ButtonTxt
        {
            get
            {
                return _buttonText;
            }
            set
            {
                _buttonText = value;
            }
        }

        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        public bool ShowManualEntryButton
        {
            get
            {
                return _showManualEntryButton;
            }
            set
            {
                _showManualEntryButton = value;
            }
        }
    }
}
