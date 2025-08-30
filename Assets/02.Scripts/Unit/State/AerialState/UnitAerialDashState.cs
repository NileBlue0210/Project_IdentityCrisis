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

        AerialDash();
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitAerialDashState Exit");

        stateMachine.Unit.UnitController.IsAerialDash = false;    // 대시 중 플래그 비활성화
    }

    public override void Update()
    {
        base.Update();

        if (!stateMachine.Unit.UnitController.IsAerialDash)
            return;

        // 대시 거리만큼 이동한 후 대시 상태 종료
        if (Time.time - dashStartTime >= stateMachine.Unit.DashDuration)
        {
            EndAerialDash();
        }
    }

    private void AerialDash()
    {
        dashStartTime = Time.time;

        float dashSpeed = stateMachine.Unit.DashSpeed;

        stateMachine.Unit.UnitController.Velocity = new Vector2(stateMachine.Unit.UnitController.DashDirection * dashSpeed, 0);

        stateMachine.StartCoroutine(stateMachine.Unit.UnitController.AerialDashCoroutine());

        stateMachine.Unit.UnitController.CurrentAerialDashCount++; // 공중 대시 혹은 백대시 횟수 증가
    }

    public void EndAerialDash()
    {
        stateMachine.Unit.UnitController.Velocity = Vector3.zero;   // 대시 종료 후 캐릭터의 속력을 초기화

        stateMachine.ChangeUnitState(stateMachine.JumpState); // 공중 대시 이후 점프 상태로 자동 전환
    }

    public void OnDashInputDetected(string id, int tapCount, int direction)
    {
        if (!stateMachine.CheckChangeStateAvailable(stateMachine.AerialDashState))
            return;

        if (!stateMachine.Unit.UnitController.CheckAerialDashAvailable())
            return;

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
            // 공중 대시 시행
            stateMachine.Unit.UnitController.DashDirection = dir;
            stateMachine.ChangeUnitState(stateMachine.AerialDashState);
        }
        else
        {
            // 공중 백대시 시행
            stateMachine.Unit.UnitController.BackDashDirection = dir;
            stateMachine.ChangeUnitState(stateMachine.AerialBackDashState);
        }
    }
}
