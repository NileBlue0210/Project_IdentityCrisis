using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 히트박스 및 허트박스를 편집하는 에디터 스크립트
/// </summary>
public class HitBoxEditor : EditorWindow
{
    private HitBoxFrameData hitBoxFrameData;
    private int currentFrameIndex = 0;
    private GameObject characterPrefab;
    private bool livePreview; // 히트, 허트박스 미리보기 토글

    // 윈도우를 열기 위한 메뉴 아이템 추가
    [MenuItem("Window/HitBox Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(HitBoxEditor), false, "HitBox Editor");    // 기존 열려있는 윈도우가 있다면 포커스, 없다면 윈도우를 새로 생성
    }

    // private void OnGUI()
    // {
    //     hitBoxFrameData = (HitBoxFrameData)EditorGUILayout.ObjectField("Animation Frame Data", hitBoxFrameData, typeof(HitBoxFrameData), false);
    //     characterPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab", characterPrefab, typeof(GameObject), false);

    //     if (hitBoxFrameData != null)
    //     {
    //         // 프레임 조절을 위한 슬라이더 UI
    //         currentFrameIndex = EditorGUILayout.IntSlider("Current Frame", currentFrameIndex, 0, Mathf.Max(0, hitBoxFrameData.frames.Count - 1));

    //         // 에디터를 통한 히트박스 추가 버튼 로직
    //         if (GUILayout.Button("Add HitBox to Current Frame"))
    //         {
    //             // 현재 프레임 데이터에 히트박스 데이터 추가
    //             if (currentFrameIndex < hitBoxFrameData.frames.Count)
    //             {
    //                 hitBoxFrameData.frames[currentFrameIndex].hitboxes.Add(new HitBoxData());
    //             }
                
    //             Repaint(); // 추가 후 즉시 Scene 뷰 갱신
    //         }

    //         // 변경 내역 저장 버튼 로직
    //         if (GUILayout.Button("Save Data"))
    //         {
    //             EditorUtility.SetDirty(hitBoxFrameData);
    //             AssetDatabase.SaveAssets();
    //         }
    //     }
    // }
    
    // private void OnSceneGUI(SceneView sceneView)
    // {
    //     if (hitBoxFrameData == null || characterPrefab == null || hitBoxFrameData.frames.Count <= currentFrameIndex)
    //         return;

    //     FrameData currentFrame = hitBoxFrameData.frames[currentFrameIndex];
    //     Handles.color = Color.red;  // 히트박스, 허트박스 시각화

    //     // to do : 히트박스와 허트박스의 색을 구별해볼 것
    //     for (int i = 0; i < currentFrame.hitboxes.Count; i++)
    //     {
    //         var hitbox = currentFrame.hitboxes[i];
            
    //         // 핸들을 이용해 위치, 크기 조절
    //         hitbox.offset = Handles.PositionHandle(hitbox.offset, Quaternion.identity);
    //         hitbox.size = Handles.ScaleHandle(hitbox.size, hitbox.offset, Quaternion.identity, HandleUtility.GetHandleSize(hitbox.offset));
            
    //         // 박스 그리기
    //         Handles.DrawWireCube(hitbox.offset, hitbox.size);
    //     }

    //     // Scene 뷰 갱신
    //     if (GUI.changed)
    //     {
    //         EditorUtility.SetDirty(hitBoxFrameData);
    //     }
    // }


    /// <summary>
    /// 에디터 GUI 작성 메소드
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("HitBox and HurtBox Editor", EditorStyles.boldLabel);

        // 히트박스 및 허트박스 데이터와 캐릭터 프리팹 할당
        hitBoxFrameData = (HitBoxFrameData)EditorGUILayout.ObjectField("Animation Frame Data", hitBoxFrameData, typeof(HitBoxFrameData), false);
        characterPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab", characterPrefab, typeof(GameObject), true);

        // 각 필드 유효성 체크
        if (hitBoxFrameData == null)
        {
            EditorGUILayout.HelpBox("Please assign an Animation Frame Data ScriptableObject or create a new one.", MessageType.Warning);

            // 히트박스 데이터가 할당되지 않은 경우, 새로 할당할 수 있는 버튼 생성
            if (GUILayout.Button("Create New Animation Frame Data"))
            {
                hitBoxFrameData = new HitBoxFrameData();
                currentFrameIndex = 0;
            }

            return;
        }

        if (characterPrefab == null || EditorUtility.IsPersistent(characterPrefab))
        {
            EditorGUILayout.HelpBox("Please assign a character prefab.", MessageType.Warning);

            return;
        }

        GUILayout.Space(10);    // 가독성을 위한 공백 추가

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
                currentFrameIndex--;
            }

