using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에 사용되는 유닛의 기본 클래스
/// </summary>
public class Unit : MonoBehaviour
{
    [Header("Unit Conponents")]
    private UnitController unitController;
    private UnitCondition unitCondition;

    #region Unit Information

    [field: SerializeField] public UnitData UnitData { get; set; } // 유닛 데이터 to do : 개발 편의를 위한 SO 사용. 추후 리팩토링을 통해 Json을 사용하도록 변경
    [field: SerializeField] public string UnitName { get; set; } // 유닛 공격력
    [field: SerializeField] public int PlayerableId { get; set; } // 플레이어블 유닛이 몇 번째 플레이어인지 to do : 게임 매니저 측에서 게임 시작 시, 각 유닛의 플레이어 Id를 부여하되, 부여 후 게임 상에 같은 Id가 없는지 체크하는 로직을 반드시 넣을 것

    #endregion Unit Information

    #region Methods

    void Awake()
    {
        // 필요 컴포넌트가 없을 경우, 취득
        if (this.TryGetComponent<UnitController>(out unitController) == false)
        {
            this.gameObject.AddComponent<UnitController>();

            Debug.LogError("Add UnitController Component to Unit");
        }

        if (this.TryGetComponent<UnitCondition>(out unitCondition) == false)
        {
            this.gameObject.AddComponent<UnitCondition>();

            Debug.LogError("Add UnitCondition Component to Unit");
        }

        unitController = this.GetComponent<UnitController>();
        unitCondition = this.GetComponent<UnitCondition>();

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
        UnitName = UnitData.UnitName;

        unitController.Attack = UnitData.Attack;
        unitController.Defense = UnitData.Defense;
        unitController.Health = UnitData.Health;
        unitController.Velocity = UnitData.Velocity;
        unitController.Gravity = UnitData.Gravity;
        unitController.FootPoint = UnitData.FootPoint;
        unitController.GroundPoint = UnitData.GroundPoint;
        unitController.LeftWallPoint = UnitData.LeftWallPoint;
        unitController.RightWallPoint = UnitData.RightWallPoint;

        unitCondition.State = UnitState.Idle;
    }
    
    #endregion Methods
}