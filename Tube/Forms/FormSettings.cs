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
        
        private static readonly string SETTINGS_FILE_NAME = "Glue.FormSettings.Json";

        private string name;
        private Rectangle position;
        private bool isMaximized;

        private static readonly Dictionary<string, FormSettings> s_collectedSettings = new Dictionary<string, FormSettings>();

        public string Name { get => name; set => name = value; }
        public Rectangle Position { get => position; set => position = value; }
        public bool IsMaximized { get => isMaximized; set => isMaximized = value; }

        public FormSettings(string name, Rectangle position, bool isMaximized)
        {
            Name = name;
            Position = position;
            IsMaximized = isMaximized;
        }

        public static void Load()
        {
            List<FormSettings> settingsList;

            LOGGER.Info("Reading form settings from [" + SETTINGS_FILE_NAME + "]");

            if (File.Exists(SETTINGS_FILE_NAME))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    DefaultValueHandling = DefaultValueHandling.Populate
                };

                try
                {
                    using (StreamReader sr = new StreamReader(SETTINGS_FILE_NAME))
                    {
                        using (JsonReader reader = new JsonTextReader(sr))
                        {
                            reader.SupportMultipleContent = true;
                            reader.Read();
                            settingsList = serializer.Deserialize<List<FormSettings>>(reader);

                            foreach(FormSettings settings in settingsList)
                            {
                                s_collectedSettings[settings.Name] = settings;
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
                LOGGER.Info("Saving form settings to [" + SETTINGS_FILE_NAME + "]");

                JsonSerializer serializer = new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.None,
                    NullValueHandling = NullValueHandling.Ignore,
                };

                using (StreamWriter sw = new StreamWriter(SETTINGS_FILE_NAME))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;

                    writer.WriteComment("\r\nGlue settings file\r\n");
                    sw.Write("\r\n");

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

        internal static void Set(string name, FormSettings settings)
        {
            s_collectedSettings[name] = settings;
            Save();
        }
    }
}
