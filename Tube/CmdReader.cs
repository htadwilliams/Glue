using System;
using System.IO;

namespace Glue
{
    class CmdReader : IDisposable
    {
        private FileSystemWatcher Watcher { get; set; }
        private StreamReader Reader { get; set; }
        private long LastReadOffset { get; set; }
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string FILENAME_CMD = "gluecmd.txt";
        

        public CmdReader()
        {
            String currentDirectory = Directory.GetCurrentDirectory();
            String fullName = currentDirectory + "\\" + FILENAME_CMD;

            Watcher = new FileSystemWatcher(
                currentDirectory,
                FILENAME_CMD);

            Watcher.EnableRaisingEvents = true;
            Watcher.Changed += OnWatcher_Changed;
            Watcher.Created += OnWatcher_Created;

            // File already exists on startup - only read new stuff
            if (File.Exists(currentDirectory + "\\" + FILENAME_CMD))
            {
                Reader = new StreamReader(new FileStream(
                    fullName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite));
                LastReadOffset = Reader.BaseStream.Length;

                LOGGER.Debug(
                    "Found existing CMD file: " + fullName + 
                    " LastReadOffset (EOF) = " + LastReadOffset);
            }
        }

        private void OnWatcher_Created(object sender, FileSystemEventArgs e)
        {
            LOGGER.Debug("Found new CMD file: " + e.FullPath);

            // File created after startup - read everything
            Reader = new StreamReader(new FileStream(
                e.FullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite));
            
            LastReadOffset = 0;

            ReadCmdFile();
        }

        private void OnWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            LOGGER.Debug("CMD file changed: " + e.FullPath);
            ReadCmdFile();
        }

        private void ReadCmdFile()
        {
            if (LastReadOffset == Reader.BaseStream.Length)
            {
                // No file length change so ignore
                LOGGER.Debug("CMD file changed with no length difference: LastReadOffset = " + LastReadOffset);
                return;
            }

            if (LastReadOffset > Reader.BaseStream.Length)
            {
                // The contents of the command file have been reset
                // Start reading from beginning again
                LOGGER.Warn(
                    "Contents of CMD file reset! LastReadOffset = " + LastReadOffset +
                    " Reader.BaseStream.Length = " + Reader.BaseStream.Length);

                LastReadOffset = 0;
            }

            Reader.BaseStream.Seek(LastReadOffset, SeekOrigin.Begin);

            string changedLine;
            while ((changedLine = Reader.ReadLine()) != null)
            {
                Tube.PlayMacro(changedLine);
                LOGGER.Debug(changedLine);
            }

            LastReadOffset = Reader.BaseStream.Position;
            LOGGER.Debug("Finished reading CMD file with LastReadOffset = " + LastReadOffset);

            if (Reader.BaseStream.Position != Reader.BaseStream.Position)
            {
                LOGGER.Warn("LastReadOffset != Position == " + Reader.BaseStream.Position);
            }
        }

        public void Dispose()
        {
            Watcher.Dispose();

            if (null != Reader)
            {
                Reader.Close();
                Reader.Dispose();
            }
        }
    }
}
