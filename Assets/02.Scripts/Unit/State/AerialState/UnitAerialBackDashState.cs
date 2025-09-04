using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAerialBackDashState : UnitAerialState
{
    private float backDashStartTime;    // 백대시 시작 시간

    public UnitAerialBackDashState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitAerialBackDashState Enter");

        stateMachine.Unit.UnitController.IsAerialBackDash = true; // 백대시 중 플래그 활성화

        // 애니메이션 재생 및 애니메이션 파라미터 세팅
        stateMachine.Unit.UnitAnimator.SetBool("AerialBackDash", stateMachine.Unit.UnitController.IsAerialBackDash);

        AerialBackDash();
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitAerialBackDashState Exit");

        stateMachine.Unit.UnitController.IsAerialBackDash = false;    // 백대시 중 플래그 비활성화

        // 애니메이션 파라미터 리셋
        stateMachine.Unit.UnitAnimator.SetBool("AerialBackDash", stateMachine.Unit.UnitController.IsAerialBackDash);
    }

    public override void Update()
    {
        base.Update();

        if (!stateMachine.Unit.UnitController.IsAerialBackDash)
            return;

        // 백대시 거리만큼 이동한 후 백대시 상태 종료
        if (Time.time - backDashStartTime >= stateMachine.Unit.DashDuration)
        {
            EndAerialBackDash();
        }
    }

    private void AerialBackDash()
    {
        backDashStartTime = Time.time;

        float backDashSpeed = stateMachine.Unit.BackDashSpeed;

        stateMachine.Unit.UnitController.Velocity = new Vector2(stateMachine.Unit.UnitController.BackDashDirection * backDashSpeed, 0);

        stateMachine.StartCoroutine(stateMachine.Unit.UnitController.AerialBackDashCoroutine());

        stateMachine.Unit.UnitController.CurrentAerialDashCount++; // 공중 대시 혹은 백대시 횟수 증가
    }

    public void EndAerialBackDash()
    {
        stateMachine.Unit.UnitController.Velocity = Vector3.zero;   // 대시 종료 후 캐릭터의 속력을 초기화

        stateMachine.ChangeUnitState(stateMachine.JumpState); // 백대시 종료 후 점프 상태로 전환
    }
}
