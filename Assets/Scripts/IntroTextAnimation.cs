using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroTextAnimation : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    
    [SerializeField] private TextMeshPro textMesh;

    private void Awake()
    {
        transform.position = startPosition;
    }

    void Start()
    {
        transform.DOMove(endPosition, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            SceneManager.LoadScene("Prologue");
        });
        textMesh.DOFade(0, duration*0.1f).SetDelay(duration * 0.9f);
    }
}
