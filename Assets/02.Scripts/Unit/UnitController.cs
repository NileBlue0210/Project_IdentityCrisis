using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 행동을 제어하는 메소드
/// </summary>
public class UnitController : MonoBehaviour
{
    #region Unit Information

    [SerializeField] private bool IsControllable { get; set; } // 유닛 조작 가능 여부
    [field: SerializeField] public float Attack { get; set; } // 유닛 공격력
    [field: SerializeField] public float Health { get; set; } // 유닛 체력
    [field: SerializeField] public float Defense { get; set; } // 유닛 방어력
    [field: SerializeField] public float Velocity { get; set; } // 유닛 속도
    [field: SerializeField] public float Gravity { get; set; } // 유닛 중력 ( 점프력 조정 스테이터스 )
    [field: SerializeField] public float FootPoint { get; set; }    // 유닛의 바닥 포인트를 설정하기 위한 발 위치 측정 변수
    [field: SerializeField] public float GroundPoint { get; set; } // 유닛과 지면사이의 거리
    [field: SerializeField] public float LeftWallPoint { get; set; } // 유닛의 왼쪽 벽 위치
    [field: SerializeField] public float RightWallPoint { get; set; } // 유닛의 오른쪽 벽 위치

    #endregion Unit Information

    #region Methods

    void Start()
    {

    }

    void Update()
    {

    }

    // to do : Start시에 플레이어의 컨트롤ID ( 1P인지 2P인지 ) 와 Unit의 컨트롤ID가 같을 경우, 조작 가능 플래그를 활성화
    
    #endregion Methods
}
