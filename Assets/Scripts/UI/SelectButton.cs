using UnityEngine;
using DG.Tweening;

public class SelectButton : SelectButtonBase
{
    [SerializeField] private SpriteRenderer selectedSprite;

    protected override void Start()
    {
        selectedSprite.color *= new Vector4(1, 1, 1, 0); //Set alpha to zero
    }

    public override void Select()
    {
        selectedSprite.gameObject.SetActive(true);
        selectedSprite.DOKill();
        selectedSprite.DOFade(1, 0.2f);
    }

    public override void Deselect()
    {
        selectedSprite.DOKill();
        selectedSprite.DOFade(0, 0.2f).OnComplete(() => { selectedSprite.gameObject.SetActive(false); });
    }
}
