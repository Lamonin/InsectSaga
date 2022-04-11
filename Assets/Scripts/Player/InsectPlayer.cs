using UnityEngine;

public class InsectPlayer : PlatformerCharacter
{
    [SerializeField] protected PlayerPosition plPos;
    public delegate void Interaction();
    public Interaction interactAction = null;
    
    private InputScheme _input;
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

    protected override void Update()
    {
        plPos = GetPlayerDirection();
        Vector2 inputDir = _input.Player.Movement.ReadValue<Vector2>();
        if (Mathf.Abs(inputDir.x) > Mathf.Abs(inputDir.y))
            inputDir.y = 0;
        else 
            inputDir.x = 0;

        MoveDir = plPos switch
        {
            PlayerPosition.Floor => inputDir.x,
            PlayerPosition.Ceil => -inputDir.x,
            PlayerPosition.LWall => -inputDir.y,
            PlayerPosition.RWall => inputDir.y,
            _ => MoveDir
        };

        base.Update();
    }
}
