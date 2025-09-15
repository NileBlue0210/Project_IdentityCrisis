using UnityEngine;

/// <summary>
/// 각 유닛을 구별하기 위한 열거형
/// </summary>
public enum EUnits
{
    None = 0,
    LowPoly = 1 << 0,
}

/// <summary>
/// addressable group에 등록된 에셋 주소 ( 사용 시, addressable group내의 이름을 복사, 붙여넣기 하여 사용 )
/// </summary>
public enum EAddressableKeys
{
    None,
    LowPolyHitBoxs
}

public class Util
{
    [Header("HitBox Properties")]
    public Color HitBoxColor = new Color(1, 0, 0, 0.5f);
    public Color HurtBoxColor = new Color(0, 1, 0, 0.5f);
}