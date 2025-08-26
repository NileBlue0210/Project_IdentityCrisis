using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    None = -1,
    Idle,
    Move,
    Jump,
    Attack,
    Hit
}

public interface IUnitBehaviour
{
    public void SetBehaviour();
    public void Update();
}

/// <summary>
/// 유닛의 상태를 정의하는 상위 클래스
/// </summary>
public class UnitBehaviour : MonoBehaviour
{
    public Unit Unit { get; set; }
    public UnitController UnitController { get; set; }
    public IUnitBehaviour CurrentBehaviour { get; set; }
    public PlayerInput PlayerInput { get; set; }

    [Header("Behaviours")]
    private MoveBehaviour moveBehaviour;

    public virtual void Awake()
    {
        PlayerInput = GameManager.Instance.GetManager<InputManager>(typeof(InputManager)).PlayerInputActions;

        moveBehaviour = new MoveBehaviour();
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (CurrentBehaviour != null)
        {
            CurrentBehaviour.Update();
        }
    }

    private void OnGUI()
    {
        if (CurrentBehaviour != null)
        {
            CurrentBehaviour.SetBehaviour();
        }
    }
}
