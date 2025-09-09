using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 히트박스 및 허트박스를 편집하는 에디터 스크립트
/// </summary>
public class HitBoxEditor : EditorWindow
{
    [Header("HitBox Components")]
    private GameObject characterPrefab; // 히트박스를 세팅할 캐릭터 프리팹

    [Header("Frame Datas")]
    private HitBoxFrameData hitBoxFrameData;    // 히트박스 및 허트박스 데이터가 담긴 스크립터블 오브젝트
    private int currentFrameIndex = 0;  // 현재 편집 중인 프레임 인덱스

    [Header("Animation Preview Variables")]
    private AnimationClip animationClip;    // 프리뷰용 애니메이션 클립
    private float currentAnimationTime; // 현재 애니메이션 재생 시간
    private Animator animator; // 프리뷰용 캐릭터 프리팹의 애니메이터 컴포넌트

    [Header("Default HitBox Settings")]
    // 새 히트, 허트박스 추가를 위한 임시 변수
    private string newHitboxName = "NewHitBox";
    private Vector3 newHitboxSize = Vector3.one;
    private Vector3 newHitboxOffset = Vector3.zero;
    private string newHurtboxName = "NewHurtBox";
    private Vector3 newHurtboxSize = Vector3.one;
    private Vector3 newHurtboxOffset = Vector3.zero;

