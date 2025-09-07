using UnityEngine;

/// <summary>
/// 히트박스 데이터를 저장하는 클래스
/// </summary>
[System.Serializable]
public class HitBoxData
{
    public string hitboxName;
    public Vector3 size;
    public Vector3 offset;
    public Vector3 knockback;
}