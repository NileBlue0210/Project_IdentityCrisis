using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 게임 내 모든 입력을 처리하는 InputManager 클래스
/// </summary>
public class InputManager : MonoBehaviour
{
    public PlayerInput PlayerInputActions { get; set; } // 플레이어 입력 액션. 플레이어블 유닛이 되는 대상 타겟의 Controller 클래스에 넘겨주어 입력받은 대상이 움직이도록 처리
    public Vector2 Move { get; private set; }

    private void Awake()
    {
        // 각 Input Action 초기화
        PlayerInputActions = new PlayerInput();

        OnPlayerActionEnable(); // 플레이어 Input Action 활성화
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void OnPlayerActionEnable()
    {
        PlayerInputActions.Enable();
    }

    public void OnPlayerUnitActionEnable()
    {
        PlayerInputActions.Unit.Enable();
    }

    public void OnPlayerUnitActionDisable()
    {
        PlayerInputActions.Unit.Disable();
    }

    public void OnPlayerUIActionEnable()
    {
        PlayerInputActions.UI.Enable();
    }

    public void OnPlayerUIActionDisable()
    {
        PlayerInputActions.UI.Disable();
    }
}
