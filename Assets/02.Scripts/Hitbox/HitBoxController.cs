using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

/// <summary>
/// 유닛의 히트박스를 관리하는 컨트롤러 클래스
/// to do : 
/// 히트박스와 허트박스끼리 부딪혔을 때의 충돌 처리 구현
/// </summary>
public class HitBoxController : MonoBehaviour
{
    [Header("HitBox Components")]
    private HitBoxFrameData frameData;  // 히트박스 데이터
    private FrameData currentFrame;  // 현재 프레임 데이터
    public List<HitBoxFrameData> hitBoxDatas;  // 모든 히트박스 데이터
    private static List<HitBoxController> allHitBoxControllers = new List<HitBoxController>();   // 씬 내 모든 히트박스 컨트롤러 리스트
    private void OnEnable() => allHitBoxControllers.Add(this);
    private void OnDisable() => allHitBoxControllers.Remove(this);

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
    /// <param name="index"></param>
    public void SetCurrentFrameData(int index)
    {
        if (frameData == null)
        {
            Debug.LogError("HitBoxFrameData is null");

            return;
        }

        if (index < 0 || index >= frameData.frames.Count)
        {
            // 프레임 번호가 유효하지 않을 경우, 경고를 출력하고 마지막 프레임 데이터로 설정하거나, 처리를 중단할 수 있습니다.
            Debug.LogWarning($"Invalid frame number: {index}. It must be between 0 and {frameData.frames.Count - 1}.");

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
    /// 히트박스와 허트박스의 충돌 여부를 판정하는 메소드
    /// </summary>
    /// <param name="hitBoxPos"></param>
    /// <param name="hitBoxSize"></param>
    /// <param name="hurtBoxPos"></param>
    /// <param name="hurtBoxSize"></param>
    /// <returns></returns>
    private bool HitBoxOverlap(Vector2 hitBoxPos, Vector2 hitBoxSize, Vector2 hurtBoxPos, Vector2 hurtBoxSize)
    {
        bool overlapX = Mathf.Abs(hitBoxPos.x - hurtBoxPos.x) < (hitBoxSize.x + hurtBoxSize.x) / 2;
        bool overlapY = Mathf.Abs(hitBoxPos.y - hurtBoxPos.y) < (hitBoxSize.y + hurtBoxSize.y) / 2;
        bool result = false;

        if (overlapX && overlapY)
            result = true;

        return result;
    }

    public void DetectAndProcessCollision()
    {
        if (currentFrame == null || currentFrame.hitboxes.Count == 0)
            return;

        foreach (HitBoxController target in allHitBoxControllers)
        {
            if (target == this)
                continue;

            if (Vector2.Distance(transform.position, target.transform.position) > 5f)
                continue;   // 일정 거리 이상 떨어진 대상은 충돌 체크에서 제외 (성능 최적화)

            CheckBoxCollision(target);
        }
    }

    /// <summary>
    /// 다른 유닛의 히트박스 컨트롤러와 충돌을 교차 체크하는 메소드
    /// </summary>
    /// <param name="target"></param>
    public void CheckBoxCollision(HitBoxController target)
    {
        if (currentFrame == null || target.currentFrame == null)
            return;

        foreach (HitBoxData hitBox in currentFrame.hitboxes)
        {
            Vector2 hitBoxCenter = (Vector2)transform.position + hitBox.offset;

            foreach (HurtBoxData hurtBox in target.currentFrame.hurtboxes)
            {
                Vector2 hurtBoxCenter = (Vector2)target.transform.position + hurtBox.offset;

                if (HitBoxOverlap(hitBoxCenter, hitBox.size, hurtBoxCenter, hurtBox.size))
                {
                    OnHitDetected(target, hitBox);
                }
            }
        }
    }
    
    /// <summary>
    /// 타격 판정이 발생했을 때 호출되는 메서드
    /// </summary>
    private void OnHitDetected(HitBoxController target, HitBoxData hitBox)
    {
        Debug.Log($"{name} hit {target.name}! Damage: {hitBox.damage}");
        target.OnHurtReceived(hitBox);

        // 여기서 대상 유닛의 피격 처리 호출 가능
        // target.OnHurtReceived(hitBox);
    }

    /// <summary>
    /// 피격 처리 (히트박스에 맞았을 때)
    /// </summary>
    public void OnHurtReceived(HitBoxData hitBox)
    {
        Debug.Log($"{name} took {hitBox.damage} damage!");
        // ex) HP 감소, 넉백 처리 등 추가 예정
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

        // Debug.Log("DrawGizmos");
    }
}
