using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 애니메이션을 제어하는 클래스
/// </summary>
public class UnitAnimationController : MonoBehaviour
{
    /*
    to do : 
    1. stateMachine에서 상태 변경 시, UpdateAnimationInfo 메소드를 실행하도록 구현
    2. AnimationController의 Update에서 현재 재생중인 애니메이션 클립과 현재 프레임을 UnitHitBoxController의 메소드에 매개변수로 전달
    3. 해당하는 프레임 데이터를 리스트에서 찾아 히트박스 출력
    */
    [Header("Unit Components")]
    private Unit unit;
    public Animator Animator { get; set; }

    [Header("Current Animation Information")]
    private AnimatorStateInfo currentAnimatorState;
    private AnimatorClipInfo[] currentAnimatorInfos;
    private float currentClipLength;    // 애니메이션 클립 길이
    private float currentClipFrameRate; // 애니메이션 프레임 레이트
    private float currentNormalizedTime;    // 현재 프레임 정규화 시간
    private float currentClipTime;  // 현재 프레임 시간
    private int currentClipFrame;   // 현재 프레임 번호
    private int currentClipTotalFrame;  // 애니메이션 전체 프레임 수

    [Header("Animation Events")]
    public Action OnAnimationStarted;   // 애니메이션 시작 이벤트
    public Action OnAnimationFinished;  // 애니메이션 종료 이벤트

    private void Awake()
    {
        // 컴포넌트 취득
        unit = this.GetComponent<Unit>();
        Animator = this.GetComponentInChildren<Animator>();

        if (Animator == null)
        {
            Debug.LogError("Animator is Null");
        }
    }

    void Start()
    {

    }

    void Update()
    {
        // 테스트용 임시 코드. 페이즈 기능 도입 후 삭제 필요
        if (unit.UnitHitBoxController.hitBoxDatas.Count == 0 || unit.UnitHitBoxController.hitBoxDatas == null)
            return;

        UpdateAnimationInfo();

        // 현재 프레임 정보 업데이트
        currentClipTime = currentNormalizedTime * currentClipLength;
        currentClipFrame = (int)(currentClipTime * currentClipFrameRate);

        unit.UnitHitBoxController.SetCurrentFrameData(currentClipFrame);
    }

    /// <summary>
    /// 애니메이션 재생 시 필요한 프레임 정보를 업데이트하는 메소드
    /// </summary>
    public void UpdateAnimationInfo()
    {
        currentAnimatorState = Animator.GetCurrentAnimatorStateInfo(0);
        currentAnimatorInfos = Animator.GetCurrentAnimatorClipInfo(0);

        // 애니메이션 전환 중 Null이 반환될 때의 예외처리
        if (currentAnimatorInfos.Length == 0)
        {
            return;
        }

        if (currentAnimatorState.normalizedTime >= 1.0f)
        {
            OnAnimationFinished?.Invoke();
        }

        // 현재 재생중인 애니메이션 클립 정보
        AnimationClip clip = currentAnimatorInfos[0].clip;

        currentNormalizedTime = currentAnimatorState.normalizedTime % 1f;
        currentClipLength = clip.length;
        currentClipFrameRate = clip.frameRate;
        currentClipTotalFrame = Mathf.CeilToInt(currentClipLength * currentClipFrameRate);

        // 히트박스 데이터 설정
        unit.UnitHitBoxController.SetCurrentHitBoxData(clip);
    }
    
    /// <summary>
    /// 모션 종료 후 대기 애니메이션으로 자동 전환하는 메소드
    /// </summary>
    public void ShiftToIdleAnimation()
    {
        Animator.Play("Idle");
    }
}
