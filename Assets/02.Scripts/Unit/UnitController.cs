using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// to do : Start시에 플레이어의 컨트롤ID ( 1P인지 2P인지 ) 와 Unit의 컨트롤ID가 같을 경우, 조작 가능 플래그를 활성화

/// <summary>
/// 유닛의 행동을 제어하는 메소드
/// </summary>
[RequireComponent(typeof(Unit))]
public class UnitController : MonoBehaviour
{
    #region Variable

    [Header("Unit Control Variables")]
    private Unit unit;
    private Vector3 velocity;   // 위치값 기반의 유닛 속력
    public Vector3 Velocity { get { return velocity; } set { velocity = value; }}
    private bool IsControllable { get; set; } // 유닛 조작 가능 여부

    [Header("Detect Variable")]
    [SerializeField] private LayerMask GroundLayer;   // 감지한 지면의 LayerMask
    [SerializeField] private LayerMask WallLayer;   // 감지한 지면의 LayerMask

    [field: SerializeField] public float Attack { get; set; } // 유닛 공격력
    [field: SerializeField] public float Health { get; set; } // 유닛 체력
    [field: SerializeField] public float Defense { get; set; } // 유닛 방어력
    [field: SerializeField] public float MoveSpeed { get; set; } // 유닛 속도
    [field: SerializeField] public float JumpForce { get; set; } // 유닛 점프력
    [field: SerializeField] public float Gravity { get; set; } // 유닛 중력 ( 점프력 조정 스테이터스 )
    [field: SerializeField] public float GroundRayRange { get; set; }    // 유닛이 바닥을 감지하기 위한 Ray 길이 변수 ( 캐릭터별로 상이한 값을 가질 수 있으니 각 캐릭터 스크립트의 초기화 부분에서 지정이 필요 )
    [field: SerializeField] public float WallRayRange { get; set; }    // 유닛이 벽을 감지하기 위한 Ray 길이 변수 ( 캐릭터별로 상이한 값을 가질 수 있으니 각 캐릭터 스크립트의 초기화 부분에서 지정이 필요 )
    [field: SerializeField] public float GroundPoint { get; set; } // 유닛과 지면사이의 거리
    [field: SerializeField] public float LeftWallPoint { get; set; } // 유닛의 왼쪽 벽 위치
    [field: SerializeField] public float RightWallPoint { get; set; } // 유닛의 오른쪽 벽 위치

    #endregion Variable

    #region Methods

    private void Awake()
    {
        unit = this.GetComponent<Unit>();

        Init();
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (IsGrounded())
        {
            velocity.y = 0; // 지면에 닿았을 경우, 중력을 적용시키지 않음
        }
        else
        {
            ApplyGravity(); // 매 프레임 중력 적용
        }

        transform.Translate(velocity * Time.deltaTime); // 캐릭터 위치값 업데이트
    }

    /// <summary>
    /// 초기화 메소드
    /// </summary>
    private void Init()
    {
        // 유닛 스테이터스 지정
        Attack = unit.UnitData.Attack;
        Defense = unit.UnitData.Defense;
        Health = unit.UnitData.Health;
        MoveSpeed = unit.UnitData.MoveSpeed;
        JumpForce = unit.UnitData.JumpForce;
        Gravity = unit.UnitData.Gravity;

        GroundRayRange = 0.5f;    // 테스트용 코드 ( 상정 상황은 각 유닛별로 Ray를 다르게 주는 것이 전제 )
        WallRayRange = 0.5f;  // 테스트용 코드 ( 상정 상황은 각 유닛별로 Ray를 다르게 주는 것이 전제 )

        if (GroundLayer == 0)   // GroundLayer 값을 설정하지 않아 기본값으로 설정되어 있을 경우, GroundLayer값을 수동으로 설정한다
        {
            // 값이 고정되어 있기 때문에 Layer순서가 변경되면 에러를 발생시킬 수 있지만, GroundLayer가 설정되어 있지 않았을 때의 비상상황을 상정한 것이기 때문에 본 사양을 채택
            // enum을 사용하는 방식도 생각해보았지만, 어차피 Layer순서나 이름이 변경되면 에러를 일으키는 것은 변함없기 때문에 고정값을 사용
            // GroundLayer = (1 << 12);
            GroundLayer = LayerMask.GetMask("Ground");
        }

        if (WallLayer == 0)   // WallLayer 값을 설정하지 않아 기본값으로 설정되어 있을 경우, WallLayer값을 수동으로 설정한다
        {
            // 값이 고정되어 있기 때문에 Layer순서가 변경되면 에러를 발생시킬 수 있지만, WallLayer가 설정되어 있지 않았을 때의 비상상황을 상정한 것이기 때문에 본 사양을 채택
            // enum을 사용하는 방식도 생각해보았지만, 어차피 Layer순서나 이름이 변경되면 에러를 일으키는 것은 변함없기 때문에 고정값을 사용
            // WallLayer = (1 << 13);
            GroundLayer = LayerMask.GetMask("Wall");
        }
    }

    public bool IsGrounded()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, GroundRayRange, GroundLayer);

        Debug.DrawRay(transform.position, Vector3.down * GroundRayRange, isGrounded ? Color.green : Color.red); // Ray를 가시적으로 확인하기 위한 테스트용 기즈모 출력

        return isGrounded;
    }

    public bool IsBumpWall()
    {
        bool isFrontBumpWall = Physics.Raycast(transform.position, Vector3.forward, WallRayRange, WallLayer);
        bool isBackBumpWall = Physics.Raycast(transform.position, Vector3.back, WallRayRange, WallLayer);

        Debug.DrawRay(transform.position, Vector3.forward * WallRayRange, isFrontBumpWall ? Color.green : Color.red); // Ray를 가시적으로 확인하기 위한 테스트용 기즈모 출력
        Debug.DrawRay(transform.position, Vector3.back * WallRayRange, isBackBumpWall ? Color.green : Color.red); // Ray를 가시적으로 확인하기 위한 테스트용 기즈모 출력

        return isFrontBumpWall || isBackBumpWall ? true : false;
    }

    private void ApplyGravity()
    {
        velocity.y += Gravity * Time.deltaTime;
    }
    
    #endregion Methods
}
