namespace Kongverge.Common.DTOs
{
    public class Settings
    {
        public Admin Admin { get; set; }
        public string InputFolder { get; set; }
        public bool DryRun { get; set; } = false;
        public string OutputFolder { get; set; }
        public static string FileExtension { get; } = ".json";
        public string TestFolder { get; set; } = "tests";
        public int TestPort { get; set; } = 65150;
    }

    public class Admin
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
