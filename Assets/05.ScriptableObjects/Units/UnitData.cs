using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 유닛의 스크립터블 오브젝트
/// to do : 개발 편의를 위한 SO. 추후 Json으로 관리할 것
/// </summary>
[CreateAssetMenu(fileName = "Unit Data", menuName = "Scriptable Objects/Unit Data", order = int.MaxValue)]
public class UnitData : ScriptableObject
{
    public string UnitGuid;   // 중복 불가능한 유닛의 고유 GUID
    public string UnitName;
    public float Attack;
    public float Health;
    public float Defense;
    public float MoveSpeed;
    public EUnitDashType DashType;  // 달리기와 돌진 등의 대시 타입
    public float DashSpeed;
    public float DashDuration;
    public float AerialDashSpeed;
    public float AerialDashDuration;
    public float BackDashSpeed;
    public float BackDashDuration;
    public float AerialBackDashSpeed;
    public float AerialBackDashDuration;
    public float JumpForce;
    public float HorizontalJumpSpeed;   // 대각선 점프 속도 ( 점프 각도 )
    public int JumpCount;   // 점프 횟수
    public int AerialDashCount; // 공중 대시 횟수
    public float Gravity;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 유닛 SO에 id값이 없을 경우 guid값을 베이스로 유닛의 id값을 생성
        if (string.IsNullOrEmpty(UnitGuid))
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            UnitGuid = AssetDatabase.AssetPathToGUID(assetPath);
        }  
    }
#endif
}
