using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 이동 상태를 제어하는 클래스
/// </summary>
public class MoveBehaviour : UnitBehaviour, IUnitBehaviour
{
    private Vector2 moveDirection;  // 이동 방향
    public bool IsMoveable { get; set; }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {

    }

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// 초기화 메소드
    /// </summary>
    private void Init()
    {
        IsMoveable = true;

        SetUnitMoveAction();
    }

    /// <summary>
    /// 유닛의 상태를 변경하는 메소드
    /// </summary>
    public void SetBehaviour()
    {
        CurrentBehaviour = this;
    }

    void IUnitBehaviour.Update()
    {
        Update();
    }

    /// <summary>
    /// Input System을 통해 플레이어가 Move Action에 해당하는 키를 입력했을 때, 이동 방향을 저장하는 메소드
    /// </summary>
    private void SetUnitMoveAction()
    {
        Debug.Log("Check");
        // PlayerInput.Unit.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        PlayerInput.Unit.Move.performed += ctx =>
        {
            Debug.Log("check A");

            if (!object.ReferenceEquals(CurrentBehaviour, this))
            {
                Debug.Log("Change Behaviour : " + this.ToString());

                SetBehaviour();
            }

            Debug.Log("check B");

            moveDirection = ctx.ReadValue<Vector2>();
        };

        PlayerInput.Unit.Move.canceled += ctx => moveDirection = Vector2.zero; // 이동 키를 해제했을 경우, 이동 방향을 초기화
    }

    /// <summary>
    /// 유닛 이동을 제어하는 메소드
    /// </summary>
    public void Move()
    {
        if (!IsMoveable) return;

        UnitController.Rb.velocity = new Vector2(moveDirection.x * UnitController.Velocity, UnitController.Rb.velocity.y);
    }
}
