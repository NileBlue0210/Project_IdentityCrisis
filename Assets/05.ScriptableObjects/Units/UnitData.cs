using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 스크립터블 오브젝트
/// to do : 개발 편의를 위한 SO. 추후 Json으로 관리할 것
/// </summary>
[CreateAssetMenu(fileName = "Unit Data", menuName = "Scriptable Objects/Unit Data", order = int.MaxValue)]
public class UnitData : ScriptableObject
{
    public string UnitName;
    public float Attack;
    public float Health;
    public float Defense;
    public float Velocity;
    public float Gravity;
    public float FootPoint;
    public float GroundPoint;
    public float LeftWallPoint;
    public float RightWallPoint;
}
