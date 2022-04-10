using System;
using UnityEngine;

public class UsableObject : MonoBehaviour
{
    private static InsectPlayer _player;
    public Sprite useIcon;
    public Vector2 offsetUseIcon;

    private GameObject useGO;

    private void CreateUseIconObject()
    {
        useGO = new GameObject("useIconSprite");
        useGO.AddComponent<SpriteRenderer>().sprite = useIcon;
        useGO.transform.SetParent(transform);
        useGO.transform.position = transform.position+(Vector3) offsetUseIcon;
        useGO.SetActive(false);
    }
    
    private void Awake()
    {
        _player ??= FindObjectOfType<InsectPlayer>(true);
        CreateUseIconObject();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    protected void Interact()
    {
        Debug.Log("Interacted");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position+(Vector3) offsetUseIcon, 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && _player.state == PlatformerCharacter.CharacterStates.Normal)
        {
            _player.interactAction = Interact;
            useGO.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (_player.interactAction == Interact)
            {
                _player.interactAction = null;
            }
            useGO.SetActive(false);
        }
    }
}
