using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 공격 입력 타입 ( 평타, 스킬, 잡기 등 )
/// </summary>
public enum EAttackInputType
{
    NormalAttack,
    SkillAttack
}

/// <summary>
/// 유닛의 공격 상태를 관리하는 상태 클래스
/// 지상, 공중 상태와 같은 상위 상태에서 파생되어 전환 가능
/// </summary>
public class UnitAttackState : UnitState
{
    private IUnitState currentBaseState; // 중첩할 현재 상태
    private UnitAttackSubState currentAttackSubState; // 현재 하위 공격 상태
    public bool IsAttacking { get; set; } // 공격 상태 플래그
    public UnitStateMachine StateMachine { get { return stateMachine; } }   // 하위 상태가 접근할 StateMachine 프로퍼티

    public UnitAttackState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        currentAttackSubState?.Exit();
        IsAttacking = false;
    }

    public override void Update()
    {
        if (!IsAttacking)
            return;

        base.Update();

        currentAttackSubState?.Update();
    }

    public void ChangeBaseState()
    {
        currentBaseState = stateMachine.CurrentState;
    }

    public void OnAttackInputDetected(UnitAttackSubState attackSubState)
    {
        ChangeBaseState();  // 현재 상태 갱신

        if (!CheckChangeStateAvailable(attackSubState))
            return;

        IsAttacking = true;

        // 하위 공격 상태 전환
        currentAttackSubState?.Exit();
        currentAttackSubState = attackSubState;
        currentAttackSubState.Enter();
    }

    /// <summary>
    /// 특정 하위 공격 상태로의 전환이 가능한지 여부를 판단하는 메소드
    /// </summary>
    /// <param name="targetState"></param>
    /// <returns></returns>
    public bool CheckChangeStateAvailable(UnitAttackSubState targetState)
    {
        return targetState.ShiftableFromStates(currentBaseState);
    }
}
