using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 히트박스 및 허트박스 데이터를 프레임 단위로 저장하는 클래스
/// </summary>
[System.Serializable]
public class FrameData
{
    public int frameNumber;
    public List<HitBoxData> hitboxes;
    public List<HurtBoxData> hurtboxes;
}

/// <summary>
/// 한 모션에 대한 프레임 데이터를 저장하는 스크립터블 오브젝트
/// </summary>
[CreateAssetMenu(fileName = "HitBox Frame Data", menuName = "Scriptable Objects/HitBoxFrameData", order = 1)]
public class HitBoxFrameData : ScriptableObject
{
    public AnimationClip targetAnimationClip;   // 실제로 적용할 애니메이션 클립
    public List<FrameData> frames;
}