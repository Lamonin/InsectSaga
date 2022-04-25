using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class SelectButton : MonoBehaviour
{
    [SerializeField] private SpriteRenderer selectedSprite;
    [SerializeField] private UnityEvent onClick;
    [HideInInspector] public int index;

    void Start()
    {
        selectedSprite.color *= new Vector4(1, 1, 1, 0); //Set alpha to zero
    }

    private void OnMouseDown()
    {
        UseButton();
    }

    private void OnMouseOver()
    {
        SelectButtonsContainer.Container.SelectButton(index);
    }

    public void UseButton()
    {
        onClick?.Invoke();
    }

    public void Select()
    {
        selectedSprite.gameObject.SetActive(true);
        selectedSprite.DOKill();
        selectedSprite.DOFade(1, 0.2f);
    }

    public void Deselect()
    {
        selectedSprite.DOKill();
        selectedSprite.DOFade(0, 0.2f).OnComplete(() => { selectedSprite.gameObject.SetActive(false); });
    }
}
