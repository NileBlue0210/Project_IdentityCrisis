using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxManager : MonoBehaviour
{
    public HitBoxFrameData animationFrameData;
    private int currentFrame;
    private BoxCollider[] hurtboxes;

    private void Awake()
    {
        // 캐릭터에 허트박스 콜라이더들을 미리 생성하고 관리
        // (또는 에디터에서 생성된 데이터 기반으로 런타임에 생성)
    }

    public void OnAnimationUpdate(int frameNumber)
    {
        this.currentFrame = frameNumber;
        if (currentFrame >= animationFrameData.frames.Count) return;

        // 1. 현재 프레임의 히트박스 데이터 가져오기
        var currentFrameData = animationFrameData.frames[currentFrame];
        var activeHitboxes = currentFrameData.hitboxes;

        // 2. 히트박스 충돌 판정
        foreach (var hitboxData in activeHitboxes)
        {
            // 월드 좌표로 변환
            Vector3 worldHitboxPosition = transform.position + hitboxData.offset;

            // 3. 상대방 캐릭터와 충돌 판정
            // 모든 상대방의 허트박스들과 충돌 검사
            foreach (var opponent in FindObjectsOfType<HitBoxManager>())
            {
                if (opponent == this) continue;

                foreach (var hurtboxData in opponent.hurtboxes)
                {
                    // AABB(Axis-Aligned Bounding Box) 충돌 검사
                    if (IsColliding(worldHitboxPosition, hitboxData.size, opponent.transform.position + hurtboxData.center, hurtboxData.size))
                    {
                        // 충돌 시 이벤트 발생
                        // opponent.TakeDamage();

                        Debug.Log("Hit detected!");
                    }
                }
            }
        }
    }

    private bool IsColliding(Vector3 pos1, Vector3 size1, Vector3 pos2, Vector3 size2)
    {
        // 간단한 AABB 충돌 검사 로직
        // 두 박스의 경계가 겹치는지 확인
        return (Mathf.Abs(pos1.x - pos2.x) * 2 < size1.x + size2.x) &&
               (Mathf.Abs(pos1.y - pos2.y) * 2 < size1.y + size2.y) &&
               (Mathf.Abs(pos1.z - pos2.z) * 2 < size1.z + size2.z);
    }
}
