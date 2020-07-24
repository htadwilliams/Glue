using Newtonsoft.Json;

namespace Glue.Actions
{
    public class ActionCmd : Action
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string cmd;

        [JsonProperty]
        public string Cmd
        {
            get => this.cmd;
            set => this.cmd = value;
        }

        public ActionCmd(long timeDelayMS, string cmd) : base(timeDelayMS)
        {
            this.Cmd = cmd;
            this.Type = ActionType.Cmd;
        }

        public override void Play()
        {
            LOGGER.Info("Executing CMD.EXE with: /C " + Cmd);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C " + Cmd,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();
            LOGGER.Info("CMD.EXE returned: " + process.StandardOutput.ReadToEnd());
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            ActionCmd scheduledCopy = new ActionCmd(this.DelayMS, this.Cmd)
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
