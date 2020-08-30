using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
public class UIUnit : MonoBehaviour
{
    [SerializeField]
    UnitController _loadedUnit;
    [SerializeField]
    TextMeshPro _remainingLifeText;
    public void SetupAs(UnitController uc)
    {
        _loadedUnit = uc;
        Observable.EveryUpdate().Select(x => _loadedUnit.LoadedUnit.Life)
        .DistinctUntilChanged().Subscribe(x => _remainingLifeText.text = x.ToString())
        .AddTo(this);
    }
}
