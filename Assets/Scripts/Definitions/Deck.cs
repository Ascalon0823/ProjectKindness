using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[System.Serializable]
public class Deck
{
    public List<Card> CurrentCards => _currentCards;
    [SerializeField]
    List<Card> _currentCards;
    Stack<Card> _cardStack;
    private static System.Random rng = new System.Random();
    public Deck()
    {
        _currentCards = new List<Card>();
    }
    public Deck(List<Card> cards)
    {
        _currentCards = cards;
    }
    public void AddCardToDeck(Card card)
    {
        CurrentCards.Add(card);
    }
    public void AddCardToStack(Card card)
    {
        _cardStack.Push(card);
        ShuffleStack();
    }

    public void ShuffleStack()
    {
        var temp = _cardStack.ToList();
        int n = temp.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var t = temp[k];
            temp[k] = temp[n];
            temp[n] = t;
        }
        _cardStack.Clear();
        foreach (var c in temp)
        {
            _cardStack.Push(c);
        }
    }
    public void Setup()
    {
        _cardStack = new Stack<Card>();
        foreach (var card in _currentCards)
        {
            _cardStack.Push(card);
        }
        ShuffleStack();
    }
    public Card Draw()
    {
        return _cardStack.Pop();
    }
}
