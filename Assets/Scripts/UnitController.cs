using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private bool _selected;
    [SerializeField]
    Animator _charAnimator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _charAnimator.SetFloat("xSpeed", 1f);
    }
    public void OnSelected()
    {

    }
    public void MoveToCell(int x, int y)
    {

    }
}
