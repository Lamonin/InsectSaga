using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService.Scene;

public class InsectPlayer : PlatformerCharacter
{
    public delegate void Interaction();
    public Interaction interactAction = null;
    
    private InputScheme _input;
    [SerializeField] private Vector2 inpDir;

    private bool _tl, _bl, _ll, _rl;

    private void Awake()
    {
        _input = new InputScheme();
        
        _input.Player.Jump.performed += context => { Jump(); };
        _input.Player.RunModeOn.performed += context => { TryToRun = true; };
        _input.Player.RunModeOff.performed += context => { ToNormalState(); };
        _input.Player.Using.performed += context => { if (interactAction != null && state != CharacterStates.Crawl) interactAction(); };
    }
    
    private void OnEnable() { _input.Enable(); }
    
    private void OnDisable() { _input.Disable(); }

    protected override void Start()
    {
        base.Start();
    }

    private bool CastRay(Vector2 dir, float distance)
    {
        return Physics2D.Raycast(transform.position, dir, distance, groundLayer);
    }

    protected override void Update()
    {
        Vector2 inputDir = _input.Player.Movement.ReadValue<Vector2>();
        
        Debug.DrawRay(transform.position, new Vector2(1,0)*0.5f, Color.green);
        Debug.DrawRay(transform.position, new Vector2(0,-1)*0.5f, Color.green);

        inpDir = inputDir;
        
        MoveDir = CurrentSide switch
        {
            GroundSide.Floor => inputDir.x,
            GroundSide.Ceil => -inputDir.x,
            GroundSide.LWall => -inputDir.y,
            GroundSide.RWall => inputDir.y,
            _ => MoveDir
        };
        
        _rl = CastRay(Vector2.right, 0.5f);
        _ll = CastRay(Vector2.left, 0.5f);
        _tl = CastRay(Vector2.up, 0.5f);
        _bl = CastRay(Vector2.down, 0.5f);

        switch (CurrentSide)
        {
            case GroundSide.Floor:
                if (_rl && inputDir.x > 0 && inputDir.y < 0)
                    MoveDir = 0;
                else if (_ll && inputDir.x < 0 && inputDir.y < 0)
                    MoveDir = 0;
                break;
            
            case GroundSide.Ceil:
                if (_rl && inputDir.x > 0 && inputDir.y > 0)
                    MoveDir = 0;
                else if (_ll && inputDir.x < 0 && inputDir.y > 0)
                    MoveDir = 0;
                break;
            
            case GroundSide.LWall:
                if (_bl && inputDir.x < 0 && inputDir.y < 0)
                    MoveDir = 0;
                else if (_tl && inputDir.x < 0 && inputDir.y > 0)
                    MoveDir = 0;
                break;
            
            case GroundSide.RWall:
                if (_bl && inputDir.x > 0 && inputDir.y < 0)
                    MoveDir = 0;
                else if (_tl && inputDir.x > 0 && inputDir.y > 0)
                    MoveDir = 0;
                break;
        }

        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            Debug.Log("COCKROACH IS DEAD INSIDE ((((((((");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
