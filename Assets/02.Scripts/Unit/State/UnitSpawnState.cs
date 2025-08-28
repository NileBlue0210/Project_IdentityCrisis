using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnState : UnitState
{
    public UnitSpawnState(UnitStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        GameManager.Instance.GetManager<InputManager>(typeof(InputManager)).OnPlayerUnitActionDisable();    // 스폰 초기에 플레이어 유닛의 입력을 비활성화
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.Unit.UnitController.IsGrounded())
        {
            stateMachine.ChangeUnitState(stateMachine.GroundState);

            GameManager.Instance.GetManager<InputManager>(typeof(InputManager)).OnPlayerUnitActionEnable(); // 스폰된 유닛이 지면에 닿은 후, 입력을 활성화 to do : 실제 상황에서는 BattleReady 페이즈에서도 입력은 불가능하게 할 예정
        }
    }
}
