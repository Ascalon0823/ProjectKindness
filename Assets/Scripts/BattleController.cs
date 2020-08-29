using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController CurrentBattle;
    public int CurrentTurn => _currentTurn;
    [SerializeField]
    int _currentTurn;
    public bool PlayerTurn => _playerTurn;
    [SerializeField]
    bool _playerTurn;
    public Hero PlayerHero => _playerHero;
    [SerializeField]
    Hero _playerHero;
    [SerializeField]
    UnitController _unitControllerPrefab;
    List<UnitController> _deployedUnits;
    public UICard SelectedCard => UIHand.CurrentHand.SelectedCard;
    void Awake()
    {
        CurrentBattle = this;
    }
    void Start()
    {

        //Stub
        var heroUnit = new Hero.HeroUnit()
        {
            Name = "Hero",
            Power = 2,
            Life = 7,
            HeroicPoint = 5,
            CommandPoint = 7
        };
        var deck = new Deck(
            new List<Card>(){
                {
                    new Unit(){
                        Name = "Spearman",
                        DeployCost = 1,
                        Range = false,
                        Power = 1,
                        Life = 3,
                        Armor = 0,
                        Speed = 4,
                    }
                },
                {
                    new Unit(){
                        Name = "Archer",
                        DeployCost = 1,
                        Range = true,
                        Power = 1,
                        Life = 2,
                        Armor = 0,
                        Speed = 3,
                    }
                },
                {
                    new Unit(){
                        Name = "Mercenary",
                        DeployCost = 2,
                        Range = false,
                        Power = 2,
                        Life = 3,
                        Armor = 0,
                        Speed = 3,
                    }
                }
            }
        );
        _playerHero = new Hero(deck, heroUnit);
        UIHero.PlayerUIHero.SetupAs(_playerHero);
        StartBattle();
        //Stub
    }
    public void StartBattle()
    {

        _currentTurn = 0;
        _playerTurn = false;
        _deployedUnits = new List<UnitController>();
        for (int i = 0; i < 2; i++)
        {
            UIHand.CurrentHand.AddCardIntoHand(_playerHero.CurrentDeck.Draw());
        }
        NextTurn();
    }
    public UnitController DeployUnit(Unit unit, int x, int y, bool friendly, bool ready = true)
    {
        var unitController = Instantiate(_unitControllerPrefab,
            BattleFieldController.ActiveBattleField.GetCellWorldPos(x, y), Quaternion.identity);
        unitController.SetupAs(unit);
        unitController.Friendly = friendly;
        unitController.Ready = true;
        _deployedUnits.Add(unitController);
        _playerHero.SpendCommandPoint(unit.DeployCost);
        return unitController;
    }
    public void CheckBattleStatus()
    {

    }
    public void NextTurn()
    {
        _currentTurn++;
        _playerTurn = !_playerTurn;
        if (_playerTurn)
        {
            _playerHero.RefreshCommandPoint();
            UIHand.CurrentHand.AddCardIntoHand(_playerHero.CurrentDeck.Draw());
        }
    }

}
