using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
public class UIHero : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _commandPointText;
    [SerializeField]
    TextMeshProUGUI _heroicPointText;
    [SerializeField]
    Image _art;
    [SerializeField]
    bool _forPlayer;
    [SerializeField]
    Hero _currentHero;
    public static UIHero PlayerUIHero;
    // Start is called before the first frame update
    void Awake()
    {
        if (_forPlayer)
            PlayerUIHero = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetupAs(Hero hero)
    {
        _currentHero = hero;
        Observable.EveryUpdate().Select(x => hero.CurrentCommandPoint).DistinctUntilChanged()
        .Subscribe(x => _commandPointText.text = x.ToString()).AddTo(this);
        Observable.EveryUpdate().Select(x => hero.CurrentHeroicPoint).DistinctUntilChanged()
        .Subscribe(x => _heroicPointText.text = x.ToString()).AddTo(this);
    }
}
