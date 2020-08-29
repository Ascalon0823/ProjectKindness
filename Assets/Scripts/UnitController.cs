using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnitController : MonoBehaviour
{
    bool _selected;
    [SerializeField]
    Animator _charAnimator;
    [SerializeField]
    Unit _unit;
    public bool Ready;
    public bool Friendly;
    public void SetupAs(Unit unit)
    {
        _unit = unit;
    }

    public void OnSelected()
    {
        if (Ready)
        {

        }
    }
}
