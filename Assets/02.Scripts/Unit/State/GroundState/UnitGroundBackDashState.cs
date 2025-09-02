using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 백대시 상태를 제어하는 클래스
/// to do : 백대시도 대시처럼 타입 구분을 해도 재미있을 듯 하니 염두해 둘 것
/// </summary>
public class UnitGroundBackDashState : UnitState
{
    private float backDashStartTime;    // 백대시 시작 시간

    public UnitGroundBackDashState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitGroundBackDashState Enter");

        if (stateMachine.CurrentState == stateMachine.AerialState)
            return; // 공중 상태에서 대시 상태로 진입하는 경우 무시

        stateMachine.Unit.UnitController.IsBackDash = true; // 백대시 중 플래그 활성화

        BackDash();
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitGroundBackDashState Exit");

        stateMachine.Unit.UnitController.IsBackDash = false;    // 백대시 중 플래그 비활성화
    }

    public override void Update()
    {
        base.Update();

        if (!stateMachine.Unit.UnitController.IsBackDash)
            return;

        // 백대시 거리만큼 이동한 후 백대시 상태 종료
        if (Time.time - backDashStartTime >= stateMachine.Unit.DashDuration)
        {
            EndBackDash();
        }
    }

    private void BackDash()
    {
        backDashStartTime = Time.time;

        float dashSpeed = stateMachine.Unit.BackDashSpeed;

        stateMachine.Unit.UnitController.Velocity = new Vector3(stateMachine.Unit.UnitController.BackDashDirection * dashSpeed, 0, 0); // 백대시 방향은 현재 바라보는 방향의 반대

        stateMachine.StartCoroutine(stateMachine.Unit.UnitController.BackDashCoroutine()); // 백대시 코루틴 실행
        stateMachine.Unit.UnitAnimator.SetTrigger("IsGroundDash"); // 백대시 애니메이션 재생
    }

    public void EndBackDash()
    {
        stateMachine.Unit.UnitController.IsBackDash = false;    // 백대시 중 플래그 비활성화

        stateMachine.ChangeUnitState(stateMachine.GroundState); // 대시 이후 대시 종료 후 지상 상태로 전환
    }
}