    // 윈도우를 열기 위한 메뉴 아이템 추가
    [MenuItem("Window/HitBox Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(HitBoxEditor), false, "HitBox Editor");    // 기존 열려있는 윈도우가 있다면 포커스, 없다면 윈도우를 새로 생성
    }

    /// <summary>
    /// 에디터 GUI 작성 메소드
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("HitBox and HurtBox Editor", EditorStyles.boldLabel);

        // 히트박스 및 허트박스 데이터와 캐릭터 프리팹 할당
        hitBoxFrameData = (HitBoxFrameData)EditorGUILayout.ObjectField("Animation Frame Data", hitBoxFrameData, typeof(HitBoxFrameData), false);
        characterPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab", characterPrefab, typeof(GameObject), true);
        animationClip = (AnimationClip)EditorGUILayout.ObjectField("Preview Animation Clip", animationClip, typeof(AnimationClip), false);

        // 각 필드 유효성 체크
        if (hitBoxFrameData == null)
        {
            EditorGUILayout.HelpBox("Please assign an Animation Frame Data ScriptableObject or create a new one.", MessageType.Warning);

            return;
        }

        if (characterPrefab == null || EditorUtility.IsPersistent(characterPrefab))
        {
            EditorGUILayout.HelpBox("Please assign a character prefab.", MessageType.Warning);

            return;
        }
        else
        {
            animator = characterPrefab.GetComponentInChildren<Animator>();

            if (animator == null || animator.runtimeAnimatorController == null)
            {
                EditorGUILayout.HelpBox("The assigned character prefab must have an Animator with a valid Controller.", MessageType.Warning);

                return;
            }
            else
            {
                // 에디터에서 애니메이션을 편집할 수 있는 모드 활성화
                AnimationMode.StartAnimationMode();
            }
        }

        if (animationClip == null)
        {
            EditorGUILayout.HelpBox("Please assign an Animation Clip for preview.", MessageType.Warning);

            return;
        }

        EditorGUI.BeginChangeCheck();   // 되돌리기 작업을 위한 에디터 내용 변경 감지

        GUILayout.Space(10);    // 가독성을 위한 공백 추가

        // 애니메이션 프리뷰 섹션
        GUILayout.Label("Animation Preview", EditorStyles.boldLabel);

        currentAnimationTime = EditorGUILayout.Slider(currentAnimationTime, 0f, animationClip.length);  // 애니메이션 재생 구간 슬라이더
        AnimationMode.SampleAnimationClip(animator.gameObject, animationClip, currentAnimationTime);    // 애니메이션 프리뷰 재생 ( GUI가 갱신될 때 마다 애니메이션도 갱신 )

        GUILayout.Space(10);

        if (hitBoxFrameData.frames.Count > 0)
        {
            currentFrameIndex = Mathf.Clamp(currentFrameIndex, 0, hitBoxFrameData.frames.Count - 1);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous Frame"))
            {
                currentFrameIndex = Mathf.Max(0, currentFrameIndex - 1);    // 음수가 되지 않도록 프레임 값 보정
            }

            GUILayout.Label($"Frame {currentFrameIndex} of {hitBoxFrameData.frames.Count - 1}");

            if (GUILayout.Button("Next Frame"))
            {
                currentFrameIndex = Mathf.Min(hitBoxFrameData.frames.Count - 1, currentFrameIndex + 1); // 총 프레임 수를 넘지 않도록 프레임 값 보정
            }

            currentFrameIndex = (int)EditorGUILayout.Slider("Go to Frame", currentFrameIndex, 0, hitBoxFrameData.frames.Count - 1); // 슬라이더를 통한 프레임 이동
            currentAnimationTime = (float)currentFrameIndex / hitBoxFrameData.frames.Count * animationClip.length; // 프레임에 맞춰 애니메이션 시간 보정

            GUILayout.EndHorizontal();

            FrameData currentFrame = hitBoxFrameData.frames[currentFrameIndex]; // SO 내부의 현재 프레임 정보 취득

            GUILayout.Space(10);

            // 히트박스 편집 섹션
            GUILayout.Label("HitBoxes", EditorStyles.boldLabel);

            GUILayout.Space(10);

            // 현재 프레임의 히트박스 데이터를 불러와 에디터에 표시
            for (int i = 0; i < currentFrame.hitboxes.Count; i++)
            {
                GUILayout.BeginHorizontal();

                HitBoxData hitbox = currentFrame.hitboxes[i];

                hitbox.hitboxName = EditorGUILayout.TextField("Name", hitbox.hitboxName);
                hitbox.size = EditorGUILayout.Vector3Field("Size", hitbox.size);
                hitbox.offset = EditorGUILayout.Vector3Field("Offset", hitbox.offset);

                // 히트박스 삭제 버튼
                if (GUILayout.Button("Remove"))
                {
                    currentFrame.hitboxes.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            // 새 히트박스 추가 섹션
            GUILayout.Label("New HitBox Settings", EditorStyles.boldLabel);

            // 히트박스 GUI에 디폴트 값 설정
            newHitboxName = EditorGUILayout.TextField("Name", newHitboxName);
            newHitboxSize = EditorGUILayout.Vector3Field("Size", newHitboxSize);
            newHitboxOffset = EditorGUILayout.Vector3Field("Offset", newHitboxOffset);

            // 새로운 히트박스 데이터 추가
            if (GUILayout.Button("Add HitBox"))
            {
                currentFrame.hitboxes.Add(new HitBoxData
                {
                    hitboxName = newHitboxName,
                    size = newHitboxSize,
                    offset = newHitboxOffset
                });
            }

            GUILayout.Space(10);

            // 허트박스 편집 섹션
            GUILayout.Label("HurtBoxes", EditorStyles.boldLabel);

            GUILayout.Space(10);

            // 허트박스 데이터를 불러와 에디터에 표시
            for (int i = 0; i < currentFrame.hurtboxes.Count; i++)
            {
                GUILayout.BeginHorizontal();

                HurtBoxData hurtbox = currentFrame.hurtboxes[i];

                hurtbox.hurtboxName = EditorGUILayout.TextField("Name", hurtbox.hurtboxName);
                hurtbox.size = EditorGUILayout.Vector3Field("Size", hurtbox.size);
                hurtbox.offset = EditorGUILayout.Vector3Field("Offset", hurtbox.offset);

                // 허트박스 삭제 버튼
                if (GUILayout.Button("Remove"))
                {
                    currentFrame.hurtboxes.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            // 새 허트박스 추가 섹션
            GUILayout.Label("New HurtBox Settings", EditorStyles.boldLabel);

            // 허트박스 GUI에 디폴트 값 설정
            newHurtboxName = EditorGUILayout.TextField("Name", newHurtboxName);
            newHurtboxSize = EditorGUILayout.Vector3Field("Size", newHurtboxSize);
            newHurtboxOffset = EditorGUILayout.Vector3Field("Offset", newHurtboxOffset);

            // 새로운 허트박스 데이터 추가
            if (GUILayout.Button("Add HurtBox"))
            {
                currentFrame.hurtboxes.Add(new HurtBoxData
                {
                    hurtboxName = newHurtboxName,
                    size = newHurtboxSize,
                    offset = newHurtboxOffset
                });
            }

            GUILayout.Space(10);
            
            // BeginChangeCheck를 기준으로 변경 내역을 감지
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(hitBoxFrameData, "Edit HitBox And HurtBox Frame Data");   // 변경 내역 기록 ( Ctr + Z 커맨드를 통해 작업 내용을 되돌릴 수 있다 )
                EditorUtility.SetDirty(hitBoxFrameData);    // Ctr + S를 눌러 저장하거나, 에디터를 닫을 때 변경 내역을 저장할 것이냐고 묻는 기능의 활성화
            }

            if (GUILayout.Button("Save Changes"))
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);
            
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }
    }

    private void OnDrawGizmos(GameObject characterPrefab)
    {
        if (hitBoxFrameData == null || currentFrameIndex >= hitBoxFrameData.frames.Count) return;

        var frame = hitBoxFrameData.frames[currentFrameIndex];

        // 히트박스를 빨간색 Gizmo로 표시
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        foreach (var hitbox in frame.hitboxes)
        {
            Gizmos.DrawCube(characterPrefab.transform.position + hitbox.offset, hitbox.size);
        }

        // 허트박스를 파란색 Gizmo로 표시
        Gizmos.color = new Color(0, 0, 1, 0.5f);

        foreach (var hurtbox in frame.hurtboxes)
        {
            Gizmos.DrawCube(characterPrefab.transform.position + hurtbox.offset, hurtbox.size);
        }
    }

    /// <summary>
    /// 에디터 윈도우가 닫힐 때 호출되는 메소드
    /// </summary>
    /// <returns></returns>
    private void OnDestroy()
    {
        AnimationMode.StopAnimationMode();
    }

    /// <summary>
    /// 에디터 윈도우가 활성화될 때 호출되는 메소드
    /// </summary>
    /// <returns></returns>
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    /// <summary>
    /// 에디터 윈도우가 비활성화될 때 호출되는 메소드
    /// </summary>
    /// <returns></returns>
    private void OnDisable()
    {
        AnimationMode.StopAnimationMode();
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (characterPrefab == null || hitBoxFrameData == null || hitBoxFrameData.frames.Count == 0)
            return;

        FrameData frame = hitBoxFrameData.frames[currentFrameIndex];

        Handles.color = new Color(1f, 0.2f, 0.2f, 0.9f);
        for (int i = 0; i < frame.hitboxes.Count; i++)
        {
            var hb = frame.hitboxes[i];

            // world center & world size (lossy scale 적용)
            Vector3 worldCenter = characterPrefab.transform.TransformPoint(hb.offset);
            Vector3 lossy = characterPrefab.transform.lossyScale;
            Vector3 worldSize = new Vector3(hb.size.x * Mathf.Abs(lossy.x),
                                            hb.size.y * Mathf.Abs(lossy.y),
                                            hb.size.z * Mathf.Abs(lossy.z));

            EditorGUI.BeginChangeCheck();

            // Position handle (world)
            Vector3 newWorldCenter = Handles.PositionHandle(worldCenter, Quaternion.identity);

            // Scale handle (we pass worldSize and map back)
            Vector3 newWorldSize = Handles.ScaleHandle(worldSize, worldCenter, Quaternion.identity, HandleUtility.GetHandleSize(worldCenter));

            // Draw wire cube for visualization
            Handles.DrawWireCube(newWorldCenter, newWorldSize);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(hitBoxFrameData, "Edit HitBox (Scene)");
                // Convert back to local offset + local size
                hb.offset = characterPrefab.transform.InverseTransformPoint(newWorldCenter);

                // avoid zero-scale division and preserve positive size (use absolute lossy)
                float sx = Mathf.Approximately(lossy.x, 0f) ? 1f : Mathf.Abs(lossy.x);
                float sy = Mathf.Approximately(lossy.y, 0f) ? 1f : Mathf.Abs(lossy.y);
                float sz = Mathf.Approximately(lossy.z, 0f) ? 1f : Mathf.Abs(lossy.z);

                hb.size = new Vector3(newWorldSize.x / sx, newWorldSize.y / sy, newWorldSize.z / sz);

                EditorUtility.SetDirty(hitBoxFrameData);
            }
        }

        Handles.color = new Color(0.2f, 0.4f, 1f, 0.9f);
        for (int i = 0; i < frame.hurtboxes.Count; i++)
        {
            var hb = frame.hurtboxes[i];

            Vector3 worldCenter = characterPrefab.transform.TransformPoint(hb.offset);
            Vector3 lossy = characterPrefab.transform.lossyScale;
            Vector3 worldSize = new Vector3(hb.size.x * Mathf.Abs(lossy.x),
                                            hb.size.y * Mathf.Abs(lossy.y),
                                            hb.size.z * Mathf.Abs(lossy.z));

            EditorGUI.BeginChangeCheck();

            Vector3 newWorldCenter = Handles.PositionHandle(worldCenter, Quaternion.identity);
            Vector3 newWorldSize = Handles.ScaleHandle(worldSize, worldCenter, Quaternion.identity, HandleUtility.GetHandleSize(worldCenter));

            Handles.DrawWireCube(newWorldCenter, newWorldSize);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(hitBoxFrameData, "Edit HurtBox (Scene)");
                hb.offset = characterPrefab.transform.InverseTransformPoint(newWorldCenter);

                float sx = Mathf.Approximately(lossy.x, 0f) ? 1f : Mathf.Abs(lossy.x);
                float sy = Mathf.Approximately(lossy.y, 0f) ? 1f : Mathf.Abs(lossy.y);
                float sz = Mathf.Approximately(lossy.z, 0f) ? 1f : Mathf.Abs(lossy.z);

                hb.size = new Vector3(newWorldSize.x / sx, newWorldSize.y / sy, newWorldSize.z / sz);

                EditorUtility.SetDirty(hitBoxFrameData);
            }
        }

        // 씬 뷰 강제 갱신 (핸들 작업 중 시각성 확보)
        sceneView.Repaint();
    }
}
