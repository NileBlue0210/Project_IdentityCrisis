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
    [Header("Properties")]
    public Unit Unit;   // 각 상태에서 캐릭터를 제어하기 위한 변수
    private InputSequenceManager inputSequenceController;    // 연속 입력을 처리하기 위한 변수
    private List<IUnitState> ignoreJumpStates; // 점프 불가능한 상태 컬렉션
    private List<IUnitState> ignoreCrouchStates; // 앉기 불가능한 상태 컬렉션
    private List<IUnitState> ignoreDashStates; // 대시, 백대시 불가능한 상태 컬렉션

    [Header("State Informations")]
    private IUnitState currentState;    // 현재 상태를 나타내는 변수
    public IUnitState CurrentState { get { return currentState; } set { currentState = value; } }

    [Header("Other States")]
    public UnitSpawnState SpawnState;

    [Header("Unit Ground States")]
    public UnitGroundState GroundState;
    public UnitGroundIdleState GroundIdleState;
    public UnitGroundWalkState GroundWalkState;
    public UnitGroundDashState GroundDashState;
    public UnitGroundBackDashState GroundBackDashState;

    [Header("Unit Crouch States")]
    public UnitCrouchState CrouchState;

    [Header("Unit Aerial States")]
    public UnitAerialState AerialState;
    public UnitJumpState JumpState;
    public UnitAerialDashState AerialDashState;
    public UnitAerialBackDashState AerialBackDashState;

    public PlayerInput PlayerInputActions { get; set; } // Input System 기반의 플레이어 입력 처리용 클래스

    private void Awake()
    {
        Unit = GetComponent<Unit>();
        inputSequenceController = GameManager.Instance.GetManager<InputSequenceManager>(typeof(InputSequenceManager));

        // 기타 상태 클래스 생성
        SpawnState = new UnitSpawnState(this);

        // 지상 상태 클래스 생성
        GroundState = new UnitGroundState(this);
        GroundIdleState = new UnitGroundIdleState(this);
        GroundWalkState = new UnitGroundWalkState(this);
        GroundDashState = new UnitGroundDashState(this);
        GroundBackDashState = new UnitGroundBackDashState(this);

        // 앉은 상태 클래스 생성
        CrouchState = new UnitCrouchState(this);

        // 공중 상태 클래스 생성
        AerialState = new UnitAerialState(this);
        JumpState = new UnitJumpState(this);
        AerialDashState = new UnitAerialDashState(this);
        AerialBackDashState = new UnitAerialBackDashState(this);

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
        ChangeUnitState(SpawnState);

        // '앉기' 동작의 입력 처리 등록
        PlayerInputActions.Unit.Crouch.performed += OnCrouchPerformed;
        PlayerInputActions.Unit.Crouch.canceled += OnCrouchCancled;

        // '점프' 동작의 입력 처리 등록
        PlayerInputActions.Unit.Jump.performed += OnJumpPerformed;  // 점프 키를 떼었을 때 점프 상태를 해제시킬 필요는 없으므로 Cancled 로직은 구현하지 않는다

        if (inputSequenceController != null)
        {
            inputSequenceController.RegisterAxisAction(PlayerInputActions.Unit.Move, InputActionType.Move.ToString(), GroundDashState.OnDashInputDetected, requiredTapCount: 2, inputTerm: 0.25f, threshold: 0.5f);  // 대시 입력을 감지하는 콜백 함수 등록
        }

        SetIgnoreStates();
    }

    public void ChangeUnitState(IUnitState state)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = state;
        currentState.Enter();
    }

    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        if (CheckChangeStateAvailable(CrouchState))
        {
            ChangeUnitState(CrouchState);
        }
    }

    private void OnCrouchCancled(InputAction.CallbackContext context)
    {
        if (currentState == CrouchState)
        {
            ChangeUnitState(GroundState);
        }
    }

    /*
        to do : 
        1. 하이 점프의 경우, Crouch - Idle or Walk - Jump 의 순으로 입력이 감지되었을 때 플래그를 통해 발동하도록 구현해 볼 것
        2. 2단 점프의 경우, 점프 상태에서 JumpCount가 MaxJumpCount보다 작을 경우, 점프 상태로 재진입 할 수 있도록 구현해 볼 것
    */
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (CheckChangeStateAvailable(JumpState))
        {
            if (Unit.UnitController.IsGrounded())
            {
                ChangeUnitState(JumpState);
            }
        }
    }

    private void SetIgnoreStates()
    {
        // 점프 가능한 상태 컬렉션 초기화
        ignoreJumpStates = new List<IUnitState>
        {
            GroundState,
            GroundIdleState,
            GroundWalkState,
            GroundDashState
        };

        // 앉기 가능한 상태 컬렉션 초기화
        ignoreCrouchStates = new List<IUnitState>
        {
            GroundState,
            GroundIdleState,
            GroundWalkState,
            GroundDashState
        };

        // 대시, 백대시 불가능한 상태 컬렉션 초기화
        ignoreDashStates = new List<IUnitState>
        {
            AerialState,
            JumpState,
            AerialDashState,
            AerialBackDashState,
            CrouchState,
            GroundDashState,    // 중복 대시 방지
            GroundBackDashState // 중복 백대시 방지
        };
    }

    /// <summary>
    /// 특정 상태로의 전환이 가능한지 여부를 판단하는 메소드
    /// </summary>
    /// <param name="targetState"></param>
    /// <returns></returns>
    public bool CheckChangeStateAvailable(IUnitState targetState)
    {
        bool result = false;

        switch (targetState)
        {
            case IUnitState state when state == JumpState:
                result = ignoreJumpStates.Contains(currentState);
                break;
            case IUnitState state when state == CrouchState:
                result = ignoreCrouchStates.Contains(currentState);
                break;
            case IUnitState state when state == GroundDashState || state == GroundBackDashState:
                result = !ignoreDashStates.Contains(currentState);
                break;
            default:
                result = false;
                break;
        }

        return result;
    }
}
