using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 애니메이션을 제어하는 클래스
/// </summary>
public class UnitAnimationController : MonoBehaviour
{
    public Animator Animator { get; set; }

    private void Awake()
    {
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

    }
}
