/// <summary>
/// 게임에서 참조되는 유틸리티 클래스
/// </summary>

// 유닛의 기본 상태
public enum UnitState
{
    None = -1, // 상태가 정의되지 않은 경우
    Idle,
    Move,
    Jump,
    Attack,
    Hit,
    Guard,
    Sit,
    Dead
}

// 유닛의 피격 상태
public enum HitStatus
{
    None = -1,  // 피격 상태가 정의되지 않은 경우
    Normal,     // 경직이 걸리지 않는 피격 상태
    Stagger,    // 히트 경직 상태
    Freeze,     // 빙결 상태
    Stun,       // 기절 상태
    WallSplat,  // 벽에 처박힌 상태
    Down,       // 쓰러진 상태
}

// 유닛의 디버프 상태
public enum DebuffStatus
{

}

public class Util
{

}
