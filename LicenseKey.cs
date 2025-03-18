namespace Api
{
    public class LicenseKey
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public bool IsUnlimited { get; set; } = false;
    }

}
