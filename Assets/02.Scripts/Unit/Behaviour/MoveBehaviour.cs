using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 이동 상태를 제어하는 클래스
/// </summary>
public class MoveBehaviour : MonoBehaviour
{
    [field: SerializeField] public float Velocity { get; set; } // 컨트롤러에서 받아오는 유닛 속도

    void Start()
    {

    }

    void Update()
    {

    }

    /// <summary>
    /// 유닛 이동을 제어하는 메소드
    /// </summary>
    /// <param name="direction"></param>
    public void Move(Vector3 direction)
    {

    }
}
