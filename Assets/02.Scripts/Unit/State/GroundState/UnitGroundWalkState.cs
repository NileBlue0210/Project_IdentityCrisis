using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroundWalkState : UnitGroundState
{
    public UnitGroundWalkState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitGroundWalkState Enter");

        // 애니메이션 파라미터 설정
        stateMachine.Unit.UnitAnimator.SetBool("IsWalk", true);
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitGroundWalkState Exit");

        // 애니메이션 파라미터 설정
        stateMachine.Unit.UnitAnimator.SetBool("IsWalk", false);
        stateMachine.Unit.UnitAnimator.SetFloat("VelocityX", 0f);
    }

    public override void Update()
    {
        base.Update();

        Vector2 moveInput = stateMachine.PlayerInputActions.Unit.Move.ReadValue<Vector2>();

        // 이동 입력이 없을 경우, groundIdle상태로 변경
        if (moveInput.magnitude < 0.1f)
        {
            stateMachine.ChangeUnitState(stateMachine.GroundIdleState);

            return;
        }

        Vector2 moveDirection = new Vector2(moveInput.x * stateMachine.Unit.MoveSpeed, stateMachine.Unit.UnitController.Velocity.y);
        stateMachine.transform.Translate(moveDirection);

        // 애니메이션 파라미터 설정
        stateMachine.Unit.UnitAnimator.SetFloat("VelocityX", moveInput.x);
    }
}
