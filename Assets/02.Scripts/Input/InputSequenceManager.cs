using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputActionType
{
    Move
}

/// <summary>
/// 연속 키 입력 등을 감지 및 처리하는 클래스
/// </summary>
public class InputSequenceManager : MonoBehaviour
{
    /// <summary>
    /// 방향 (축) 기반의 입력을 처리하는 클래스
    /// </summary>
    private class AxisRegister
    {
        public InputAction InputAction; // 입력을 감지할 Input Action
        public string Id;   // 좌, 우, 2P 입력 등의 확장성을 고려한 이벤트 Id
        public int RequiredTapCount;    // 연속 입력이 감지되기 위한 입력 횟수
        public float InputTerm;    // 연속 입력 감지 텀
        public float Threshold; // 입력 감지 기준 ( 손가락을 이동시키며 키보드에 스쳐 입력되거나, 스틱의 미세한 떨림으로 입력된 작은 값을 어디까지 무시할 것인가에 대한 지표 )
        public Action<string, int, int> MultiTapCallback;   // 연속 입력 감지 시, 호출되는 콜백 함수
        public int LastSign;    // 직전 입력된 축의 방향
    }

    /// <summary>
    /// 버튼 입력 기반의 입력을 처리하는 클래스
    /// </summary>
    private class ButtonRegister
    {
        public InputAction InputAction;
        public string Id;
        public int RequiredTapCount;
        public float InputTerm;
        public Action<string, int, int> MultiTapCallback;
        public Action<InputAction.CallbackContext> performedHandler;
    }

    [Header("Variables")]
    private PlayerInput playerInputActions;

    [Header("Multi Tap Controller")]
    public Action<string, int, int> MultiTapAction;
    private Dictionary<string, List<float>> tapTimes = new Dictionary<string, List<float>>();   // InputTerm 사이에 몇 번 입력되었는지를 판단하기 위한 딕셔너리 컬렉션 ( 딕셔너리 채용 이유 : 각 키별로 따로 입력 감지 처리가 필요하기 때문 )
    private List<AxisRegister> axisRegisters = new List<AxisRegister>();    // 연속 축 입력과 콜백을 연결한 이벤트 리스트들을 모아놓은 컬렉션
    private List<ButtonRegister> buttonRegisters = new List<ButtonRegister>();  // 연속 버튼 입력과 콜백을 연결한 이벤트 리스트들을 모아놓은 컬렉션

    private void Awake()
    {
        playerInputActions = GameManager.Instance.GetManager<InputManager>(typeof(InputManager)).PlayerInputActions;
    }

    void Start()
    {

    }

    void Update()
    {
        // 축 입력에 대한 연속 입력 감지 처리
        foreach (AxisRegister register in axisRegisters)
        {
            float inputValue = ReadActionAsFloat(register.InputAction);
            int sign = 0;

            if (Mathf.Abs(inputValue) >= register.Threshold)    // 일정 값 이상( 플레이어의 실수가 아닌 수준 )의 입력이 감지될 경우
            {
                sign = (int)Mathf.Sign(inputValue); // 0의 경우, 중립( 입력되지 않음 ) / 1의 경우, 오른쪽 / -1의 경우, 왼쪽
            }

            if (register.LastSign == 0 && sign != 0)    // 중립에서 입력이 일어났을 경우, 즉, 연속 입력을 위한 첫 번째 입력이 일어났을 경우
            {
                RegisterTap($"{register.Id}:{sign}", register.RequiredTapCount, register.InputTerm, register.MultiTapCallback, sign);
            }
            else if (register.LastSign != 0 && sign != 0 && register.LastSign != sign)  // 각기 다른 방향의 연속 입력이 일어났을 경우( 커맨드 입력 등의 확장성을 고려 )
            {
                RegisterTap($"{register.Id}:{sign}", register.RequiredTapCount, register.InputTerm, register.MultiTapCallback, sign);   // to do : 특정 패턴의 입력이 일어났을 때 커맨드로 입력받도록 처리할 것
            }

            register.LastSign = sign;   // 최근 입력 이력으로 현재 입력을 저장
        }
    }

    /// <summary>
    /// 입력 이벤트가 Input System의 Button형과 Value형을 분리해 입력된 키 값을 반환하는 메소드 
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private float ReadActionAsFloat(InputAction action)
    {
        if (action == null)
            return 0f;

        try
        {
            return action.ReadValue<float>();   // Button형일 경우, 입력되었는지 아닌지의 여부를 판단하는 float값을 반환
        }
        catch
        {
            try
            {
                // Value형일 경우, 입력된 버튼 값을 Vector2 형태로 반환
                Vector2 inputVector = action.ReadValue<Vector2>();

                return inputVector.x;
            }
            catch
            {
                return 0f;
            }
        }
    }

