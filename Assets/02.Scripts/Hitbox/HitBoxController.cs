using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

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
    public List<HitBoxFrameData> hitBoxDatas;  // 모든 히트박스 데이터

    [Header("HitBox Settings")]
    private Color hitBoxColor = new Color(1, 0, 0, 0.5f);
    private Color hurtBoxColor = new Color(0, 1, 0, 0.5f);

    /// <summary>
    /// 현재 재생중인 애니메이션 클립을 기반으로 히트박스 데이터를 설정하는 메소드
    /// </summary>
    /// <param name="clip"></param>
    public void SetCurrentHitBoxData(AnimationClip clip)
    {
        foreach (HitBoxFrameData hitBoxData in hitBoxDatas)
        {
            if (hitBoxData.targetAnimationClip == clip)
            {
                frameData = hitBoxData;

                break;
            }
        }

        if (frameData == null)
        {
            Debug.LogError("Invalid animation clip for hitbox data");

            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentFrame"></param>
    public void SetCurrentFrameData(int index)
    {
        if (frameData == null)
        {
            Debug.LogError("HitBoxFrameData is null");

            return;
        }

        if (index < 0 || index >= frameData.frames.Count)
        {
            Debug.LogError($"Invalid frame number: {currentFrame}");

            return;
        }

        currentFrame = frameData.frames[index];

        if (currentFrame == null)
        {
            Debug.LogError("Invalid frame data");

            return;
        }
    }

    /// <summary>
    /// 유닛의 히트박스 SO 폴더 주소를 기준으로 모든 히트박스 SO 데이터를 로드하는 메소드
    /// </summary>
    /// <param name="dataPath"></param>
    public async Task LoadAllHitBoxData(EUnits unitType)
    {
        string characterLabel = string.Empty;
        string assetTypeLabel = string.Empty;

        switch (unitType)
        {
            case EUnits.LowPoly:
                characterLabel = AddressableLabels.CHARACTER_LOWPOLY;
                assetTypeLabel = AddressableLabels.ASSET_HITBOX;

                break;
            default:
                Debug.LogError($"There is no hitbox data path for unit type '{unitType}'");

                return;
        }

        if (string.IsNullOrEmpty(characterLabel) || string.IsNullOrEmpty(assetTypeLabel))
        {
            Debug.LogError($"Invalid character label or asset type label for unit type '{unitType}'");

            return;
        }

        // 라벨을 통해 모든 HitBoxFrameData SO를 로드
        List<string> labelsToLoad = new List<string>
        {
            characterLabel,
            assetTypeLabel
        };
        
        // 해당되는 라벨을 모두 가진 에셋만 로드
        AsyncOperationHandle<IList<HitBoxFrameData>> loadHandle = 
            Addressables.LoadAssetsAsync<HitBoxFrameData>(
                labelsToLoad,
                null,
                Addressables.MergeMode.Intersection // 라벨 교집합 로드 옵션
            );

        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            string keyLog = string.Join(" & ", labelsToLoad);   // 로드한 라벨 정보 로그용 변수
            hitBoxDatas = new List<HitBoxFrameData>(loadHandle.Result);

            Debug.Log($"successfully loaded {hitBoxDatas.Count} hitbox data with labels: '{keyLog}'");
        }
        else
        {
            string keyLog = string.Join(" & ", labelsToLoad);   // 로드한 라벨 정보 로그용 변수

            Debug.LogError($"faild to load hitbox data with labels: '{keyLog}'. Status: {loadHandle.Status}");
        }
    }

    /// <summary>
    /// 유닛의 매 행동별 판정을 씬에서 확인하기 위한 기즈모 생성 메소드
    /// </summary>
    private void OnDrawGizmos()
    {
        if (frameData == null || currentFrame == null)
        {
            Debug.Log("frameData or currentFrame is null");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.Log("GameManager is null");
            return;
        }

        Gizmos.matrix = transform.localToWorldMatrix;   // 월드 좌표 기준으로 Gizmo를 표시

        Gizmos.color = hitBoxColor;

        // 히트박스 Gizmo 생성
        foreach (HitBoxData hitBoxData in currentFrame.hitboxes)
        {
            Gizmos.DrawWireCube(hitBoxData.offset, hitBoxData.size);
        }

        Gizmos.color = hurtBoxColor;

        // 허트박스 Gizmo 생성
        foreach (HurtBoxData hurtBoxData in currentFrame.hurtboxes)
        {
            Gizmos.DrawWireCube(hurtBoxData.offset, hurtBoxData.size);
        }

        Debug.Log("DrawGizmos");
    }
}
