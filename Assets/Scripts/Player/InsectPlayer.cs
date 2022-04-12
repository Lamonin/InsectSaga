using UnityEngine;
using UnityEngine.SceneManagement;

public class InsectPlayer : PlatformerCharacter
{
    public delegate void Interaction();
    public Interaction interactAction = null;
    
    private InputScheme _input;

    private bool _ceilRay, _floorRay, _leftWallRay, _rightWallRay;

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

        #region CorrectPlayerInputDirection

        MoveDir = ChSide switch
        {
            GroundSide.Floor => inputDir.x,
            GroundSide.Ceil => -inputDir.x,
            GroundSide.LWall => -inputDir.y,
            GroundSide.RWall => inputDir.y,
            _ => MoveDir
        };

        float rot = transform.rotation.eulerAngles.z;

        if (rot > 5 && rot < 85 && inputDir.x < 0 && inputDir.y > 0)
            MoveDir = 0;
        else if (rot > 95 && rot < 175 && inputDir.x < 0 && inputDir.y < 0)
            MoveDir = 0;
        else if (rot > 185 && rot < 265 && inputDir.x > 0 && inputDir.y < 0)
            MoveDir = 0;
        else if (rot > 275 && rot < 355 && inputDir.x > 0 && inputDir.y > 0)
            MoveDir = 0;

        _rightWallRay = CastRay(Vector2.right, 0.5f);
        _leftWallRay = CastRay(Vector2.left, 0.5f);
        _ceilRay = CastRay(Vector2.up, 0.5f);
        _floorRay = CastRay(Vector2.down, 0.5f);

        switch (ChSide)
        {
            case GroundSide.Floor:
                if (_rightWallRay && inputDir.x > 0 && inputDir.y < 0 || _leftWallRay && inputDir.x < 0 && inputDir.y < 0)
                    MoveDir = 0;
                break;
            
            case GroundSide.Ceil:
                if (_rightWallRay && inputDir.x > 0 && inputDir.y > 0 || _leftWallRay && inputDir.x < 0 && inputDir.y > 0)
                    MoveDir = 0;
                break;
            
            case GroundSide.LWall:
                if (_floorRay && inputDir.x < 0 && inputDir.y < 0 || _ceilRay && inputDir.x < 0 && inputDir.y > 0)
                    MoveDir = 0;
                break;
            
            case GroundSide.RWall:
                if (_floorRay && inputDir.x > 0 && inputDir.y < 0 || _ceilRay && inputDir.x > 0 && inputDir.y > 0)
                    MoveDir = 0;
                break;
        }
        
        #endregion

        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Enemy")) return;
        
        Debug.Log("COCKROACH IS DEAD INSIDE ((((((((");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
