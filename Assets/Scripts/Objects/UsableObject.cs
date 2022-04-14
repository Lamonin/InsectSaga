using UnityEngine;

public class UsableObject : MonoBehaviour
{
    public Vector2 useIconPosition;
    void Start()
    {
        useIconPosition = (Vector2) transform.position + useIconPosition;
    }

    protected virtual void OnDestroy()
    {
        
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
