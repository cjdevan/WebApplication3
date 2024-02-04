namespace WebApplication3.ViewModels
{
    public class Index
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? NRIC { get; set; }
        public string? UserEmail { get; set; }
        public string? Password { get; set; }
        public string? DateOfBirth { get; set; }
        public string? ResumeFilePath { get; set; }
        public string? WhoAmI { get; set; }

        public string? Resume => Path.GetFileName(ResumeFilePath);
    }
}
