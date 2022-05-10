using UnityEngine;
using DG.Tweening;

public class TrashPlatformMovement : MonoBehaviour {
    public Transform pos1;
    public Transform pos2;
    public float duration;

    Rigidbody2D rb2d;


    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        transform.position = pos1.position;
        transform.DOMove(pos2.position, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.parent != transform)
                other.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}