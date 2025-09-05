using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAerialDashState : UnitAerialState
{
    private float dashStartTime;    // 대시 시작 시간

    public UnitAerialDashState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitAerialDashState Enter");

        stateMachine.Unit.UnitController.IsAerialDash = true; // 공중 대시 혹은 백대시 중 플래그 활성화

        // 애니메이션 재생 및 애니메이션 파라미터 세팅
        stateMachine.Unit.UnitAnimator.SetBool("AerialDash", stateMachine.Unit.UnitController.IsAerialDash);

        AerialDash();
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitAerialDashState Exit");

        stateMachine.Unit.UnitController.IsAerialDash = false;    // 대시 중 플래그 비활성화

        // 애니메이션 파라미터 리셋
        stateMachine.Unit.UnitAnimator.SetBool("AerialDash", stateMachine.Unit.UnitController.IsAerialDash);
    }

    public override void Update()
    {
        base.Update();

        if (!stateMachine.Unit.UnitController.IsAerialDash)
            return;

        // 대시 거리만큼 이동한 후 대시 상태 종료
        if (Time.time - dashStartTime >= stateMachine.Unit.AerialDashDuration)
        {
            EndAerialDash();
        }
    }

    private void AerialDash()
    {
        dashStartTime = Time.time;

        float dashSpeed = stateMachine.Unit.AerialDashSpeed;

        stateMachine.Unit.UnitController.Velocity = new Vector2(stateMachine.Unit.UnitController.DashDirection * dashSpeed, 0);

        stateMachine.StartCoroutine(stateMachine.Unit.UnitController.AerialDashCoroutine());

        stateMachine.Unit.UnitController.CurrentAerialDashCount++; // 공중 대시 혹은 백대시 횟수 증가
    }

    public void EndAerialDash()
    {
        stateMachine.Unit.UnitController.Velocity = Vector3.zero;   // 대시 종료 후 캐릭터의 속력을 초기화

        stateMachine.ChangeUnitState(stateMachine.JumpState); // 공중 대시 이후 점프 상태로 자동 전환
    }
}
