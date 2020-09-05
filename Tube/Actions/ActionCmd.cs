using Glue.Events;
using NerfDX.Events;
using Newtonsoft.Json;
using System;

namespace Glue.Actions
{
    public class ActionCmd : Action
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string cmd;
        private string finishedSoundPath;
        private string errorSoundPath;

        // Required
        [JsonProperty]
        public string Cmd
        {
            get => this.cmd;
            set => this.cmd = value;
        }

        // Optional
        [JsonProperty]
        public string FinishedSoundPath
        {
            get => this.finishedSoundPath;
            set => this.finishedSoundPath = value;
        }

        // Optional
        [JsonProperty]
        public string ErrorSoundPath
        {
            get => this.errorSoundPath;
            set => this.errorSoundPath = value;
        }

        [JsonConstructor]
        public ActionCmd(long timeDelayMS, string cmd) : base(timeDelayMS)
        {
            this.Cmd = cmd;
            this.Type = ActionType.Cmd;
        }

        public override void Play()
        {
            string arguments = "/C " + Cmd;         // carries out the command and terminates process
            string nl = Environment.NewLine;

            string message = "Calling CMD.EXE " + arguments;
            LOGGER.Info(message);
            EventBus<EventUserInfo>.Instance.SendEvent(
                this,
                new EventUserInfo(nl + "ActionCmd " + message + " ..." + nl));

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",               // see cmd.exe /? for more details on argument tweaking
                Arguments = arguments,              // 
                RedirectStandardInput = true,       // stdin <- System.Diagnostics.Process.StandardInput
                RedirectStandardOutput = true,      // stdout -> System.Diagnostics.Process.StandardOutput
                RedirectStandardError = true,       // stderr -> System.Diagnostics.Process.StandardError`
                CreateNoWindow = true,              // Operate silently - Glue handles/displays output
                UseShellExecute = false,            // Spawned exe creates separate process
            };

            process.StartInfo = startInfo;
            process.Start();

            // This thread will be blocked while the command executes
            process.WaitForExit();

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            message =
                nl + nl +
                "CMD.EXE returned (" + process.ExitCode + ") " 
                + nl + nl + stdout + nl;

            // Handle stuff from stderr
            if (stderr.Length > 0)
            {
                message += 
                "STDEERR! : " + nl +
                stderr + nl;
            }

            LOGGER.Info(message);

            EventBus<EventUserInfo>.Instance.SendEvent(
                this,
                new EventUserInfo(message));

            // Play sound indicating finish and error status if specified
            if (process.ExitCode == 0)
            {
                if (null != finishedSoundPath && finishedSoundPath.Length > 0)
                {
                    ActionSound.GetPlayer(finishedSoundPath)?.Play();
                }
            }
            else
            {
                if (null != errorSoundPath && errorSoundPath.Length > 0)
                {
                    ActionSound.GetPlayer(errorSoundPath)?.Play();
                }
            }
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            ActionCmd scheduledCopy = new ActionCmd(this.DelayMS, this.Cmd)
            {
                ScheduledTick = timeScheduleFrom + this.DelayMS,
                FinishedSoundPath = this.FinishedSoundPath,
                ErrorSoundPath = this.ErrorSoundPath,
            };

            return new Action[] { scheduledCopy };
        }

        public override string ToString()
        {
            return base.ToString() + " " + this.Cmd;
        }
    }
}
