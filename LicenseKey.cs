namespace Api
{
    public class LicenseKey
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public bool IsUnlimited { get; set; } = false;
        public string? UserIdentifier { get; set; }

        private DateTime? _trialStart;
        public DateTime? TrialStart
        {
            get => _trialStart;
            set => _trialStart = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }

        private DateTime _expirationDate;
        public DateTime ExpirationDate
        {
            get => _expirationDate;
            set => _expirationDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }

}
