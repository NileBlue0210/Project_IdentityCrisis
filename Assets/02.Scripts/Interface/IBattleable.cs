/// <summary>
/// 전투 가능한 유닛을 정의하는 인터페이스
/// </summary>
/// 
/// memo : 유닛의 필수 스테이터스 또한 인터페이스에 정의해두는 편이 좋을까?
public interface IBattleable
{
    /// <summary>
    /// 유닛의 초기 스테이터스를 설정하는 메서드
    /// </summary>
    void Init();
}
