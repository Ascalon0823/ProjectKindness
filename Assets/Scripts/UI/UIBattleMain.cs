using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class UIBattleMain : MonoBehaviour
{
    [SerializeField]
    CanvasGroup _mainGroup;
    // Start is called before the first frame update
    void Start()
    {
        // Observable.EveryUpdate().Select(x => BattleController.CurrentBattle.SelectedUnit == null && BattleController.CurrentBattle.SelectedCard == null)
        //     .DistinctUntilChanged()
        //     .Subscribe(x =>
        //     {
        //         LeanTween.alphaCanvas(_mainGroup, x ? 1f : 0.333f, 0.25f).setEaseInOutExpo();
        //         _mainGroup.interactable = x;
        //         _mainGroup.blocksRaycasts = x;
        //     }).AddTo(this);
    }
}
