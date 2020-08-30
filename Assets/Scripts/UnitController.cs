using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class UnitController : MonoBehaviour
{
    bool _selected;
    [SerializeField]
    Animator _charAnimator;
    public Unit LoadedUnit => _unit;
    [SerializeField]
    Unit _unit;
    public bool Ready;
    public bool Friendly;
    public Vector2Int Pos;



    public void SetupAs(Unit unit)
    {
        _unit = unit;
        if (GetComponentInChildren<UIUnit>() != null)
        {
            GetComponentInChildren<UIUnit>().SetupAs(this);
        }
        Observable.EveryUpdate().Select(x => Ready).DistinctUntilChanged()
        .Subscribe(x =>
        {
            foreach (var spriterenderer in _charAnimator.GetComponentsInChildren<SpriteRenderer>())
            {
                spriterenderer.color = x ? Color.white : Color.gray;
            }
        }).AddTo(this);
    }

    void OnSelected()
    {
        _selected = true;
        BattleFieldController.ActiveBattleField.DrawSphereRange(Pos, LoadedUnit.Speed);
    }
    void OnDeselected()
    {
        _selected = false;
        BattleFieldController.ActiveBattleField.ClearCurrentRangeTiles();
    }
    public void SetPos(int x, int y)
    {
        Pos = new Vector2Int(x, y);
        transform.position = BattleFieldController.ActiveBattleField.GetCellWorldPos(x, y);
    }
    public bool CanMoveTo(int x, int y)
    {
        return (x != Pos.x || y != Pos.y) && (Mathf.Abs(x - Pos.x) + Mathf.Abs(y - Pos.y)) <= LoadedUnit.Speed;
    }
    public void Select()
    {
        if (BattleController.CurrentBattle.SelectedUnit != null)
        {
            BattleController.CurrentBattle.SelectedUnit.Deselect();
        }
        BattleController.CurrentBattle.SetSelectController(this);
        OnSelected();
    }
    public void Deselect()
    {
        BattleController.CurrentBattle.SetSelectController(null);
        OnDeselected();
    }
}
