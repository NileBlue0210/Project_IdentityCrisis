using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HSM 구조를 위한 인터페이스
/// </summary>
public interface IUnitState
{
    void Enter();
    void Update();
    void Exit();
}

/// <summary>
/// 유닛의 상태를 관리하는 베이스 클래스
/// </summary>
public abstract class UnitState : IUnitState
{
    protected UnitStateMachine stateMachine;

    public UnitState(UnitStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {

    }
    
    public virtual void Update()
    {

    }
    
    public virtual void Exit()
    {

    }
}
