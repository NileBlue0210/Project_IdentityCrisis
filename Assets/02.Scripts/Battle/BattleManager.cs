using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전투를 제어하는 메소드
/// </summary>
public class BattleManager : MonoBehaviour
{
    [field: SerializeField] public Dictionary<int, Unit> Units { get; set; } // 전투에 참여하는 유닛들. 각 플레이어가 유닛을 선택하면 해당 유닛을 플레이어블ID ( 1p, 2p ... ) 와 함께 추가
    [field: SerializeField] public Dictionary<EUnits, List<HitBoxFrameData>> UnitHitBoxDataDictionary { get; set; } // 유닛 타입을 받아 해당 유닛의 히트박스 데이터를 반환하는 딕셔너리 컬렉션 ( 유닛 히트박스 데이터는 Addressable으로 일괄 로드 )

    private void Awake()
    {
        
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void SetBattleUnits(int side, Unit unit)
    {
        Units.Add(side, unit);
    }
}
