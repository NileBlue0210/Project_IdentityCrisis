/// <summary>
/// 유닛의 공격 하위 상태를 관리하는 추상 상태 클래스
/// </summary>
public abstract class UnitAttackSubState
{
    protected readonly UnitAttackState attackState;

    public bool IsFinished { get; protected set; } // 하위 상태 완료 플래그 ( 공격 후 후딜레이 상태, 피격으로 인한 캔슬 등 )

    public UnitAttackSubState(UnitAttackState attackState)
    {
        this.attackState = attackState;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
