using System;

namespace Assets.Scripts.Classes.Events
{
    public delegate void ProgressEvent(object sender, ProgressEventArgs args);

    public class ProgressEventArgs : EventArgs
    {
        public float Progress { get; }

        public ProgressEventArgs(float newProgress)
        {
            Progress = newProgress;
        }
    }
}
