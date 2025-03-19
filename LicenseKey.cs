namespace Api
{
    public class LicenseKey
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public bool IsUnlimited { get; set; } = false;
    
        public DateTime ExpirationDate
        {
            get => _expirationDate;
            set => _expirationDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private DateTime _expirationDate;
    }
}
