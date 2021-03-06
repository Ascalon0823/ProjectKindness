﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class UIHand : MonoBehaviour
{
    public static UIHand CurrentHand;
    [SerializeField]
    UIUnitCard _uiUnitCardPrefab;
    [SerializeField]
    RectTransform _handCardsHolder;
    [SerializeField]
    UICard _selectedCard;
    public UICard SelectedCard => _selectedCard;
    void Awake()
    {
        CurrentHand = this;
    }
    public void AddCardIntoHand(Card card)
    {
        var uicard = SpawnUIForCard(card);
        uicard.transform.SetParent(_handCardsHolder, false);
        uicard.transform.SetAsLastSibling();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_handCardsHolder);
    }
    public UICard SpawnUIForCard(Card card)
    {
        if (card is Unit)
        {
            var uiunitcard = Instantiate(_uiUnitCardPrefab, Vector3.zero, Quaternion.identity);
            uiunitcard.SetupAs(card as Unit);
            return uiunitcard;
        }
        return null;
    }
    public void SetSelectedCard(UICard card)
    {
        _selectedCard = card;
    }
}
