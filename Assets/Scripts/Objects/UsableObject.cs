using Player;
using UnityEngine;

public class UsableObject : MonoBehaviour
{
    public Vector2 useIconPosition;
    public static PlayerInputHandler playerHandler;
    protected static int usableObjectsCount;
    
    void Start()
    {
        usableObjectsCount++;
        useIconPosition = (Vector2) transform.position + useIconPosition;

        if (playerHandler is null)
        {
            playerHandler = FindObjectOfType<PlayerInputHandler>(true);
        }
    }

    protected virtual void OnDestroy()
    {
        usableObjectsCount--;
        if (usableObjectsCount == 0)
            playerHandler = null;
    }

    public virtual void Interact()
    {
        Debug.Log("Было взаимодействие!");
    }

    public virtual void Deactivate()
    {
        GameUI.HideUseIcon();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2) transform.position + useIconPosition, 0.1f);
    }
}
