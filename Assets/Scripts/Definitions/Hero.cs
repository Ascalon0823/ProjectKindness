using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hero
{
    [System.Serializable]
    public class HeroUnit : Unit
    {
        public int HeroicPoint;
        public int CommandPoint;

    }
    public Deck CurrentDeck => _deck;
    [SerializeField]
    Deck _deck;
    [SerializeField]
    HeroUnit _loadedHeroUnit;
    public int CurrentHeroicPoint => _currentHeroicPoint;
    public int CurrentCommandPoint => _currentCommandPoint;
    [SerializeField]
    int _currentHeroicPoint;
    [SerializeField]
    int _currentCommandPoint;
    public Hero(Deck deck, HeroUnit heroUnit)
    {
        _deck = deck;
        _loadedHeroUnit = heroUnit;
        _deck.Setup();
        RefreshCommandPoint();
        RefreshHeroicPoint();
    }
    public void RefreshCommandPoint()
    {
        _currentCommandPoint = _loadedHeroUnit.CommandPoint;
    }
    public void RefreshHeroicPoint()
    {
        _currentHeroicPoint = _loadedHeroUnit.HeroicPoint;
    }
    public void SpendCommandPoint(int cost)
    {
        _currentCommandPoint -= cost;
    }
}
