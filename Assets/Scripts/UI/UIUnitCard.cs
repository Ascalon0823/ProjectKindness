using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class UIUnitCard : UICard
{
    [SerializeField]
    Unit _loadedUnit;

    [SerializeField]
    TextMeshProUGUI _deployCostText;
    [SerializeField]
    TextMeshProUGUI _powerText;
    [SerializeField]
    TextMeshProUGUI _lifeText;
    public Unit LoadedUnit => _loadedUnit;
    public void SetupAs(Unit unit)
    {
        _loadedUnit = unit;
        _deployCostText.text = _loadedUnit.DeployCost.ToString();
        _nameText.text = _loadedUnit.Name;
        _selected = false;
        _powerText.text = _loadedUnit.Power.ToString();
        _lifeText.text = _loadedUnit.Life.ToString();
    }
    public override void Use(int x, int y)
    {
        var newunit = (BattleController.CurrentBattle.SelectedCard as UIUnitCard).LoadedUnit;
        if (BattleController.CurrentBattle.PlayerHero.CurrentCommandPoint < newunit.DeployCost
        || x > LoadedUnit.Speed) return;
        BattleController.CurrentBattle.DeployUnit(newunit, x, y, true);
        base.Use(x, y);
    }
    protected override void OnSelected()
    {
        BattleFieldController.ActiveBattleField.DrawBoxRange(Vector2Int.zero, new Vector2Int(LoadedUnit.Speed, BattleFieldController.ActiveBattleField.Height - 1));
    }
    protected override void OnDeselected()
    {
        BattleFieldController.ActiveBattleField.ClearCurrentRangeTiles();
    }
}
