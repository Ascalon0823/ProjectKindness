using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
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
    List<UnitController> _deployedUnits = new List<UnitController>();

    public UICard SelectedCard => UIHand.CurrentHand.SelectedCard;
    public UnitController SelectedUnit => _selectedUnit;
    [SerializeField]
    UnitController _selectedUnit;
    [SerializeField]
    int _numOfEnemy;
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
                        UseCost = 1,
                        ActionCost = 1,
                        Range = false,
                        Power = 1,
                        Life = 3,
                        Armor = 0,
                        Speed = 10,
                    }
                },
                {
                    new Unit(){
                        Name = "Archer",
                        UseCost = 1,
                        ActionCost = 1,
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
                        UseCost = 2,
                        ActionCost = 1,
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
        DeployUnit(new Unit()
        {
            Name = "OrcWarrior",
            UseCost = 1,
            ActionCost = 2,
            Range = false,
            Power = 2,
            Life = 2,
            Armor = 0,
            Speed = 4,
        }, 3, 4, false);
        //Stub
        Observable.EveryUpdate().Select(x => GetEnemyUnits().Count)
        .DistinctUntilChanged()
        .Subscribe(x => _numOfEnemy = x)
        .AddTo(this);
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
            Vector3.zero, Quaternion.identity);
        unitController.SetupAs(unit);
        if (friendly)
            _playerHero.SpendCommandPoint(unit.UseCost);
        BattleFieldController.ActiveBattleField.AddUnitToBattleField(unitController, x, y);
        unitController.Friendly = friendly;
        _deployedUnits.Add(unitController);
        unitController.OnDeployed();
        return unitController;
    }
    public List<UnitController> GetEnemyUnits(bool includeDead = false)
    {
        return _deployedUnits.Where(x => !x.Friendly && (!x.IsDead || includeDead)).ToList();
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
            foreach (var unit in _deployedUnits)
            {
                if (unit.Friendly == _playerTurn)
                {
                    unit.OnNewTurn();
                }
            }
        }
    }
    public void SetSelectController(UnitController unitc)
    {
        _selectedUnit = unitc;
    }

}
