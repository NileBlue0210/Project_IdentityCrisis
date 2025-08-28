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

        Jump();
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitJumpState Exit");
    }

    public override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 유닛 점프 로직
    /// </summary>
    private void Jump()
    {
        Vector2 moveInput = stateMachine.PlayerInputActions.Unit.Move.ReadValue<Vector2>(); // 대각선 점프를 위한 플레이어 좌 우 이동 입력 감지

        float HorizontalJumpDirection = 0f;

        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            HorizontalJumpDirection = Mathf.Sign(moveInput.x) * stateMachine.Unit.UnitController.HorizontalJumpSpeed;   // Sign함수를 통해 좌 우 입력에 따라 x값 보정
        }

        stateMachine.Unit.UnitController.Velocity = new Vector2(
            HorizontalJumpDirection,
            stateMachine.Unit.UnitController.JumpForce
        );
    }
}
