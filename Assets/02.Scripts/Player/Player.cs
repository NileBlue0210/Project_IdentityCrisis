using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾�� ĳ���Ͱ� ��Ӱ����� �÷��̾� �ñ״�ó Ŭ����
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Player Component")]
    private PlayerController playerController;
    private PlayerCondition playerCondition;

    #region Player Information

    [field: SerializeField] public string PlayerName { get; set; }
    [field: SerializeField] public int Level { get; set; }
    [field: SerializeField] public int Exp { get; set; }

    #endregion Player Information

    #region Methods

    protected virtual void Awake()
    {
        // PlayerController�� ���� ���, �߰�
        if (this.TryGetComponent<PlayerController>(out playerController) == false)
        {
            this.gameObject.AddComponent<PlayerController>();

            Debug.LogError("Add PlayerController Component to Player");
        }

        // PlayerCondition�� ���� ���, �߰�
        if (this.TryGetComponent<PlayerCondition>(out playerCondition) == false)
        {
            this.gameObject.AddComponent<PlayerCondition>();

            Debug.LogError("Add PlayerCondition Component to Player");
        }

        playerCondition = this.GetComponent<PlayerCondition>();

        Init(); // ������ �ʱ� �������ͽ� ����
    }

    /// <summary>
    /// �÷��̾��� ������ ����ϴ� �޼ҵ�
    /// to do : DB�� ���� �α����� ������ ������ �޾ƿ����� �غ���
    /// </summary>
    public void Init()
    {
        PlayerName = "Administrator";
        Level = 1;
        Exp = 0;
    }
    
    #endregion Methods
}
