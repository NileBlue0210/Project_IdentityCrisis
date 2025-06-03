using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾�� ĳ���Ͱ� ��Ӱ����� �÷��̾� �ñ״�ó Ŭ����
/// </summary>
public class Player : MonoBehaviour
{
    private PlayerController _pController;

    private void Awake()
    {
        _pController = GetComponent<PlayerController>();

        UnitManager.Instance.Player = this;
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void LateUpdate()
    {

    }

    protected virtual void Move()
    {

    }

    protected virtual void Jump()
    {

    }
}
