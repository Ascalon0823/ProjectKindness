using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Unit : Card
{
    public int Power;
    public int Life;
    public int Speed;
    public bool Range;
    public int Armor;
    public int ActionCost;
    public List<Action> Actions;
    public abstract class Action
    {
        public int Cost;
        public abstract bool Perform();
    }
    public class AttackAction : Action
    {
        Unit _offencer;
        Vector2Int _offencerPos;
        Unit _defender;
        Vector2Int _defenderPos;
        public override bool Perform()
        {
            _defender.Life -= _offencer.Power;
            return false;
        }
    }
}
public static class UnitTemplates
{
    
}
