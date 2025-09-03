using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitJumpState : UnitAerialState
{
    public UnitJumpState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitJumpState Enter");

        // 점프 애니메이면 재생
        stateMachine.Unit.UnitAnimator.SetTrigger("Jump");
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitJumpState Exit");

        // 점프 상태를 벗어날 때 애니메이터의 Y축 파라미터를 0으로 초기화
        stateMachine.Unit.UnitAnimator.SetFloat("VelocityY", 0f);
    }

    public override void Update()
    {
        base.Update();

        // 점프 애니메이션 블렌딩을 위해 Y축 파라미터 값을 업데이트
        stateMachine.Unit.UnitAnimator.SetFloat("VelocityY", stateMachine.Unit.UnitController.Velocity.y);
    }

    /// <summary>
    /// 유닛 점프 로직
    /// </summary>
    public void Jump()
    {
        // 최대 점프 횟수를 초과하거나, 점프 상태가 아닐 경우 점프 로직을 실행하지 않음
        if (stateMachine.CurrentState != stateMachine.JumpState || !stateMachine.Unit.UnitController.CheckAerialDashAvailable())
            return;

        Vector2 moveInput = stateMachine.PlayerInputActions.Unit.Move.ReadValue<Vector2>(); // 대각선 점프를 위한 플레이어 좌 우 이동 입력 감지

        float HorizontalJumpDirection = 0f;

        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            HorizontalJumpDirection = Mathf.Sign(moveInput.x) * stateMachine.Unit.HorizontalJumpSpeed;   // Sign함수를 통해 좌 우 입력에 따라 x값 보정
        }

        stateMachine.Unit.UnitController.Velocity = new Vector2(
            HorizontalJumpDirection,
            stateMachine.Unit.JumpForce
        );

        stateMachine.Unit.UnitController.CurrentJumpCount++; // 점프 횟수 증가
    }
}
