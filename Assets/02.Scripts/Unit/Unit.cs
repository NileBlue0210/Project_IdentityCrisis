using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 달리기 타입
/// Dash : 짧은 거리를 순간적으로 이동
/// Run : 키를 유지하는 동안 증가된 이동속도로 이동
/// </summary>
public enum UnitDashType
{
    Dash,
    Run
}

/// <summary>
/// 게임에 사용되는 유닛의 기본 클래스
/// </summary>
public class Unit : MonoBehaviour
{
    public UnitController UnitController { get; set; }
    public UnitCondition UnitCondition { get; set; }
    public UnitStateMachine UnitStateMachine { get; set; }
    public Animator UnitAnimator { get; set; }

    #region Unit Information

    [field: SerializeField] public UnitData UnitData { get; set; } // 유닛 데이터 to do : 개발 편의를 위한 SO 사용. 추후 리팩토링을 통해 Json을 사용하도록 변경
    [field: SerializeField] public string UnitName { get; set; } // 유닛 이름
    [field: SerializeField] public int PlayerableId { get; set; } // 플레이어블 유닛이 몇 번째 플레이어인지 to do : 게임 매니저 측에서 게임 시작 시, 각 유닛의 플레이어 Id를 부여하되, 부여 후 게임 상에 같은 Id가 없는지 체크하는 로직을 반드시 넣을 것
    [field: SerializeField] public float Attack { get; set; } // 유닛 공격력
    [field: SerializeField] public float Health { get; set; } // 유닛 체력
    [field: SerializeField] public float Defense { get; set; } // 유닛 방어력
    [field: SerializeField] public float MoveSpeed { get; set; } // 유닛 속도
    [field: SerializeField] public UnitDashType DashType { get; set; } // 유닛의 대시 타입
    [field: SerializeField] public float DashSpeed { get; set; } // 달리기 시 가속도, 또는 대시 속도
    [field: SerializeField] public float DashDuration { get; set; } // 대시 시 이동 거리
    [field: SerializeField] public float BackDashSpeed { get; set; } // 백대시 속도
    [field: SerializeField] public float BackDashDuration { get; set; } // 백대시 시 이동 거리
    [field: SerializeField] public float JumpForce { get; set; } // 유닛 점프력
    [field: SerializeField] public float HorizontalJumpSpeed { get; set; }  // 유닛 대각선 점프 속도 ( 점프 각도 )
    [field: SerializeField] public int JumpCount { get; set; } // 유닛 최대 점프 횟수
    [field: SerializeField] public int AerialDashCount { get; set; } // 유닛 최대 대시, 백대시 횟수 ( 대시, 백대시는 횟수를 공유 )
    [field: SerializeField] public float Gravity { get; set; } // 유닛 중력 ( 점프력 조정 스테이터스 )

    #endregion Unit Information

    #region Methods

    void Awake()
    {
        // 필요 컴포넌트가 없을 경우, 취득
        if (this.TryGetComponent<UnitController>(out UnitController UnitControllerComponent) == false)
        {
            this.gameObject.AddComponent<UnitController>();

            Debug.LogError("Add UnitController Component to Unit");
        }

        if (this.TryGetComponent<UnitCondition>(out UnitCondition UnitConditionComponent) == false)
        {
            this.gameObject.AddComponent<UnitCondition>();

            Debug.LogError("Add UnitCondition Component to Unit");
        }

        if (this.TryGetComponent<UnitStateMachine>(out UnitStateMachine UnitStateMachineComponent) == false)
        {
            this.gameObject.AddComponent<UnitStateMachine>();

            Debug.LogError("Add UnitStateMachine Component to Unit");
        }

        if (this.GetComponentInChildren<Animator>() == null)
        {
            Debug.LogError("Unit Animator is Null");
        }

        UnitController = this.GetComponent<UnitController>();
        UnitCondition = this.GetComponent<UnitCondition>();
        UnitStateMachine = this.GetComponent<UnitStateMachine>();
        UnitAnimator = GetComponentInChildren<Animator>();  // 유닛 하위에 있는 모델 Animator 컴포넌트 취득

        Init(); // 유닛 정보 초기화
    }

    void Start()
    {

    }

    void Update()
    {

    }

    // 유닛 정보 초기화
    public void Init()
    {
        // 유닛 정보 지정
        UnitName = UnitData.UnitName;

        // 유닛 스테이터스 지정
        Attack = UnitData.Attack;
        Defense = UnitData.Defense;
        Health = UnitData.Health;
        MoveSpeed = UnitData.MoveSpeed;
        DashType = UnitData.DashType;
        DashSpeed = UnitData.DashSpeed;
        DashDuration = UnitData.DashDuration;
        BackDashSpeed = UnitData.BackDashSpeed;
        BackDashDuration = UnitData.BackDashDuration;
        JumpForce = UnitData.JumpForce;
        HorizontalJumpSpeed = UnitData.HorizontalJumpSpeed;
        JumpCount = UnitData.JumpCount;
        AerialDashCount = UnitData.AerialDashCount;
        Gravity = UnitData.Gravity;
    }

    #endregion Methods
}