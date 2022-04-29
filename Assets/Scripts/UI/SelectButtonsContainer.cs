using UnityEngine;

public class SelectButtonsContainer : MonoBehaviour
{
    [SerializeField] private SelectButtonBase[] buttons;
    public static SelectButtonsContainer Container;

    private GUIActions _input;
    private int _selectedIndex = 0;
    
    public void Awake()
    {
        Container ??= this;

        for (int i = 0; i < buttons.Length; i++)
            buttons[i].index = i;

        _input = new GUIActions();
        _input.UI.Navigate.performed += context =>
        {
            var v2 = context.ReadValue<Vector2>();
            if (v2.y != 0)
                SelectButton(_selectedIndex + (v2.y > 0 ? -1 : 1));
        };

        _input.UI.Accept.performed += _ => 
        {
            buttons[_selectedIndex].UseButton();
        };

        
        buttons[_selectedIndex].Select();
    }

    public void SelectButton(int index)
    {
        buttons[_selectedIndex].Deselect();
        _selectedIndex = index;
        
        if (_selectedIndex < 0) _selectedIndex = buttons.Length - 1;
        else if (_selectedIndex >= buttons.Length) _selectedIndex = 0;
        
        buttons[_selectedIndex].Select();
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void OnDestroy()
    {
        Container = null;
    }
}
