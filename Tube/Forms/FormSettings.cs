using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Glue.Forms
{
    internal class FormSettings
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private static readonly string SETTINGS_FILE_NAME_DEFAULT = "form-settings.json";

        private string name;
        private Rectangle position;
        private bool isMaximized;
        private static string s_settingsFileName = SETTINGS_FILE_NAME_DEFAULT;

        private static readonly Dictionary<string, FormSettings> s_collectedSettings = new Dictionary<string, FormSettings>();

        public string Name { get => name; set => name = value; }
        public Rectangle Position { get => position; set => position = value; }
        public bool IsMaximized { get => isMaximized; set => isMaximized = value; }
        public static string FileName { get => s_settingsFileName; set => s_settingsFileName = value; }

        public FormSettings(string name, Rectangle position, bool isMaximized)
        {
            Name = name;
            Position = position;
            IsMaximized = isMaximized;
        }

        public static void Load()
        {
            List<FormSettings> settingsList;

            if (File.Exists(FileName))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    DefaultValueHandling = DefaultValueHandling.Populate
                };

                try
                {
                    using (StreamReader sr = new StreamReader(FileName))
                    {
                        if (sr.BaseStream.Length == 0)
                        {
                            sr.Close();
                            File.Delete(FileName);
                            LOGGER.Warn("Form settings found but is empty. Deleting " + FileName + " and using default values");
                            return;
                        }

                        using (JsonReader reader = new JsonTextReader(sr))
                        {
                            reader.SupportMultipleContent = true;
                            reader.Read();
                            settingsList = serializer.Deserialize<List<FormSettings>>(reader);

                            if (null != settingsList)
                            {
                                foreach (FormSettings settings in settingsList)
                                {
                                    s_collectedSettings[settings.Name] = settings;
                                }
                            }
                            else
                            {
                                LOGGER.Warn("Form settings found but unable to read. May have been corrupted by hard reset.");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // Expected the first time run, and can be ignored in all cases
                    LOGGER.Warn("Exception while reading settings: " + e.Message);
                }
            }
        }

        public static void Save()
        {
            if (s_collectedSettings?.Count > 0)
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.None,
                    NullValueHandling = NullValueHandling.Ignore,
                };

                using (StreamWriter sw = new StreamWriter(FileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;

                    writer.WriteComment(
                        Environment.NewLine + 
                        "Glue settings file" +
                        Environment.NewLine);
                    sw.Write(Environment.NewLine);

                    List<FormSettings> settingsList = new List<FormSettings>(s_collectedSettings.Values);

                    serializer.Serialize(writer, settingsList);
                }
            }
        }

        internal static FormSettings Get(string name)
        {
            Load();

            FormSettings settings = null;
            s_collectedSettings?.TryGetValue(name, out settings);

            return settings;
        }

        internal static void Save(string name, FormSettings settings)
        {
            s_collectedSettings[name] = settings;
            Save();
        }
    }
}
