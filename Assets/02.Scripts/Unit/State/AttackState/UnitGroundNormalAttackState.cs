using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 지상 평타 공격 상태를 관리하는 하위 공격 상태 클래스
/// </summary>
public class UnitGroundNormalAttackState : UnitAttackSubState
{
    public override List<System.Type> ShiftableFromStatesList { get; set; } = new List<System.Type>
    {
        // 지상 평타 공격으로 전환 가능한 상위 상태들
        typeof(UnitGroundIdleState),
        typeof(UnitGroundWalkState),
        typeof(UnitGroundDashState)
    };

    public UnitGroundNormalAttackState(UnitAttackState attackState) : base(attackState)
    {

    }

    public override bool ShiftableFromStates(IUnitState currentState)
    {
        bool result = ShiftableFromStatesList.Contains(currentState.GetType());

        return result;
    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("check animator : " + attackState.StateMachine.UnitAnimationController.Animator);

        // 지상 평타 공격 애니메이션 재생
        attackState.StateMachine.UnitAnimationController.Animator.SetTrigger("Punch");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
