using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace Glue.Actions
{
    public class ActionCmd : Action
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string cmd;
        private string finishedSoundPath;

        [JsonProperty]
        public string Cmd
        {
            get => this.cmd;
            set => this.cmd = value;
        }

        [JsonProperty]
        public string FinishedSoundPath
        {
            get => this.finishedSoundPath;
            set => this.finishedSoundPath = value;
        }

        [JsonConstructor]
        public ActionCmd(long timeDelayMS, string cmd) : base(timeDelayMS)
        {
            this.Cmd = cmd;
            this.Type = ActionType.Cmd;
        }

        public ActionCmd(long timeDelayMS, string cmd, string finishedSoundPath) : base(timeDelayMS)
        {
            this.Cmd = cmd;
            this.Type = ActionType.Cmd;
            this.FinishedSoundPath = finishedSoundPath;
        }

        public override void Play()
        {
            LOGGER.Info("Executing CMD.EXE with: /C " + Cmd);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",               // see cmd.exe /? for more details on argument tweaking
                Arguments = "/C " + Cmd,            // carries out the command and terminates process
                RedirectStandardInput = true,       // stdin <- System.Diagnostics.Process.StandardInput
                RedirectStandardOutput = true,      // stdout -> System.Diagnostics.Process.StandardOutput
                RedirectStandardError = true,       // stderr -> System.Diagnostics.Process.StandardError`
                CreateNoWindow = true,              // Operate silently - Glue handles/displays output
                UseShellExecute = false,            // Spawned exe creates separate process
            };

            process.StartInfo = startInfo;
            process.Start();

            // This thread will be paused while the command executes
            // TODO Spawned process from ActionCmd could block thread in queue and needs timeout.
            process.WaitForExit();

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            string newline = Environment.NewLine;

            string message =
                newline +
                "CMD.EXE " + process.StartInfo.Arguments + " returned(" + process.ExitCode + ") " + newline +
                stdout;

            // An error was returned - not very common case as most processes 
            // are lazy and report errors on stdout
            if (stderr.Length > 0)
            {
                message += 
                "STDEERR! : " + newline +
                stderr + newline;
            }

            LOGGER.Info(message);

            message = message.Insert(0, "ActionCmd execute ");
            Tube.LogToGUI(message);

            // Play sound indicating finished if one is specified
            if (null != finishedSoundPath && finishedSoundPath.Length > 0)
            {
                ActionSound.GetPlayer(finishedSoundPath).Play();
            }
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            ActionCmd scheduledCopy = new ActionCmd(this.DelayMS, this.Cmd, this.FinishedSoundPath)
            {
                ScheduledTick = timeScheduleFrom + this.DelayMS
            };

            return new Action[] { scheduledCopy };
        }

        public override string ToString()
        {
            return base.ToString() + " " + this.Cmd;
        }
    }
}
