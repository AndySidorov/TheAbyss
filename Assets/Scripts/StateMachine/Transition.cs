public class Transition
{
    public IState To { get; private set; }
    public Condition Condition { get; private set; }

    public Transition(IState to, Condition condition)
    {
        To = to;
        Condition = condition;
    }
}
