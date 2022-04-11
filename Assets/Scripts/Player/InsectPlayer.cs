using UnityEngine;

public class InsectPlayer : PlatformerCharacter
{
    public delegate void Interaction();
    public Interaction interactAction = null;
    
    private InputScheme _input;
    private void Awake()
    {
        _input = new InputScheme();
        
        _input.Player.Jump.performed += context => { TryJump(); };
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
        Vector2 inputDir = _input.Player.Movement.ReadValue<Vector2>();
        /*if (Mathf.Abs(inputDir.x) > Mathf.Abs(inputDir.y))
            inputDir.y = 0;
        else if (Mathf.Abs(inputDir.x) < Mathf.Abs(inputDir.y))
            inputDir.x = 0;*/

        MoveDir = CurrentSide switch
        {
            GroundSide.Floor => inputDir.x,
            GroundSide.Ceil => -inputDir.x,
            GroundSide.LWall => -inputDir.y,
            GroundSide.RWall => inputDir.y,
            _ => MoveDir
        };

        base.Update();
    }
}
