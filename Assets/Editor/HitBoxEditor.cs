using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    // 새 히트, 허트박스 추가를 위한 디폴트 값 설정 변수
    private string newHitboxName = "HitBox_";
    private Vector3 newHitboxSize = Vector3.zero;
    private Vector3 newHitboxOffset = Vector3.zero;
    private float newHitboxDamage = 0f;
    private Vector3 newHitboxKnockback = Vector3.zero;
    private string newHurtboxName = "HurtBox_";
    private Vector3 newHurtboxSize = Vector3.zero;
    private Vector3 newHurtboxOffset = Vector3.zero;

    [Header("Copy HitBox Data Properties")]
    // 히트, 허트박스 데이터 복사를 위한 변수
    private int startFrameIndex;
    private int endFrameIndex;

    // 윈도우를 열기 위한 메뉴 아이템 추가
    [MenuItem("Window/HitBox Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(HitBoxEditor), false, "HitBox Editor");    // 기존 열려있는 윈도우가 있다면 포커스, 없다면 윈도우를 새로 생성
    }

    /// <summary>
    /// 히트박스 에디터를 구현하는 메소드
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
                hitbox.size = EditorGUILayout.Vector2Field("Size", hitbox.size);
                hitbox.offset = EditorGUILayout.Vector2Field("Offset", hitbox.offset);
                hitbox.damage = EditorGUILayout.FloatField("Damage", hitbox.damage);
                hitbox.knockback = EditorGUILayout.Vector2Field("Knockback", hitbox.knockback);

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
            newHitboxSize = EditorGUILayout.Vector2Field("Size", newHitboxSize);
            newHitboxOffset = EditorGUILayout.Vector2Field("Offset", newHitboxOffset);
            newHitboxDamage = EditorGUILayout.FloatField("Damage", newHitboxDamage);
            newHitboxKnockback = EditorGUILayout.Vector2Field("Knockback", newHitboxKnockback);

            // 새로운 히트박스 데이터 추가
            if (GUILayout.Button("Add HitBox"))
            {
                currentFrame.hitboxes.Add(new HitBoxData
                {
                    hitboxName = newHitboxName,
                    size = newHitboxSize,
                    offset = newHitboxOffset,
                    damage = newHitboxDamage,
                    knockback = newHitboxKnockback
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
                hurtbox.size = EditorGUILayout.Vector2Field("Size", hurtbox.size);
                hurtbox.offset = EditorGUILayout.Vector2Field("Offset", hurtbox.offset);

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
            newHurtboxSize = EditorGUILayout.Vector2Field("Size", newHurtboxSize);
            newHurtboxOffset = EditorGUILayout.Vector2Field("Offset", newHurtboxOffset);

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
            // 히트박스 데이터 복제 섹션
            GUILayout.Label("Copy HitBox Data", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();

            // 사용자에게 히트박스 복제 범위를 입력받음
            startFrameIndex = EditorGUILayout.IntField("Copy Start Frame", startFrameIndex);
            endFrameIndex = EditorGUILayout.IntField("Copy End Frame", endFrameIndex);

            GUILayout.EndHorizontal();

            // 입력 값 보정
            if (hitBoxFrameData.frames.Count > 0)
            {
                startFrameIndex = Mathf.Clamp(startFrameIndex, 0, hitBoxFrameData.frames.Count - 1);
                endFrameIndex = Mathf.Clamp(endFrameIndex, 0, hitBoxFrameData.frames.Count - 1);
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Copy HitBox Datas"))
            {
                CopyFrameData(startFrameIndex, endFrameIndex, true);
            }
            if (GUILayout.Button("Copy HurtBox Datas"))
            {
                CopyFrameData(startFrameIndex, endFrameIndex, false);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // BeginChangeCheck를 기준으로 변경 내역을 감지
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(hitBoxFrameData, "Record Edit HitBox And HurtBox Frame Data");   // 변경 내역 기록 ( Ctr + Z 커맨드를 통해 작업 내용을 되돌릴 수 있다 )
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

    /// <summary>
    /// 에디터 편집 내용을 씬에 적용시키는 메소드
    /// </summary>
    /// <param name="sceneView"></param>
    private void OnSceneGUI(SceneView sceneView)
    {
        if (characterPrefab == null || hitBoxFrameData == null || hitBoxFrameData.frames.Count == 0)
            return;

        // 기즈모를 그릴 때 현재 프레임 데이터가 유효한지 확인
        if (currentFrameIndex >= 0 && currentFrameIndex < hitBoxFrameData.frames.Count)
        {
            FrameData currentFrame = hitBoxFrameData.frames[currentFrameIndex];

            Handles.matrix = characterPrefab.transform.localToWorldMatrix;

            // 히트박스 기즈모 그리기
            foreach (var hitbox in currentFrame.hitboxes)
            {
                Rect rect = new Rect(
                    hitbox.offset.x - hitbox.size.x / 2,
                    hitbox.offset.y - hitbox.size.y / 2,
                    hitbox.size.x,
                    hitbox.size.y
                );
                Handles.DrawSolidRectangleWithOutline(rect, new Color(1, 0, 0, 0.2f), Color.red);
            }

            // 허트박스 기즈모 그리기
            foreach (var hurtbox in currentFrame.hurtboxes)
            {
                Rect rect = new Rect(
                    hurtbox.offset.x - hurtbox.size.x / 2,
                    hurtbox.offset.y - hurtbox.size.y / 2,
                    hurtbox.size.x,
                    hurtbox.size.y
                );
                Handles.DrawSolidRectangleWithOutline(rect, new Color(0, 1, 0, 0.2f), Color.green);
            }
        }
    }

    /// <summary>
    /// 히트, 허트박스 데이터를 복사하는 메소드
    /// </summary>
    /// <param name="startFrameIndex"></param>
    /// <param name="endFrameIndex"></param>
    /// <param name="isHitbox"></param>
    private void CopyFrameData(int startFrameIndex, int endFrameIndex, bool isHitBox)
    {
        // 매개변수 유효성 검사
        if (hitBoxFrameData == null || startFrameIndex < 0 || endFrameIndex < 0 || startFrameIndex >= hitBoxFrameData.frames.Count || endFrameIndex >= hitBoxFrameData.frames.Count || startFrameIndex > endFrameIndex)
        {
            Debug.LogError("Invalid Properties");

            return;
        }

        // 되돌리기 수행을 위한 편집 내역 기록
        Undo.RecordObject(hitBoxFrameData, "Record Copy HitBox Or HurtBox Frame Data");

        FrameData frameData = hitBoxFrameData.frames[currentFrameIndex];
        Match nameTagMatch;    // 복사된 히트박스 이름을 갱신하기 위한 매치 필드

        for (int i = startFrameIndex; i <= endFrameIndex; i++)
        {
            FrameData targetFrame = hitBoxFrameData.frames[i];

            // 히트박스 복제 시
            if (isHitBox)
            {
                targetFrame.hitboxes.Clear();   // 기존 히트박스 데이터 삭제

                foreach (HitBoxData hitbox in frameData.hitboxes)
                {
                    nameTagMatch = Regex.Match(hitbox.hitboxName, @"(\D+)(\d+)");    // 기존 이름 필드에서 문자 부분만 추출 ( 기본적으로 히트, 허트박스 이름은 HitBox_로 시작한다는 전제 )

                    // 히트박스 이름이 양식을 따르는 경우
                    if (nameTagMatch.Success)
                    {
                        // 히트박스 복제
                        targetFrame.hitboxes.Add(new HitBoxData
                        {
                            hitboxName = $"{nameTagMatch.Groups[1].Value}_{i}",
                            size = hitbox.size,
                            offset = hitbox.offset
                        });
                    }
                    else
                    {
                        targetFrame.hitboxes.Add(new HitBoxData
                        {
                            hitboxName = hitbox.hitboxName, // 양식을 따르지 않았을 경우, 히트박스 명은 그대로 복사한다
                            size = hitbox.size,
                            offset = hitbox.offset
                        });
                    }
                }
            }
            else    // 허트박스 복제 시
            {
                targetFrame.hurtboxes.Clear();  // 기존 허트박스 데이터 삭제

                foreach (HurtBoxData hurtbox in frameData.hurtboxes)
                {
                    nameTagMatch = Regex.Match(hurtbox.hurtboxName, @"(\D+)(\d+)");    // 기존 이름 필드에서 문자 부분만 추출 ( 기본적으로 히트, 허트박스 이름은 HitBox_로 시작한다는 전제 )

                    // 허트박스 이름이 양식을 따르는 경우
                    if (nameTagMatch.Success)
                    {
                        // 허트박스 복제
                        targetFrame.hurtboxes.Add(new HurtBoxData
                        {
                            hurtboxName = $"{nameTagMatch.Groups[1].Value}_{i}",
                            size = hurtbox.size,
                            offset = hurtbox.offset
                        });
                    }
                    else
                    {
                        targetFrame.hurtboxes.Add(new HurtBoxData
                        {
                            hurtboxName = hurtbox.hurtboxName, // 양식을 따르지 않았을 경우, 허트박스 명은 그대로 복사한다
                            size = hurtbox.size,
                            offset = hurtbox.offset
                        });
                    }
                }
            }
        }

        Debug.Log($"Copy {(isHitBox ? "HitBox" : "HurtBox")} {startFrameIndex} to {endFrameIndex} successful ( from {currentFrameIndex})");

        EditorUtility.SetDirty(hitBoxFrameData);    // 이력 변경 표시 활성화
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
    
    /// <summary>
    /// 에디터 윈도우가 닫힐 때 호출되는 메소드
    /// </summary>
    /// <returns></returns>
    private void OnDestroy()
    {
        AnimationMode.StopAnimationMode();
    }
}
