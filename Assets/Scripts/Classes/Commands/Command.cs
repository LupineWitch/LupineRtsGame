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
        public CommandState currentState { get => _currentState; protected set
            {
                _currentState = value;
                if (currentStateCallback != null)
                    currentStateCallback(_currentState);
                else
                    Debug.LogError("Changed command state without callback!");
            }
        }
        public CommandResult commandResult
        {
            get => _commandResult; protected set
            {
                _commandResult = value;
                if(commandResultCallback != null)
                    commandResultCallback(_commandResult);
                else
                    Debug.LogError("Changed command result without callback!");
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

        public virtual IEnumerator CommandCoroutine()
        {
            return CommandCoroutine(SetResult, SetState);
        }

        private void SetState(CommandState state) => this.currentState = state;

        private void SetResult(CommandResult result) => this.commandResult = result;

        public virtual CommandState GetCurrentState() => currentState;

        public virtual CommandResult GetCommandResult() => commandResult;
    }
}
