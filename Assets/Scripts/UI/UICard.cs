using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    protected TextMeshProUGUI _nameText;
    [SerializeField]
    protected Image _art;
    [SerializeField]
    protected bool _selected;
    public void Select()
    {
        _selected = true;
        UIHand.CurrentHand.SelectedCard?.Deselect();
        UIHand.CurrentHand.SetSelectedCard(this);
        OnSelected();
    }
    public void Deselect()
    {
        _selected = false;
        UIHand.CurrentHand.SetSelectedCard(null);
        OnDeselected();
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (_selected)
        {

            Deselect();
            Debug.Log("Card been deselected" + this);
        }
        else
        {

            Select();
            Debug.Log("Card been selected" + this);
        }

    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Card been hovered" + this);
    }
    public virtual void Use(int x, int y)
    {
        Deselect();
        Destroy(gameObject);
    }
    protected virtual void OnSelected()
    {

    }
    protected virtual void OnDeselected()
    {

    }
}
