using System;
using System.IO;
using System.Web.Script.Serialization;

namespace DBAILearningAssistant
{
    internal sealed class AppSettings
    {
        public string DifyBaseUrl { get; set; } = "https://api.dify.ai/v1";
        public string DifyApiKey { get; set; } = "";
        public string UserId { get; set; } = "designbuilder-student";

        private static string Folder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DBAILearningAssistant");
        private static string FilePath => Path.Combine(Folder, "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(FilePath)) return new AppSettings();
                return new JavaScriptSerializer().Deserialize<AppSettings>(File.ReadAllText(FilePath))
                       ?? new AppSettings();
            }
            catch { return new AppSettings(); }
        }

        public void Save()
        {
            Directory.CreateDirectory(Folder);
            File.WriteAllText(FilePath, new JavaScriptSerializer().Serialize(this));
        }
    }
}