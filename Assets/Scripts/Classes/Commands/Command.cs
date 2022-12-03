using Assets.Scripts.Classes.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Classes.Commands
{
    public enum CommandResult
    {
        NoResult = 0,
        Success,
        Cancelled,
        Interrupted,
        Failed
    }
    
    public enum CommandState
    {
        Cold = 0,
        Queued,
        Starting,
        InProgress,
        Ending,
        Ended
    }

    public abstract class Command<RecieverType>
    {
        private RecieverType reciver;
        private CommandState state = CommandState.Cold;
        private CommandResult result = CommandResult.NoResult;
        protected Queue<Command<RecieverType>> steps;
        protected Command<RecieverType> currentStep;

        public virtual CommandState GetCurentState => this.state;

        public virtual CommandResult GetCommandResult => state.IsActiveState() ? CommandResult.NoResult : result;

        public RecieverType GetReciver => reciver;

        protected Command(RecieverType reciver)
        {
            this.reciver = reciver;
        }
        
        protected Command(RecieverType reciver, Queue<Command<RecieverType>> steps)
        {
            this.reciver = reciver;
            this.steps = steps;
        }

        protected virtual void SetCurentState(CommandState state)
        {
            this.state = state;
        }

        protected virtual void SetCommandResult(CommandResult resultToSet)
        {
            this.result = resultToSet;
        }

        public virtual void StartCommand()
        {
            this.state = CommandState.Starting;

            if( currentStep == null || (currentStep.GetCurentState == default || currentStep.GetCurentState == CommandState.Ended))
            {
                this.state = CommandState.Ended;
                return;
            }

            if( !steps.TryDequeue(out Command<RecieverType> firstStep))
            {
                this.state = CommandState.Ended;
                return;
            }

            this.currentStep = firstStep;
            this.state = CommandState.InProgress;
        }

        public virtual CommandState ExecuteNextStep()
        {
            if (!steps.TryDequeue(out Command<RecieverType> nextStep))
            {
                result = EndCommand();
                return CommandState.Ended;
            }

            currentStep = nextStep;
            return CommandState.InProgress;
        }

        public abstract CommandState ExecuteOnUpdate();

        public virtual CommandResult Cancel()
        {
            steps.Clear();
            return CommandResult.Cancelled;
        }

        public virtual CommandResult EndCommand()
        {
            SetCurentState(CommandState.Ended);
            return CommandResult.Success;
        }
        
        public virtual CommandResult EndCommand(CommandResult withResult)
        {
            EndCommand();
            return withResult;
        }
    }
}
