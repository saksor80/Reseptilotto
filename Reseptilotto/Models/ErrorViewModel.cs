namespace CustomerEstablishmentRequest.Models
{
    public class ErrorViewModel //Sets ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}