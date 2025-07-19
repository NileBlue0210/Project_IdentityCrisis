using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어블 캐릭터가 상속가능한 플레이어 시그니처 클래스
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
        // PlayerController가 없을 경우, 추가
        if (this.TryGetComponent<PlayerController>(out playerController) == false)
        {
            this.gameObject.AddComponent<PlayerController>();

            Debug.LogError("Add PlayerController Component to Player");
        }

        // PlayerCondition이 없을 경우, 추가
        if (this.TryGetComponent<PlayerCondition>(out playerCondition) == false)
        {
            this.gameObject.AddComponent<PlayerCondition>();

            Debug.LogError("Add PlayerCondition Component to Player");
        }

        playerCondition = this.GetComponent<PlayerCondition>();

        Init(); // 유닛의 초기 스테이터스 설정
    }

    /// <summary>
    /// 플레이어의 정보를 취득하는 메소드
    /// to do : DB를 통해 로그인한 유저의 정보를 받아오도록 해보자
    /// </summary>
    public void Init()
    {
        PlayerName = "Administrator";
        Level = 1;
        Exp = 0;
    }
    
    #endregion Methods
}
