using System;
using System.Collections.Generic;
using Misc;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameConsole : MonoBehaviour
{
    public static InGameConsole Console;

    [SerializeField] private GameObject consoleMain;

    [SerializeField] private CommandList commandList;
    [SerializeField] private TMP_InputField commandField;
    [SerializeField] private TextMeshProUGUI resultField;

    [HideInInspector] public List<string> lastCommands;
    [HideInInspector] public bool cheatMode;
    private bool _consoleState;
    
    
    void Awake()
    {
        if (Console != null && Console != this)
        {
            lastCommands = Console.lastCommands;
            cheatMode = Console.cheatMode;
            Destroy(Console.gameObject);
            Console = null;
        }

        if (Console is null)
        {
            Console ??= this;
            DontDestroyOnLoad(gameObject);
        }

        _consoleState = consoleMain.activeSelf;
        commandList.consoleState = _consoleState;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            _consoleState = !_consoleState;
            commandList.consoleState = _consoleState;
            consoleMain.SetActive(_consoleState);
            if (!_consoleState)
            {
                commandField.DeactivateInputField();
            }
            else
            {
                commandField.ActivateInputField();
            }
        }

        if (!_consoleState) return;

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Home))
        {
            AcceptCommand();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) PrevCommand();
        if (Input.GetKeyDown(KeyCode.DownArrow)) NextCommand();
    }

    public void AcceptCommand()
    {
        if (string.IsNullOrEmpty(commandField.text)) return;
        resultField.text = commandList.ProcessConsoleInput(commandField.text);
        LayoutRebuilder.ForceRebuildLayoutImmediate(resultField.rectTransform);
        SaveLastInput(commandField.text);
    }

    private void SaveLastInput(string input)
    {
        if (lastCommands.Contains(input)) lastCommands.Remove(input);
        
        lastCommands.Add(input);
        _currentLastCommand = lastCommands.Count;
        commandField.text = string.Empty;

        if (lastCommands.Count > 10)
        {
            lastCommands.RemoveAt(0);
        }
    }

    private int _currentLastCommand;

    private void PrevCommand()
    {
        if (lastCommands.Count <= 0) return;

        _currentLastCommand--;
        if (_currentLastCommand < 0) _currentLastCommand = lastCommands.Count - 1;
        commandField.text = lastCommands[_currentLastCommand];
        commandField.ActivateInputField();
    }
    
    private void NextCommand()
    {
        if (lastCommands.Count <= 0) return;
        
        _currentLastCommand++;
        if (_currentLastCommand >= lastCommands.Count) _currentLastCommand = 0;
        commandField.text = lastCommands[_currentLastCommand];
        commandField.ActivateInputField();
    }
}
