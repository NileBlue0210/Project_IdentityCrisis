using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 지상 상태를 관리하는 상태 클래스
/// </summary>
public class UnitGroundState : UnitState
{
    public UnitGroundState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        // 착지 직후 남아있던 수평 이동 값을 초기화
        stateMachine.Unit.UnitController.Velocity = new Vector3(
            0f,
            stateMachine.Unit.UnitController.Velocity.y,
            0f
        );

        stateMachine.Unit.UnitController.CurrentJumpCount = 0; // 착지 시 현재 점프 횟수 초기화
        stateMachine.Unit.UnitController.CurrentAerialDashCount = 0; // 착지 시 현재 공중 대시 횟수 초기화

        // 지상 상태에 진입할 때 애니메이터의 IsGrounded 값을 true로 설정
        stateMachine.Unit.UnitAnimator.SetBool("IsGrounded", true);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        Vector2 moveInput = stateMachine.PlayerInputActions.Unit.Move.ReadValue<Vector2>(); // Move Action에 매핑된 키가 감지되었을 때 입력값을 Vector2 형태로 읽어온다

        if (moveInput.magnitude > 0.1f)
        {
            // Move Action의 이벤트가 감지되었을 경우, groundWalk 상태로 전환한다
            stateMachine.ChangeUnitState(stateMachine.GroundWalkState);
        }
        else
        {
            // Move Action의 이벤트가 감지되지 않을 경우, groundIdle 상태로 전환한다
            stateMachine.ChangeUnitState(stateMachine.GroundIdleState);
        }
    }

    public void OnDashInputDetected(string id, int tapCount, int direction)
    {
        if (!stateMachine.CheckChangeStateAvailable(stateMachine.GroundDashState))
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
            // 대시 혹은 달리기 시행
            stateMachine.Unit.UnitController.DashDirection = dir;
            stateMachine.ChangeUnitState(stateMachine.GroundDashState);
        }
        else
        {
            // 백대시 시행
            stateMachine.Unit.UnitController.BackDashDirection = dir;
            stateMachine.ChangeUnitState(stateMachine.GroundBackDashState);
        }
    }
}
