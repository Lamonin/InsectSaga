using UnityEngine;
using UnityEngine.Events;

public class SelectButtonBase:MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;

    private int _index;

    public int index { get => _index; set => _index = value; }

    protected virtual void Start()
    {
        
    }

    private void OnMouseDown()
    {
        UseButton();
    }

    private void OnMouseOver()
    {
        SelectButtonsContainer.Container.SelectButton(_index);
    }

    public virtual void UseButton()
    {
        onClick?.Invoke();
    }

    public virtual void SelectGUI(){}

    public virtual void Select(){}
    public virtual void Deselect(){}
}