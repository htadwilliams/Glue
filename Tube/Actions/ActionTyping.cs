﻿using System;
using System.Collections.Generic;
using Glue.Events;
using Newtonsoft.Json;
using WindowsInput;
using WindowsInput.Native;

namespace Glue.Actions
{
    public class ActionTyping : Action
    {
        public string TypedString => this.typedString;
        public long CharDelayMS => this.charDelayMS;
        public long DwellTimeMS => this.dwellTimeMS;

        [JsonProperty]
        private readonly string typedString;
        [JsonProperty]
        private readonly long charDelayMS;
        [JsonProperty]
        private readonly long dwellTimeMS;

        public ActionTyping(string typedString, long keyDelayMS, long dwellTimeMS) : base(0)
        {
            this.typedString=typedString;
            this.charDelayMS=keyDelayMS;
            this.dwellTimeMS=dwellTimeMS;
            this.Type = ActionType.Typing;
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            // TODO ActionTyping.Schedule() needs a complete re-write for filter-driver injection. 
            List<Action> actions = new List<Action>();

            // TODO INPUT generated with AddCharacters(char) or AddCharacters(string) cause PACKET to be displayed by keyboard hook
            INPUT[] inputs = new InputBuilder().AddCharacters(this.typedString).ToArray();

            int actionCount = 0;
            long actionTimeMS = this.ScheduledTick + timeScheduleFrom;

            foreach (INPUT input in inputs)
            {
                Action action = new ActionKey(
                    actionTimeMS,
                    input, 
                    (actionCount % 2) == 0 
                        ? ButtonStates.Press
                        : ButtonStates.Release
                    );

                actions.Add(action);

                actionTimeMS += 
                    (actionCount % 2) == 0 
                        ? this.charDelayMS 
                        : this.dwellTimeMS;

                actionCount++;
            }

            return actions.ToArray();
        }

        public override void Play()
        {
            // ActionTyping is only used to schedule ActionKey 
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString() + " " + this.typedString;
        }
    }
}
