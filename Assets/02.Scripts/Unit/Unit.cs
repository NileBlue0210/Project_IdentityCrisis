using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ӿ� ���Ǵ� ������ �⺻ Ŭ����
/// </summary>
public class Unit : MonoBehaviour
{
    [Header("Unit Conponents")]
    private UnitController unitController;
    private UnitCondition unitCondition;

    #region Unit Information

    [field: SerializeField] public UnitData UnitData { get; set; } // ���� ������ to do : ���� ���Ǹ� ���� SO ���. ���� �����丵�� ���� Json�� ����ϵ��� ����
    [field: SerializeField] public string UnitName { get; set; } // ���� ���ݷ�
    [field: SerializeField] public int PlayerableId { get; set; } // �÷��̾�� ������ �� ��° �÷��̾����� to do : ���� �Ŵ��� ������ ���� ���� ��, �� ������ �÷��̾� Id�� �ο��ϵ�, �ο� �� ���� �� ���� Id�� ������ üũ�ϴ� ������ �ݵ�� ���� ��

    #endregion Unit Information

    #region Methods

    void Awake()
    {
        // �ʿ� ������Ʈ�� ���� ���, ���
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

        Init(); // ���� ���� �ʱ�ȭ
    }

    void Start()
    {

    }

    void Update()
    {

    }

    // ���� ���� �ʱ�ȭ
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