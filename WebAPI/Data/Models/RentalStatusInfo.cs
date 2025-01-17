namespace WebAPI.Data.Models
{
    // TODO: use it
    public static class RentalStatusInfo
    {
        public static int ConfirmedId { get; private set; }
        public static int CompletedId { get; private set; }

        public static string ConfirmedDescription { get; private set; } = string.Empty;
        public static string CompletedDescription { get; private set; } = string.Empty;

        public static void Initialize(int confirmedId, string confirmedDescription, int completedId, string completedDescription)
        {
            ConfirmedId = confirmedId;
            ConfirmedDescription = confirmedDescription;
            CompletedId = completedId;
            CompletedDescription = completedDescription;
        }
    }
}