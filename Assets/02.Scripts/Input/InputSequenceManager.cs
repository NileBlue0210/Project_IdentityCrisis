using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 연속 키 입력 등을 감지 및 처리하는 클래스
/// </summary>
public class InputSequenceManager : MonoBehaviour
{
    private class AxisRegister
    {
        public InputAction InputAction;
        public string Id;
        public int RequiredTapCount;
        public float Window;
        public float Threshold;
        public Action<string, int, int> MultiTapCallback;
        public int LastSign;
    }

    private class ButtonRegister
    {
        public InputAction InputAction;
        public string Id;
        public int RequiredTapCount;
        public float Window;
        public Action<string, int, int> MultiTapCallback;
        public Action<InputAction.CallbackContext> performedHandler;
    }

    [Header("Variables")]
    private PlayerInput playerInputActions;

    [Header("Multi Tap Controller")]
    public Action<string, int, int> MultiTapAction;
    private Dictionary<string, List<float>> tapTimes = new Dictionary<string, List<float>>();
    private List<AxisRegister> axisRegisters = new List<AxisRegister>();
    private List<ButtonRegister> buttonRegisters = new List<ButtonRegister>();

    private void Awake()
    {
        playerInputActions = GameManager.Instance.GetManager<InputManager>(typeof(InputManager)).PlayerInputActions;
    }

    void Start()
    {

    }

    void Update()
    {
        foreach (AxisRegister register in axisRegisters)
        {
            float val = ReadActionAsFloat(register.InputAction);
            int sign = 0;

            if (Mathf.Abs(val) >= register.Threshold) sign = (int)Mathf.Sign(val);

            if (register.LastSign == 0 && sign != 0)
            {
                RegisterTap($"{register.Id}:{sign}", register.RequiredTapCount, register.Window, register.MultiTapCallback, sign);
            }
            else if (register.LastSign != 0 && sign != 0 && register.LastSign != sign)
            {
                RegisterTap($"{register.Id}:{sign}", register.RequiredTapCount, register.Window, register.MultiTapCallback, sign);
            }

            register.LastSign = sign;
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

    public void RegisterAxisAction(InputAction axisAction, string id, Action<string, int, int> multiTapCallback, int requiredTapCount = 2, float window = 0.25f, float threshold = 0.5f)
    {
        if (axisAction == null)
        {
            throw new ArgumentNullException(nameof(axisAction));
        }

        axisRegisters.Add(new AxisRegister
        {
            InputAction = axisAction,
            Id = id,
            RequiredTapCount = requiredTapCount,
            Window = window,
            Threshold = threshold,
            MultiTapCallback = multiTapCallback,
            LastSign = 0
        });
    }

    public void RegisterButtonAction(InputAction buttonAction, string id, Action<string, int, int> multiTapCallback, int requiredTapCount = 2, float window = 0.25f)
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
            Window = window,
            MultiTapCallback = multiTapCallback
        };

        register.performedHandler = (ctx) =>
        {
            RegisterTap(id, register.RequiredTapCount, register.Window, register.MultiTapCallback, 0);
        };

        buttonAction.performed += register.performedHandler;
        buttonRegisters.Add(register);
    }

    private void RegisterTap(string id, int requiredTapCount, float window, Action<string, int, int> callback, int direction)
    {
        if (!tapTimes.TryGetValue(id, out var list))
        {
            list = new List<float>();

            tapTimes[id] = list;
        }

        float nowTime = Time.time;

        list.Add(nowTime);
        list.RemoveAll(t => nowTime - t > window);

        if (list.Count >= requiredTapCount)
        {
            callback?.Invoke(id, list.Count, direction);

            list.Clear();
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
