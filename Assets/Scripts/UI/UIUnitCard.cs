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

}