    /// <summary>
    /// 축 입력과 콜백을 연결시키는 메소드
    /// 콜백 조건 등록 : 연속 입력 횟수, 연속 입력 유효 시간 등
    /// </summary>
    /// <param name="axisAction"></param>
    /// <param name="id"></param>
    /// <param name="multiTapCallback"></param>
    /// <param name="requiredTapCount"></param>
    /// <param name="inputTerm"></param>
    /// <param name="threshold"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RegisterAxisAction(InputAction axisAction, string id, Action<string, int, int> multiTapCallback, int requiredTapCount = 2, float inputTerm = 0.25f, float threshold = 0.5f)
    {
        if (axisAction == null)
        {
            throw new ArgumentNullException(nameof(axisAction));
        }

        // 이벤트 컬렉션에 축 입력 시의 콜백 이벤트 등록
        axisRegisters.Add(new AxisRegister
        {
            InputAction = axisAction,
            Id = id,
            RequiredTapCount = requiredTapCount,
            InputTerm = inputTerm,
            Threshold = threshold,
            MultiTapCallback = multiTapCallback,
            LastSign = 0
        });
    }

    /// <summary>
    /// 버튼 입력과 콜백을 연결시키는 메소드
    /// 콜백 조건 등록 : 연속 입력 횟수, 연속 입력 유효 시간 등
    /// </summary>
    /// <param name="buttonAction"></param>
    /// <param name="id"></param>
    /// <param name="multiTapCallback"></param>
    /// <param name="requiredTapCount"></param>
    /// <param name="inputTerm"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RegisterButtonAction(InputAction buttonAction, string id, Action<string, int, int> multiTapCallback, int requiredTapCount = 2, float inputTerm = 0.25f)
    {
        if (buttonAction == null)
        {
            throw new ArgumentNullException(nameof(buttonAction));
        }

        ButtonRegister register = new ButtonRegister
        {
            InputAction = buttonAction,
            Id = id,
            RequiredTapCount = requiredTapCount,
            InputTerm = inputTerm,
            MultiTapCallback = multiTapCallback
        };

        register.performedHandler = (ctx) =>
        {
            RegisterTap(id, register.RequiredTapCount, register.InputTerm, register.MultiTapCallback, 0);   // 버튼의 경우 방향 입력이 없으므로 기본적으로 중립값인 0을 넘겨준다
        };

        buttonAction.performed += register.performedHandler;    // 버튼이 눌러졌을 때의 콜백 이벤트를 등록( 입력값과 콜백 이벤트를 연결해 버튼 액션으로 등록 )
        buttonRegisters.Add(register);  // 이벤트 컬렉션에 등록
    }

    /// <summary>
    /// 연속 입력 감지를 위한 키 입력 사이 시간 기록 등의 처리를 하는 메소드
    /// 콜백 이벤트 충족 시 ( requiredTapCount 등의 조건 ) 이벤트 발생 유도
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requiredTapCount"></param>
    /// <param name="inputTerm"></param>
    /// <param name="callback"></param>
    /// <param name="direction"></param>
    private void RegisterTap(string id, int requiredTapCount, float inputTerm, Action<string, int, int> callback, int direction)
    {
        if (!tapTimes.TryGetValue(id, out List<float> list))    // 특정 이벤트의 트리거 키 연속 입력이 기록되지 않고 있을 경우
        {
            list = new List<float>();   // 입력 시간 리스트 초기화

            tapTimes[id] = list;    // 입력 시간을 기록할 딕셔너리 컬렉션에 등록
        }

        float nowTime = Time.time;  // 입력이 일어난 시간

        list.Add(nowTime);  // 입력이 일어난 시간 기록
        list.RemoveAll(t => nowTime - t > inputTerm);   // 직전에 입력된 키가 연속 입력 유예 기간보다 길 경우, 입력 초기화

        if (list.Count >= requiredTapCount) // 콜백 이벤트 발생을 위한 연속 입력 횟수를 충족했을 경우
        {
            callback?.Invoke(id, list.Count, direction);    // 트리거 키에 연결된 콜백 이벤트 실행

            list.Clear();   // 다음 이벤트 처리를 위한 리스트 초기화
        }
    }

    private void OnDestroy()
    {
        foreach (ButtonRegister register in buttonRegisters)
        {
            if (register?.InputAction != null && register.performedHandler != null)
                register.InputAction.performed -= register.performedHandler;
        }
    }

    public void ClearHistory()
    {
        tapTimes.Clear();

        foreach (AxisRegister register in axisRegisters) register.LastSign = 0;
    }
}
