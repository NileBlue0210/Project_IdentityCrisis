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
    private bool previewPlaying = false; // 프리뷰 애니메이션 재생 여부
    private bool animationModeActive = false; // 애니메이션 모드 활성화 여부
    private Animator animator; // 프리뷰용 캐릭터 프리팹의 애니메이터 컴포넌트

    [Header("Default HitBox Settings")]
    // 새 히트박스 추가를 위한 임시 변수
    private string newHitboxName = "NewHitBox";
    private Vector3 newHitboxSize = Vector3.one;
    private Vector3 newHitboxOffset = Vector3.zero;

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

            if (GUILayout.Button("Create New HitBoxFrameData Asset"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Create HitBox Frame Data", "NewHitBoxFrameData", "asset", "Save HitBoxFrameData asset");

                if (!string.IsNullOrEmpty(path))
                {
                    var asset = ScriptableObject.CreateInstance<HitBoxFrameData>();

                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();

                    hitBoxFrameData = asset;
                }
            }

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
        }

        if (animationClip == null)
        {
            EditorGUILayout.HelpBox("Please assign an Animation Clip for preview.", MessageType.Warning);

            return;
        }

        GUILayout.Space(10);    // 가독성을 위한 공백 추가

        if (!previewPlaying)
        {
            if (GUILayout.Button("Start Preview"))
            {
                if (characterPrefab != null)
                {
                    AnimationMode.StartAnimationMode();

                    animationModeActive = true;
                    previewPlaying = true;
                }
                else EditorUtility.DisplayDialog("No Character", "Assign scene character instance first.", "OK");
            }
        }
        else
        {
            if (GUILayout.Button("Stop Preview"))
            {
                StopPreview();

                previewPlaying = false;
            }
        }

        // 시간 슬라이더
        currentAnimationTime = EditorGUILayout.Slider(currentAnimationTime, 0f, animationClip.length);

        if (previewPlaying && animationModeActive && characterPrefab != null)
        {
            // 샘플링 (매 GUI 갱신 시)
            AnimationMode.SampleAnimationClip(animator.gameObject, animationClip, currentAnimationTime);
        }

        GUILayout.Space(10);

        // 프레임 추가 및 네비게이션 버튼
        if (GUILayout.Button("Add Frame"))
        {
            hitBoxFrameData.frames.Add(new FrameData { frameNumber = hitBoxFrameData.frames.Count });
            currentFrameIndex = hitBoxFrameData.frames.Count - 1;
        }

        if (hitBoxFrameData.frames.Count > 0)
        {
            currentFrameIndex = Mathf.Clamp(currentFrameIndex, 0, hitBoxFrameData.frames.Count - 1);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous Frame"))
            {
                currentFrameIndex = Mathf.Max(0, currentFrameIndex - 1);    // 음수가 되지 않도록 프레임 값 보정
            }

            GUILayout.Label($"Frame {currentFrameIndex + 1} of {hitBoxFrameData.frames.Count}");

            if (GUILayout.Button("Next Frame"))
            {
                currentFrameIndex = Mathf.Min(hitBoxFrameData.frames.Count - 1, currentFrameIndex + 1); // 총 프레임 수를 넘지 않도록 프레임 값 보정
            }

            currentFrameIndex = (int)EditorGUILayout.Slider("Go to Frame", currentFrameIndex, 1, hitBoxFrameData.frames.Count - 1); // 슬라이더를 통한 프레임 이동

            GUILayout.EndHorizontal();

            FrameData currentFrame = hitBoxFrameData.frames[currentFrameIndex];

            GUILayout.Space(10);

            GUILayout.Label($"Editing Frame {currentFrame.frameNumber}", EditorStyles.boldLabel);

            GUILayout.Space(10);

            // HitBox 편집
            GUILayout.Label("HitBoxes", EditorStyles.boldLabel);

            for (int i = 0; i < currentFrame.hitboxes.Count; i++)
            {
                HitBoxData hitbox = currentFrame.hitboxes[i];
                GUILayout.BeginHorizontal();

                hitbox.hitboxName = EditorGUILayout.TextField("Name", hitbox.hitboxName);
                hitbox.size = EditorGUILayout.Vector3Field("Size", hitbox.size);
                hitbox.offset = EditorGUILayout.Vector3Field("Offset", hitbox.offset);
                hitbox.knockback = EditorGUILayout.Vector3Field("Knockback", hitbox.knockback);

                if (GUILayout.Button("Remove"))
                {
                    currentFrame.hitboxes.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Label("New HitBox Settings", EditorStyles.boldLabel);

            newHitboxName = EditorGUILayout.TextField("Name", newHitboxName);
            newHitboxSize = EditorGUILayout.Vector3Field("Size", newHitboxSize);
            newHitboxOffset = EditorGUILayout.Vector3Field("Offset", newHitboxOffset);

            if (GUILayout.Button("Add HitBox"))
            {
                currentFrame.hitboxes.Add(new HitBoxData {
                    hitboxName = newHitboxName,
                    size = newHitboxSize,
                    offset = newHitboxOffset,
                    knockback = Vector3.up
                });
            }

            GUILayout.Space(10);

            // HurtBox 편집
            GUILayout.Label("HurtBoxes", EditorStyles.boldLabel);

            for (int i = 0; i < currentFrame.hurtboxes.Count; i++)
            {
                HurtBoxData hurtbox = currentFrame.hurtboxes[i];
                GUILayout.BeginHorizontal();

                hurtbox.hurtboxName = EditorGUILayout.TextField("Name", hurtbox.hurtboxName);
                hurtbox.size = EditorGUILayout.Vector3Field("Size", hurtbox.size);
                hurtbox.offset = EditorGUILayout.Vector3Field("Offset", hurtbox.offset);

                if (GUILayout.Button("Remove"))
                {
                    currentFrame.hurtboxes.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add HurtBox"))
            {
                // currentFrame.hurtboxes.Add(new HurtBoxData("NewHurtBox", Vector3.one, Vector3.zero));
                currentFrame.hurtboxes.Add(new HurtBoxData());
            }

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(hitBoxFrameData, "Edit HitBox Frame Data");
                EditorUtility.SetDirty(hitBoxFrameData);
            }

            if (GUILayout.Button("Save Changes"))
            {
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(hitBoxFrameData);
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);
            
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }
    }

    /// <summary>
    /// 캐릭터 프리팹에 히트박스 및 허트박스 시각화
    /// </summary>
    /// <param name="characterPrefab">캐릭터 프리팹</param>
    /// <param name="frameData">현재 프레임 데이터</param>
    /// <returns></returns>
    private void VisualizeHitAndHurtBoxes(GameObject characterPrefab, FrameData frameData)
    {
        ClearVisualization(characterPrefab);

        foreach (var hitbox in frameData.hitboxes)
        {
            GameObject hitboxObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hitboxObj.name = $"HitBox_{hitbox.hitboxName}";
            hitboxObj.transform.SetParent(characterPrefab.transform);
            hitboxObj.transform.localPosition = hitbox.offset;
            hitboxObj.transform.localScale = hitbox.size;
            var renderer = hitboxObj.GetComponent<Renderer>();
            renderer.material.color = new Color(1, 0, 0, 0.5f); // 반투명 빨간색
            DestroyImmediate(hitboxObj.GetComponent<Collider>()); // 콜라이더 제거
        }

        foreach (var hurtbox in frameData.hurtboxes)
        {
            GameObject hurtboxObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hurtboxObj.name = $"HurtBox_{hurtbox.hurtboxName}";
            hurtboxObj.transform.SetParent(characterPrefab.transform);
            hurtboxObj.transform.localPosition = hurtbox.offset;
            hurtboxObj.transform.localScale = hurtbox.size;
            var renderer = hurtboxObj.GetComponent<Renderer>();
            renderer.material.color = new Color(0, 0, 1, 0.5f); // 반투명 파란색
            DestroyImmediate(hurtboxObj.GetComponent<Collider>()); // 콜라이더 제거
        }
    }

    /// <summary>
    /// 캐릭터 프리팹에서 히트박스 및 허트박스 시각화 제거
    /// </summary>
    /// <param name="characterPrefab">캐릭터 프리팹</param>
    private void ClearVisualization(GameObject characterPrefab)
    {
        var children = new List<GameObject>();
        foreach (Transform child in characterPrefab.transform)
        {
            if (child.name.StartsWith("HitBox_") || child.name.StartsWith("HurtBox_"))
            {
                children.Add(child.gameObject);
            }
        }

        foreach (var child in children)
        {
            DestroyImmediate(child);
        }
    }

    void OnDrawGizmos(GameObject characterPrefab)
    {
        if (hitBoxFrameData == null || currentFrameIndex >= hitBoxFrameData.frames.Count) return;

        var frame = hitBoxFrameData.frames[currentFrameIndex];
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        foreach (var hitbox in frame.hitboxes)
        {
            Gizmos.DrawCube(characterPrefab.transform.position + hitbox.offset, hitbox.size);
        }

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
        StopPreview();
    }

    private void StopPreview()
    {
        if (animationModeActive)
        {
            AnimationMode.StopAnimationMode();

            animationModeActive = false;
        }
    }

    /// <summary>
    /// 에디터 윈도우가 활성화될 때 호출되는 메소드
    /// </summary>
    /// <returns></returns>
    private void OnEnable()
    {
        // if (characterPrefab != null && hitBoxFrameData != null && hitBoxFrameData.frames.Count > 0)
        // {
        //     FrameData currentFrame = hitBoxFrameData.frames[currentFrameIndex];
        //     VisualizeHitAndHurtBoxes(characterPrefab, currentFrame);
        // }

        SceneView.duringSceneGui += OnSceneGUI;
    }

    /// <summary>
    /// 에디터 윈도우가 비활성화될 때 호출되는 메소드
    /// </summary>
    /// <returns></returns>
    private void OnDisable()
    {
        // if (characterPrefab != null)
        // {
        //     ClearVisualization(characterPrefab);
        // }

        SceneView.duringSceneGui -= OnSceneGUI;
        StopPreview();
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
