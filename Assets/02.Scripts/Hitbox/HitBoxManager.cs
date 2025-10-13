using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxManager : MonoBehaviour
{
    public HitBoxFrameData animationFrameData;
    private int currentFrame;
    private int facingDirection = 1; // 1: 오른쪽, -1: 왼쪽

    // 충돌 처리를 위한 상대 허트박스 데이터
    private static readonly List<HitBoxManager> allHitBoxManagers = new List<HitBoxManager>();
    private void OnEnable() => allHitBoxManagers.Add(this);
    private void OnDisable() => allHitBoxManagers.Remove(this);

    private void Awake()
    {
        // 캐릭터에 허트박스 콜라이더들을 미리 생성하고 관리
        // (또는 에디터에서 생성된 데이터 기반으로 런타임에 생성)
    }

    public void OnAnimationUpdate(int frameNumber)
    {
        if (animationFrameData == null || frameNumber < 0 || frameNumber >= animationFrameData.frames.Count)
            return;

        var frame = animationFrameData.frames[frameNumber];

        foreach (var hitbox in frame.hitboxes)
        {
            // 좌우 반전 적용한 로컬 오프셋
            Vector3 localOffset = hitbox.offset; localOffset.x *= facingDirection;
            // 월드 변환
            Vector3 worldCenter = transform.TransformPoint(localOffset);
            Vector3 worldSize = Vector3.Scale(hitbox.size, transform.lossyScale);
            // 2D면 z는 0으로 고정하거나 무시

            foreach (var op in allHitBoxManagers)
            {
                if (op == this) continue;
                // 상대 허트박스 구성(컴포넌트/데이터 중 하나로 통일 권장)
                foreach (var hurtCol in op.GetComponentsInChildren<BoxCollider>())
                {
                    Vector3 oc = op.transform.TransformPoint(hurtCol.center);
                    Vector3 os = Vector3.Scale(hurtCol.size, op.transform.lossyScale);
                    if (IsColliding(worldCenter, worldSize, oc, os))
                    {
                        // TODO: 대미지/히트스톱/넉백 처리
                    }
                }
            }
        }

        // currentFrame = frameNumber;

            // // 1. 현재 프레임의 히트박스 데이터 가져오기
            // var currentFrameData = animationFrameData.frames[currentFrame];
            // var activeHitboxes = currentFrameData.hitboxes;

            // // 2. 히트박스 충돌 판정
            // foreach (var hitboxData in activeHitboxes)
            // {
            //     // 월드 좌표로 변환
            //     Vector3 worldHitboxPosition = transform.position + hitboxData.offset;

            //     // 3. 상대방 캐릭터와 충돌 판정
            //     // 모든 상대방의 허트박스들과 충돌 검사
            //     foreach (var opponent in FindObjectsOfType<HitBoxManager>())
            //     {
            //         if (opponent == this) continue;

            //         foreach (var hurtboxData in opponent.hurtboxes)
            //         {
            //             // AABB(Axis-Aligned Bounding Box) 충돌 검사
            //             if (IsColliding(worldHitboxPosition, hitboxData.size, opponent.transform.position + hurtboxData.center, hurtboxData.size))
            //             {
            //                 // 충돌 시 이벤트 발생
            //                 // opponent.TakeDamage();

            //                 Debug.Log("Hit detected!");
            //             }
            //         }
            //     }
            // }
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
