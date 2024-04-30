using System.Collections.Generic;

public class StateNode
{
    public IState State { get; private set; }
    public HashSet<Transition> Transitions{ get; private set; }

    public StateNode(IState state)
    {
        State = state;
        Transitions = new HashSet<Transition>();
    }

    public void AddTransition(IState to, Condition condition)
    {
        var newTransition = new Transition(to, condition);
        Transitions.Add(newTransition);
    }
}
