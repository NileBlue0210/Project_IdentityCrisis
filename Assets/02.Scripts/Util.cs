/// <summary>
/// ���ӿ��� �����Ǵ� ��ƿ��Ƽ Ŭ����
/// </summary>

// ������ �⺻ ����
public enum UnitState
{
    None = -1, // ���°� ���ǵ��� ���� ���
    Idle,
    Move,
    Jump,
    Attack,
    Hit,
    Guard,
    Sit,
    Dead
}

// ������ �ǰ� ����
public enum HitStatus
{
    None = -1,  // �ǰ� ���°� ���ǵ��� ���� ���
    Normal,     // ������ �ɸ��� �ʴ� �ǰ� ����
    Stagger,    // ��Ʈ ���� ����
    Freeze,     // ���� ����
    Stun,       // ���� ����
    WallSplat,  // ���� ó���� ����
    Down,       // ������ ����
}

// ������ ����� ����
public enum DebuffStatus
{

}

public class Util
{

}
