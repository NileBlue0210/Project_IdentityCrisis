using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 지상 상태를 관리하는 상태 클래스
/// </summary>
public class UnitGroundState : UnitState
{
    public UnitGroundState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        // 착지 직후 남아있던 수평 이동 값을 초기화
        stateMachine.Unit.UnitController.Velocity = new Vector3(
            0f,
            stateMachine.Unit.UnitController.Velocity.y,
            0f
        );

        stateMachine.Unit.UnitController.CurrentJumpCount = 0; // 착지 시 현재 점프 횟수 초기화
        stateMachine.Unit.UnitController.CurrentAerialDashCount = 0; // 착지 시 현재 공중 대시 횟수 초기화

        // 지상 상태에 진입할 때 애니메이터의 IsGrounded 값을 true로 설정
        stateMachine.Unit.UnitAnimator.SetBool("IsGrounded", true);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        Vector2 moveInput = stateMachine.PlayerInputActions.Unit.Move.ReadValue<Vector2>(); // Move Action에 매핑된 키가 감지되었을 때 입력값을 Vector2 형태로 읽어온다

        if (moveInput.magnitude > 0.1f)
        {
            // Move Action의 이벤트가 감지되었을 경우, groundWalk 상태로 전환한다
            stateMachine.ChangeUnitState(stateMachine.GroundWalkState);
        }
        else
        {
            // Move Action의 이벤트가 감지되지 않을 경우, groundIdle 상태로 전환한다
            stateMachine.ChangeUnitState(stateMachine.GroundIdleState);
        }
    }
}
