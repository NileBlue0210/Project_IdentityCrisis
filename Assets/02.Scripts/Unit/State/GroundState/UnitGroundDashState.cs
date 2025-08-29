using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// to do : 대시 스테이트 쪽에서 델리게이트에 대시 이벤트를 묶는 처리를 넣도록 하고, 대시 감지를 SequenceController에서 받아 처리
/// 연속 입력이 발생했을 때, InputManager에서 SequenceController에 이벤트를 발생시키도록 하거나
/// 특정 키(방향키 등)에 한해서만 이벤트를 호출하도록 구현해 볼 것
public class UnitGroundDashState : UnitGroundState
{
    private int dashDirection;   // 대시 방향

    public UnitGroundDashState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("UnitGroundDashState Enter");
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("UnitGroundDashState Exit");
    }

    public override void Update()
    {
        base.Update();
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
            // 앞대시: 대시 상태로 전이
            dashDirection = dir;
            stateMachine.ChangeUnitState(stateMachine.GroundDashState); // DashState에서 PendingDashDirection을 읽도록 설계
        }
        else
        {
            // 백대시: 별도 State 혹은 DashState에서 isBackDash 플래그로 처리
            dashDirection = dir;
            stateMachine.ChangeUnitState(stateMachine.GroundDashState);
        }
    }
}
