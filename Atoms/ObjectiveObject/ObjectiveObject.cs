using Godot;
using System;

public class ObjectiveObject : Node2D
{
    [Signal] public delegate void StateResolved(ObjectiveObject obj);
    public enum OBJECTIVE_STATE {
        PENDING,
        SUCCESS,
        FAILURE
    }
    public OBJECTIVE_STATE state = OBJECTIVE_STATE.PENDING;
    
    public bool IsPending() => state == OBJECTIVE_STATE.PENDING;
    public bool IsSucceeded() => state == OBJECTIVE_STATE.SUCCESS;
    public bool IsFailed() => state == OBJECTIVE_STATE.FAILURE;
    
    public virtual void Disable() { }
    
    public virtual void Enable() { }
    
    public void NotifySuccess()
    {
        state = OBJECTIVE_STATE.SUCCESS;
        EmitSignal(nameof(StateResolved), this);
    }
    
    public void NotifyFailure()
    {
        state = OBJECTIVE_STATE.FAILURE;
        EmitSignal(nameof(StateResolved), this);
    }
}
