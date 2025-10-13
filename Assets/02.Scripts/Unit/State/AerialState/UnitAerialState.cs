using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 체공 상태를 관리하는 상태 클래스
/// </summary>
public class UnitAerialState : UnitState
{
    public UnitAerialState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        // 체공 상태에 진입할 때 애니메이터의 IsGrounded 값을 false로 설정
        stateMachine.Unit.UnitAnimator.SetBool("IsGrounded", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 유닛이 지면에 닿았을 경우, 지상 상태로 전환
        if (stateMachine.Unit.UnitController.IsGrounded())
        {
            stateMachine.ChangeUnitState(stateMachine.GroundState);
        }
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
