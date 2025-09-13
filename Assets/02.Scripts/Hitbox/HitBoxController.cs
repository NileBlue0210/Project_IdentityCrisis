using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 히트박스를 관리하는 컨트롤러 클래스
/// to do : 유닛의 히트박스 정보를 애니메이션에 맞게 가져오려면 어떻게 하는게 좋을까?
/// 1. HitBoxFrameData SO에 애니메이션 클립을 할당하는 필드를 만들고, 유닛이 현재 재생시키고 있는 애니메이션과 동일한 클립의 히트박스 정보를 취득
/// 2. 애니메이션 재생 비율이나, 프레임 정보를 받아와 해당하는 히트, 허트박스를 유닛에 적용
/// 3. 히트, 허트박스를 표시할 때 Gizmo를 통해 씬에서 판정을 확인할 수 있도록 구현
/// 4. 히트박스와 허트박스끼리 부딪혔을 때의 충돌 처리 구현
/// </summary>
public class HitBoxController : MonoBehaviour
{
    [Header("HitBox Components")]
    private HitBoxFrameData frameData;  // 히트박스 데이터
    private FrameData currentFrame;  // 현재 프레임 데이터

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void SetCurrentHitBoxData(HitBoxFrameData data)
    {
        frameData = data;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="frame"></param>
    public void SetCurrentFrame(FrameData frame)
    {
        currentFrame = frame;
    }

    /// <summary>
    /// 유닛의 매 행동별 판정을 씬에서 확인하기 위한 기즈모 생성 메소드
    /// </summary>
    private void OnDrawGizmos()
    {
        if (frameData == null || currentFrame == null)
            return;

        Gizmos.matrix = transform.localToWorldMatrix;   // 월드 좌표 기준으로 Gizmo를 표시

        Gizmos.color = GameManager.Instance.Util.HitBoxColor;

        // 히트박스 Gizmo 생성
        foreach (HitBoxData hitBoxData in currentFrame.hitboxes)
        {
            Gizmos.DrawWireCube(hitBoxData.offset, hitBoxData.size);
        }

        Gizmos.color = GameManager.Instance.Util.HurtBoxColor;

        // 허트박스 Gizmo 생성
        foreach (HurtBoxData hurtBoxData in currentFrame.hurtboxes)
        {
            Gizmos.DrawWireCube(hurtBoxData.offset, hurtBoxData.size);
        }
    }
}
