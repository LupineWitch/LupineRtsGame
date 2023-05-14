using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Classes.Commands
{
    public enum CommandResult
    {
        Invalid = -1,
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

    public abstract class Command<SenderType, RecieverType>
    {
        public event Action<object, CommandResult> CommandFinished;

        public CommandState CurrentState
        {
            get => _currentState; 
            protected set
            {
                _currentState = value;
                if (currentStateCallback != null)
                    currentStateCallback(_currentState);
                else
                    Debug.LogWarning("Changed command state without callback!");
            }
        }
        public CommandResult CommandResult
        {
            get => _commandResult; 
            protected set
            {
                _commandResult = value;
                this.CommandFinished?.Invoke(this, value);
                if (commandResultCallback != null)
                    commandResultCallback(_commandResult);
                else
                    Debug.LogWarning("Changed command result without callback!");
            }
        }

        protected SenderType sender;
        protected RecieverType reciever;
        protected Action<CommandResult> commandResultCallback;
        protected Action<CommandState> currentStateCallback;

        private CommandResult _commandResult = CommandResult.Invalid;
        private CommandState _currentState = CommandState.Cold;

        protected Command(RecieverType recieverType, SenderType sender)
        {
            this.reciever = recieverType;
            this.sender = sender;
        }

        /// <summary>
        /// A coroutine which contains actions in given command
        /// </summary>
        /// <param name="resultCallback">A method group to execute when coroutine ends with result.</param>
        /// <param name="stateCallback">A method group to execute when state of executing command changes</param>
        /// <returns>A coroutine which contains action the command consits of.</returns>
        public abstract IEnumerator CommandCoroutine(Action<CommandResult> resultCallback, Action<CommandState> stateCallback);

        public IEnumerator CommandCoroutine()
        {
            return CommandCoroutine(SetResult, SetState);
        }

        private void SetState(CommandState state) => _currentState = state;

        private void SetResult(CommandResult result) => _commandResult = result;

        public virtual CommandState GetCurrentState() => CurrentState;

        public virtual CommandResult GetCommandResult() => CommandResult;

        public virtual CommandResult CancelCommand()
        {
            CommandResult = CommandResult.Cancelled;
            CurrentState = CommandState.Ended;
            return CommandResult;
        }
    }
}
