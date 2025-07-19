using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전투를 제어하는 메소드
/// </summary>
public class BattleManager : MonoBehaviour
{
    [field:SerializeField] public Dictionary<int, Unit> Units { get; set; } // 전투에 참여하는 유닛들. 각 플레이어가 유닛을 선택하면 해당 유닛을 플레이어블ID ( 1p, 2p ... ) 와 함께 추가


    void Start()
    {

    }

    void Update()
    {

    }
}
