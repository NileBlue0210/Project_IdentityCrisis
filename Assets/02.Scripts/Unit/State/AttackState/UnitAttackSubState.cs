using System.Collections.Generic;

/// <summary>
/// 유닛의 공격 하위 상태를 관리하는 추상 상태 클래스
/// </summary>
public abstract class UnitAttackSubState
{
    protected readonly UnitAttackState attackState;

    public bool IsFinished { get; protected set; } // 하위 상태 완료 플래그 ( 공격 후 후딜레이 상태, 피격으로 인한 캔슬 등 )
    public abstract List<System.Type> ShiftableFromStatesList { get; set; } // 이 하위 공격 상태로 전환 가능한 상위 상태 컬렉션

    public UnitAttackSubState(UnitAttackState attackState)
    {
        this.attackState = attackState;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    // 현재 상태에서 하위 공격 상태로 전환 가능한지 여부 확인
    public abstract bool ShiftableFromStates(IUnitState currentState);
}