            GUILayout.Label($"Frame {currentFrameIndex + 1} of {hitBoxFrameData.frames.Count}");

            if (GUILayout.Button("Next Frame"))
            {
                currentFrameIndex++;
            }

            currentFrameIndex = Mathf.Clamp(currentFrameIndex, 0, hitBoxFrameData.frames.Count - 1);

            GUILayout.EndHorizontal();

            FrameData currentFrame = hitBoxFrameData.frames[currentFrameIndex];

            GUILayout.Space(10);

            GUILayout.Label($"Editing Frame {currentFrame.frameNumber}", EditorStyles.boldLabel);

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

            if (GUILayout.Button("Add HitBox"))
            {
                // currentFrame.hitboxes.Add(new HitBoxData("NewHitBox", Vector3.one, Vector3.zero, Vector3.up));
                currentFrame.hitboxes.Add(new HitBoxData());
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

            // 변경 사항 저장
            if (GUI.changed)
            {
                EditorUtility.SetDirty(hitBoxFrameData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // 캐릭터 프리팹에 히트박스 및 허트박스 시각화
            if (GUILayout.Button("Visualize on Character"))
            {
                VisualizeHitAndHurtBoxes(characterPrefab, currentFrame);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Clear Visualization"))
            {
                ClearVisualization(characterPrefab);
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

    /// <summary>
    /// 에디터 윈도우가 닫힐 때 호출되는 메소드
    /// </summary>
    /// <returns></returns>
    private void OnDestroy()
    {
        if (characterPrefab != null)
        {
            ClearVisualization(characterPrefab);
        }
    }

    /// <summary>
    /// 에디터 윈도우가 포커스를 잃을 때 호출되는 메소드
    /// </summary>
    /// <returns></returns>
    private void OnFocusLost()
    {
        if (characterPrefab != null)
        {
            ClearVisualization(characterPrefab);
        }
    }

    /// <summary>
    /// 에디터 윈도우가 포커스를 얻을 때 호출되는 메소드
    /// </summary>
    /// <returns></returns>
    private void OnFocusGained()
    {
        if (characterPrefab != null && hitBoxFrameData != null && hitBoxFrameData.frames.Count > 0)
        {
            FrameData currentFrame = hitBoxFrameData.frames[currentFrameIndex];
            VisualizeHitAndHurtBoxes(characterPrefab, currentFrame);
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
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!livePreview || characterPrefab == null || hitBoxFrameData == null || hitBoxFrameData.frames.Count == 0)
            return;

        var frame = hitBoxFrameData.frames[currentFrameIndex];
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        // 캐릭터 트랜스폼 기준으로 그리기
        using (new Handles.DrawingScope(characterPrefab.transform.localToWorldMatrix)) {
            // 히트박스: 빨강 외곽선
            foreach (var hb in frame.hitboxes) {
                Handles.color = new Color(1f, 0f, 0f, 0.8f);
                Handles.DrawWireCube(hb.offset, hb.size);
                // (원하시면 Position/Scale 핸들 추가해 실시간 편집도 가능)
            }
            
            // 허트박스: 파랑 외곽선
            foreach (var hb in frame.hurtboxes)
            {
                Handles.color = new Color(0f, 0.6f, 1f, 0.8f);
                Handles.DrawWireCube(hb.offset, hb.size);
            }
        }
        
        sceneView.Repaint();
    }

    /// <summary>
    /// 에디터 윈도우가 리셋될 때 호출되는 메소드
    /// </summary> <returns></returns>
    private void OnReset()
    {
        if (characterPrefab != null)
        {
            ClearVisualization(characterPrefab);
        }
    }

    /// <summary>
    /// 에디터 윈도우가 업데이트될 때 호출되는 메소드
    /// </summary> <returns></returns>
    private void OnUpdate()
    {
        Repaint();
    }
}
