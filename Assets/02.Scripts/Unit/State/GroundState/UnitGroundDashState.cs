using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 유닛의 대시, 달리기 상태를 제어하는 클래스
/// 캐릭터별로 대시 타입, 달리기 타입으로 나뉜다
/// 공중 사용과는 별개의 상태이다
/// </summary>
public class UnitGroundDashState : UnitState
{
    private float dashStartTime;    // 대시 시작 시간
    private EUnitDashType dashType;  // 대시 타입

    public UnitGroundDashState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitGroundDashState Enter");
        
        dashType = stateMachine.Unit.DashType;
        stateMachine.Unit.UnitController.IsDash = true; // 대시 중 플래그 활성화

        if (dashType == EUnitDashType.Dash)
        {
            Dash();
        }
        else if (dashType == EUnitDashType.Run)
        {
            Run();
        }
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitGroundDashState Exit");
    }

    public override void Update()
    {
        base.Update();

        if (!stateMachine.Unit.UnitController.IsDash)
            return;

        if (dashType == EUnitDashType.Dash)
        {
            // 대시 거리만큼 이동한 후 대시 상태 종료
            if (Time.time - dashStartTime >= stateMachine.Unit.DashDuration)
            {
                EndDash();
            }
        }
        else if (dashType == EUnitDashType.Run)
        {
            float runInput = stateMachine.PlayerInputActions.Unit.Move.ReadValue<Vector2>().x;

            // 대시 입력을 유지하지 않거나, 입력 방향이 바뀌었을 경우 달리기 상태 종료
            if (Mathf.Abs(runInput) < 0.1f || Mathf.Sign(runInput) != Mathf.Sign(stateMachine.Unit.UnitController.Velocity.x))
            {
                EndRun();
            }
        }
    }

    private void Dash()
    {
        dashStartTime = Time.time;

        float dashSpeed = stateMachine.Unit.DashSpeed;

        stateMachine.Unit.UnitController.Velocity = new Vector3(stateMachine.Unit.UnitController.DashDirection * dashSpeed, 0, 0);

        // 대시 애니메이션 재생
        stateMachine.Unit.UnitAnimator.SetBool("Dash", stateMachine.Unit.UnitController.IsDash);

        stateMachine.StartCoroutine(stateMachine.Unit.UnitController.DashCoroutine());
    }

    public void EndDash()
    {
        stateMachine.Unit.UnitController.Velocity = Vector3.zero;   // 대시 종료 후 캐릭터의 속력을 초기화

        stateMachine.Unit.UnitController.IsDash = false;    // 대시 중 플래그 비활성화

        stateMachine.Unit.UnitAnimator.SetBool("Dash", stateMachine.Unit.UnitController.IsDash);

        stateMachine.ChangeUnitState(stateMachine.GroundState); // 대시 이후 대시 상태 자동 종료
    }

    private void Run()
    {
        float runSpeed = ( stateMachine.Unit.MoveSpeed * 100 ) * stateMachine.Unit.DashSpeed;
        stateMachine.Unit.UnitController.Velocity = new Vector3(stateMachine.Unit.UnitController.DashDirection * runSpeed, 0, 0);
    }

    private void EndRun()
    {
        stateMachine.Unit.UnitController.Velocity = Vector3.zero;   // 달리기 종료 후 캐릭터의 속력을 초기화

        stateMachine.Unit.UnitController.IsDash = false;    // 대시 중 플래그 비활성화

        stateMachine.ChangeUnitState(stateMachine.GroundState);
    }
}
