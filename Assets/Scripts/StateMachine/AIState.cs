using UnityEngine.AI;

public abstract class AIState : IState
{
    protected NavMeshAgent _agent;

    protected AIState(NavMeshAgent agent)
    {
        _agent = agent;
    }
    
    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void Update()
    {
    }
}
