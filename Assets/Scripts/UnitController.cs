using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
public class UnitController : MonoBehaviour
{
    bool _selected;
    [SerializeField]
    Animator _charAnimator;
    public Unit LoadedUnit => _unit;
    [SerializeField]
    Unit _unit;
    public bool Ready;
    public bool InAction => _inAction;
    [SerializeField]
    bool _inAction;
    public bool Friendly;
    public Vector2Int Pos;
    public Vector2Int TurnBeginPos => _turnBeginPos;
    Vector2Int _turnBeginPos;
    [SerializeField]
    bool _canRetaliate;
    public bool IsDead => _dead;
    [SerializeField]
    bool _dead;

    [SerializeField]
    Transform _rotator;
    [SerializeField]
    Transform _scaler;


    public void SetupAs(Unit unit)
    {
        _unit = unit;
        if (GetComponentInChildren<UIUnit>() != null)
        {
            GetComponentInChildren<UIUnit>().SetupAs(this);
        }
        Observable.EveryUpdate().Select(x => CanAct() && !_dead).DistinctUntilChanged()
        .Subscribe(x =>
        {
            foreach (var spriterenderer in _charAnimator.GetComponentsInChildren<SpriteRenderer>())
            {
                spriterenderer.color = x ? Color.white : Color.gray;
            }
        }).AddTo(this);
        Observable.EveryUpdate().Select(x => _dead).DistinctUntilChanged()
        .Subscribe(x =>
        {
            _rotator.rotation = Quaternion.Euler(0, 0, x ? -90 : 0);
            _charAnimator.enabled = !x;
        }).AddTo(this);
        Observable.EveryUpdate().Select(x => Friendly).DistinctUntilChanged()
       .Subscribe(x =>
       {
           _scaler.transform.localScale = new Vector3(x ? 1 : -1, 1, 1);//Hard coded
       }).AddTo(this);
    }
    void Refresh()
    {
        if (!_dead)
        {
            Ready = true;
            _inAction = false;
            _canRetaliate = true;
        }
        _turnBeginPos = Pos;
    }
    public void OnDeployed()
    {
        Refresh();
    }
    public void OnNewTurn()
    {
        Refresh();
    }
    public bool CanAct()
    {
        return LoadedUnit.ActionCost <= BattleController.CurrentBattle.PlayerHero.CurrentCommandPoint && Ready;
    }
    void OnSelected()
    {
        _selected = true;
        if (CanAct())
        {
            BeginAction();
        }

    }
    public void BeginAction()
    {
        _inAction = true;
        BattleFieldController.ActiveBattleField.DrawSphereRange(Pos, LoadedUnit.Speed);
    }
    public void CancelAction()
    {
        BattleFieldController.ActiveBattleField.UpdateUnitPos(this, _turnBeginPos.x, _turnBeginPos.y);
        _inAction = false;
    }
    public void CompleteAction()
    {
        _inAction = false;
        BattleController.CurrentBattle.PlayerHero.SpendCommandPoint(LoadedUnit.ActionCost);
        Ready = false;
        _turnBeginPos = Pos;
        if (_selected)
        {
            Deselect();
        }
    }
    void OnDeselected()
    {
        _selected = false;
        BattleFieldController.ActiveBattleField.ClearCurrentRangeTiles();
        if (_inAction)
        {
            CancelAction();
        }
    }
    public void SetPos(int x, int y)
    {
        Pos = new Vector2Int(x, y);
        transform.position = BattleFieldController.ActiveBattleField.GetCellWorldPos(x, y);
    }
    public bool CanMoveTo(int x, int y)
    {
        return (x != Pos.x || y != Pos.y)
            && (Mathf.Abs(x - _turnBeginPos.x) + Mathf.Abs(y - _turnBeginPos.y)) <= LoadedUnit.Speed
            && BattleFieldController.ActiveBattleField.GetUnitControllerFromCoord(new Vector2Int(x, y)) == null;
    }
    public bool HasAnyTargetInRange()
    {
        return BattleController.CurrentBattle.GetEnemyUnits().Any(x => !x.IsDead && (CanAttack(x)));
    }
    public bool CanAttack(UnitController target)
    {
        if (target.Friendly == Friendly) return false;
        if (_unit.Range)
        {
            return true;
        }
        return Mathf.Abs(Pos.x - target.Pos.x) <= 1 && Mathf.Abs(Pos.y - target.Pos.y) <= 1;
    }
    public void Attack(UnitController target)
    {
        var damage = _unit.Power;
        var range = false;
        if (_unit.Range && (target.Pos - Pos).magnitude > 8)
        {
            damage = damage / 2;
            range = true;
        }
        Debug.Log($"[{_unit.Name}] attacks [{target.LoadedUnit.Name}] for {damage}");
        target.TakeDamage(this, damage, range);
    }
    public void TakeDamage(UnitController from, int damage, bool range = false, bool causeRetaliate = true)
    {
        _unit.Life -= damage;
        Debug.Log($"[{_unit.Name}] receive  {damage} from [{from.LoadedUnit.Name}]");
        if (_unit.Life <= 0)
        {
            _unit.Life = 0;
            Killed(from);
        }
        if (!_dead && _canRetaliate && !range && causeRetaliate)
        {
            Retailiate(from);
        }

    }
    public void Retailiate(UnitController target)
    {
        var damage = _unit.Power / 2;
        if (_unit.Range)
        {
            damage = damage / 2;
        }
        _canRetaliate = false;
        Debug.Log($"[{_unit.Name}] retaliated {damage} to [{target.LoadedUnit.Name}]");
        target.TakeDamage(this, damage, false, false);
    }
    public void Killed(UnitController by)
    {
        Debug.Log($"[{_unit.Name}] killed by [{by.LoadedUnit.Name}]");
        _dead = true;
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
