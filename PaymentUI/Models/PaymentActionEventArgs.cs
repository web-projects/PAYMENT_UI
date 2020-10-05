using System;

namespace PaymentUI.Models
{
    public class PaymentActionEventArgs : EventArgs
    {
        //public CommunicationHeader Header { get; set; }
        //public XO.Requests.LinkRequest LinkRequest { get; set; }
        public int Timeout { get; set; }
        public string SessionId { get; set; }
        public string Message { get; set; }
        public string ButtonText { get; set; }
    }
}
