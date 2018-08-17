namespace Kongverge.Common.DTOs
{
    public class Settings
    {
        public Admin Admin { get; set; } = new Admin();
        public string InputFolder { get; set; }
        public bool DryRun { get; set; } = false;
        public string OutputFolder { get; set; }
        public static string FileExtension { get; } = ".json";
        public string TestFolder { get; set; } = "tests";
        public int TestPort { get; set; } = 65150;
        public string GlobalConfigPath { get; set; } = "global.json";
    }

    public class Admin
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
