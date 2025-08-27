using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 유닛의 상태를 관리하는 상태 머신 클래스
/// </summary>
[RequireComponent(typeof(Unit))]
public class UnitStateMachine : MonoBehaviour
{
    private IUnitState currentState;    // 현재 상태를 나타내는 변수
    private List<IUnitState> states;    // 중첩 상태 구현을 위한 리스트 컬렉션
    public Unit Unit;

    [Header("Unit Ground States")]
    public UnitGroundState groundState;
    public UnitGroundIdleState groundIdleState;
    public UnitGroundWalkState groundWalkState;

    [Header("Unit Crouch States")]
    public UnitCrouchState crouchState;

    public PlayerInput PlayerInputActions { get; set; } // Input System 기반의 플레이어 입력 처리용 클래스

    private void Awake()
    {
        Unit = GetComponent<Unit>();

        // to do : 각 상태 클래스 생성
        // ex : idleState = new IdleState();
        // states.Add(new IdleState());
        states = new List<IUnitState>();

        // 각 상태 클래스 생성
        groundState = new UnitGroundState(this);
        groundIdleState = new UnitGroundIdleState(this);
        groundWalkState = new UnitGroundWalkState(this);
        crouchState = new UnitCrouchState(this);

        PlayerInputActions = GameManager.Instance.GetManager<InputManager>(typeof(InputManager)).PlayerInputActions;    // 매니저 클래스를 통해 Input System 인스턴스 정보 취득
    }

    void Start()
    {
        Init(); // 상태 초기화
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    private void Init()
    {
        ChangeUnitState(groundState);   // 테스트용 더미 코드. to do : 기본 상태를 어떻게 처리할 것인지 고민 필요

        // '앉기' 상태의 입력 처리 등록
        PlayerInputActions.Unit.Crouch.performed += OnCrouchPerformed;
        PlayerInputActions.Unit.Crouch.canceled += OnCrouchCancled;
    }

    public void ChangeUnitState(IUnitState state)
    {
        if (currentState != null)
        {
            currentState.Exit();
            states.Remove(currentState);
        }

        currentState = state;
        currentState.Enter();
        states.Add(currentState);
    }

    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        if (currentState == groundState || currentState == groundIdleState || currentState == groundWalkState)  // to do : 달리기 상태에서도 앉을 수 있도록 수정 필요
        {
            ChangeUnitState(crouchState);
        }
    }

    private void OnCrouchCancled(InputAction.CallbackContext context)
    {
        if (currentState == crouchState)
        {
            ChangeUnitState(groundState);
        }
    }
}
