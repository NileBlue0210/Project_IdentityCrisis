using UnityEngine;

/// <summary>
/// 각 유닛을 구별하기 위한 열거형
/// </summary>
public enum EUnits
{
    None = 0,
    LowPoly = 1 << 0,
}

public class Util
{
    [Header("HitBox Properties")]
    public Color HitBoxColor = new Color(1, 0, 0, 0.5f);
    public Color HurtBoxColor = new Color(0, 1, 0, 0.5f);
}