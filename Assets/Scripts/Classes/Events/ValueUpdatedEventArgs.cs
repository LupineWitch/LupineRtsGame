public class ValueUpdatedEventArgs
{
    public float ChangedTo { get => changedTo; }

    private float changedTo;

    public ValueUpdatedEventArgs(float changedTo)
    {
        this.changedTo = changedTo;
    }
}
