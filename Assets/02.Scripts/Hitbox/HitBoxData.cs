using UnityEngine;

/// <summary>
/// 히트박스 데이터를 저장하는 클래스
/// </summary>
[System.Serializable]
public class HitBoxData
{
    public string hitboxName;
    public Vector2 size;
    public Vector2 offset;
    public float damage;
    public Vector2 knockback;
}