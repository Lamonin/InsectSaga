using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [HideInInspector] [SerializeField] private bool isSaved; //Checkpoint save state only once

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isSaved) return;
        if (other.CompareTag("Player"))
        {
            isSaved = true;
            Debug.Log("Задействовал чекпоинт!");
            if (LevelManager.Handler != null)
                LevelManager.Handler.SaveData();
        }
    }
}
