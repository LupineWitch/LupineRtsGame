using Assets.Scripts.Commandables.Directives;

public class BasicUnitScript : EntityBase
{
    public float unitSpeed = 10f;


    protected virtual void Start()
    {
        defaultDirective = new ContextDirective();
        menuActions[0] = new MoveDirective();
    }
}
