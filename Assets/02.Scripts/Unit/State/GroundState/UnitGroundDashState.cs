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
    private int dashDirection;   // 대시 방향
    private float dashStartTime;    // 대시 시작 시간
    private UnitDashType dashType;  // 대시 타입

    public UnitGroundDashState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitGroundDashState Enter");
        
        dashType = stateMachine.Unit.DashType;
        stateMachine.Unit.UnitController.IsDash = true; // 대시 중 플래그 활성화

        if (dashType == UnitDashType.Dash)
        {
            Dash();
        }
        else if (dashType == UnitDashType.Run)
        {
            Run();
        }
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitGroundDashState Exit");

        stateMachine.Unit.UnitController.IsDash = false;    // 대시 중 플래그 비활성화
    }

    public override void Update()
    {
        base.Update();

        if (!stateMachine.Unit.UnitController.IsDash)
            return;

        if (dashType == UnitDashType.Dash)
        {
            // 대시 거리만큼 이동한 후 대시 상태 종료
            if (Time.time - dashStartTime >= stateMachine.Unit.DashDuration)
            {
                EndDash();
            }
        }
        else if (dashType == UnitDashType.Run)
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

        stateMachine.Unit.UnitController.Velocity = new Vector3(dashDirection * dashSpeed, 0, 0);

        stateMachine.StartCoroutine(stateMachine.Unit.UnitController.DashCoroutine());
    }

    public void EndDash()
    {
        stateMachine.Unit.UnitController.Velocity = Vector3.zero;   // 대시 종료 후 캐릭터의 속력을 초기화

        stateMachine.ChangeUnitState(stateMachine.GroundState); // 대시 이후 대시 상태 자동 종료
    }

    private void Run()
    {
        float runSpeed = ( stateMachine.Unit.MoveSpeed * 100 ) * stateMachine.Unit.DashSpeed;
        stateMachine.Unit.UnitController.Velocity = new Vector3(dashDirection * runSpeed, 0, 0);
    }

    private void EndRun()
    {
        stateMachine.Unit.UnitController.Velocity = Vector3.zero;   // 달리기 종료 후 캐릭터의 속력을 초기화

        stateMachine.ChangeUnitState(stateMachine.GroundState);
    }

    public void OnDashInputDetected(string id, int tapCount, int direction)
    {
        int dir = direction;

        if (dir == 0)
        {
            var InputDirectionX = GameManager.Instance.GetManager<InputManager>(typeof(InputManager)).PlayerInputActions.Unit.Move.ReadValue<Vector2>().x;

            dir = InputDirectionX > 0 ? 1 : (InputDirectionX < 0 ? -1 : (int)stateMachine.transform.localScale.x);  // 입력값을 1, -1로 보정
        }

        int facing = (int)Mathf.Sign(stateMachine.transform.localScale.x);  // 캐릭터가 바라보는 방향
        bool isForward = Mathf.Sign(dir) == Mathf.Sign(facing); // 입력한 방향이 캐릭터가 바라보는 방향과 일치하는가를 판단하는 플래그

        if (isForward)
        {
            // 대시 혹은 달리기 시행
            dashDirection = dir;
            stateMachine.ChangeUnitState(stateMachine.GroundDashState);
        }
        else
        {
            // 백대시 시행
            dashDirection = dir;
            stateMachine.ChangeUnitState(stateMachine.GroundBackDashState);
        }
    }
}
