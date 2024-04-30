using System;
using System.Collections.Generic;
using UnityEngine.XR;

public class StateMachine
{
    private Dictionary<Type, StateNode> _stateNodes = new ();
    private StateNode _current;

    public void AddState(IState state)
    {
        _stateNodes[state.GetType()] = new StateNode(state);
    }

    public void AddTransition(IState from, IState to, Condition condition)
    {
        var stateNode = _stateNodes[from.GetType()];
        stateNode?.AddTransition(to, condition);
    }

    public void Update()
    {
        CheckConditions();
        
        _current.State.Update();
    }

    private void CheckConditions()
    {
        foreach (var transition in _current.Transitions)
        {
            if (transition.Condition.Evaluate())
            {
                ChangeState(transition.To);
                break;
            }
        }
    }

    private void ChangeState(IState to)
    {
        _current.State.OnExit();
        _current = _stateNodes[to.GetType()];
        _current.State.OnEnter();
    }

    public void SetInitialState(IState state)
    {
        if (_current == null)
        {
            _current = _stateNodes[state.GetType()];
            _current.State.OnEnter();
        }
    }
}
