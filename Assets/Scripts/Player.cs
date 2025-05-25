using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerController _pController;

    private void Awake()
    {
        _pController = GetComponent<PlayerController>();

        UnitManager.Instance.Player = this;
    }
}
