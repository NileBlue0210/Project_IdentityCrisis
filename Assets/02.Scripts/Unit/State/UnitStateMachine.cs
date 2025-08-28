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
    public int PendingDashDirection { get; set; }

    [Header("State Informations")]
    private IUnitState currentState;    // 현재 상태를 나타내는 변수

    [Header("Other States")]
    public UnitSpawnState SpawnState;

    [Header("Unit Ground States")]
    public UnitGroundState GroundState;
    public UnitGroundIdleState GroundIdleState;
    public UnitGroundWalkState GroundWalkState;
    public UnitGroundDashState GroundDashState;

    [Header("Unit Crouch States")]
    public UnitCrouchState CrouchState;

    [Header("Unit Aerial States")]
    public UnitAerialState AerialState;
    public UnitJumpState JumpState;

    public PlayerInput PlayerInputActions { get; set; } // Input System 기반의 플레이어 입력 처리용 클래스

    private void Awake()
    {
        Unit = GetComponent<Unit>();
        inputSequenceController = GameManager.Instance.GetManager<InputSequenceManager>(typeof(InputSequenceManager));

        // 각 상태 클래스 생성
        SpawnState = new UnitSpawnState(this);

        GroundState = new UnitGroundState(this);
        GroundIdleState = new UnitGroundIdleState(this);
        GroundWalkState = new UnitGroundWalkState(this);
        GroundDashState = new UnitGroundDashState(this);

        CrouchState = new UnitCrouchState(this);

        AerialState = new UnitAerialState(this);
        JumpState = new UnitJumpState(this);

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
            inputSequenceController.RegisterAxisAction(PlayerInputActions.Unit.Move, "Move", OnMultiTapDetected, requiredTapCount: 2, window: 0.20f, threshold: 0.5f);
        }
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
        if (currentState == GroundState || currentState == GroundIdleState || currentState == GroundWalkState)  // to do : 달리기 상태에서도 앉을 수 있도록 수정 필요
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
        if (currentState == GroundState || currentState == GroundIdleState || currentState == GroundWalkState)
        {
            if (Unit.UnitController.IsGrounded())
            {
                ChangeUnitState(JumpState);
            }
        }
    }

    private void OnMultiTapDetected(string id, int tapCount, int direction)
    {
        int dir = direction;

        if (dir == 0)
        {
            var InputDirectionX = GameManager.Instance.GetManager<InputManager>(typeof(InputManager)).PlayerInputActions.Unit.Move.ReadValue<Vector2>().x;

            dir = InputDirectionX > 0 ? 1 : (InputDirectionX < 0 ? -1 : (int)transform.localScale.x);
        }

        int facing = (int)Mathf.Sign(transform.localScale.x);
        bool isForward = Mathf.Sign(dir) == Mathf.Sign(facing);

        if (isForward)
        {
            // 앞대시: 대시 상태로 전이
            PendingDashDirection = dir;
            ChangeUnitState(GroundDashState); // DashState에서 PendingDashDirection을 읽도록 설계
        }
        else
        {
            // 백대시: 별도 State 혹은 DashState에서 isBackDash 플래그로 처리
            PendingDashDirection = dir;
            ChangeUnitState(GroundDashState);
        }
    }
}
