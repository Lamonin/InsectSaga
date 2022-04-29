using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SelectButtonUI : SelectButtonBase
{
    [SerializeField] private Image selectedImage;

    protected override void Start() {
        selectedImage.color *= new Vector4(1, 1, 1, 0); //Set alpha to zero
    }

    public override void SelectGUI()
    {
        SelectButtonsContainer.Container.SelectButton(index);
    }

    public override void Select()
    {
        selectedImage.gameObject.SetActive(true);
        selectedImage.DOKill();
        selectedImage.DOFade(1, 0.2f).SetUpdate(true);
    }

    public override void Deselect()
    {
        selectedImage.DOKill();
        selectedImage.DOFade(0, 0.2f).SetUpdate(true).OnComplete(() => { selectedImage.gameObject.SetActive(false); });
    }
}