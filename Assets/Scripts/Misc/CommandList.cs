using System;
using System.Collections.Generic;
using InsectCharacter;
using Localize;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Misc
{
    public class CommandList : MonoBehaviour
    {
        private Dictionary<string, ConsoleCommand> _commands;
        private bool _cheatMode;
        
        [SerializeField] private TextMeshProUGUI fieldPlayerPos;
        
        [HideInInspector] public bool consoleState;
        
        private void Awake()
        {
            _commands = new Dictionary<string, ConsoleCommand>();
            LoadCommands();
        }

        private void Start()
        {
            _cheatMode = InGameConsole.Console.cheatMode;
        }

        private void Update()
        {
            if (!consoleState) return;
            
            if (InsectPlayerMediator.Mediator != null)
            {
                if (fieldPlayerPos != null)
                {
                    fieldPlayerPos.text = "Player Pos: " + InsectPlayerMediator.Mediator.transform.position;
                }
            }
        }

        private void LoadCommands()
        {
            ConsoleCommand temp;
            
            temp = new ConsoleCommand("set_scene", C_LoadScene, "Succesfully load scene!", "Error when loading!");
            _commands.Add(temp.Command, temp);
            
            temp = new ConsoleCommand("debug_command", C_DebugCommand);
            _commands.Add(temp.Command, temp);
                
            temp = new ConsoleCommand("restart_scene", C_RestartScene, "Scene restarted!");
            _commands.Add(temp.Command, temp);
            
            temp = new ConsoleCommand("cockroach_mode", C_CheatMode, "Cheats state changed!");
            _commands.Add(temp.Command, temp);
            
            temp = new ConsoleCommand("get_all_scenes", C_GetAllScenes);
            _commands.Add(temp.Command, temp);
            
            temp = new ConsoleCommand("move_to_pos", C_MovePlayerToPosition);
            _commands.Add(temp.Command, temp);
            
            temp = new ConsoleCommand("move_on_vector", C_MovePlayerOnVector);
            _commands.Add(temp.Command, temp);
            
            temp = new ConsoleCommand("set_lang", C_SetLanguage, "Successfully change language!");
            _commands.Add(temp.Command, temp);
        }

        private void C_LoadScene(string[] args)
        {
            if (!_cheatMode) return;
            if (args.Length < 2)
                throw new Exception("Not enough arguments for this command!");

            if (args.Length > 2)
                throw new Exception("A lot of arguments for this command!");
            
            
            var argType = int.TryParse(args[1], out var sceneIndex);
            if (argType)
            {
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                SceneManager.LoadScene(args[1]);
            }
        }

        private void C_DebugCommand(string[] args)
        {
            if (!_cheatMode) return;
            if (args.Length > 1)
                throw new Exception("A lot of arguments for this command!");

            Debug.Log("DEBUG COMMAND SUCCESS!");
        }

        private void C_RestartScene(string[] args)
        {
            if (!_cheatMode) return;
            if (args.Length > 1)
                throw new Exception("A lot of arguments for this command!");

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        private void C_MovePlayerToPosition(string[] args)
        {
            if (!_cheatMode) return;
            
            if (args.Length > 3)
                throw new Exception("A lot of arguments for this command!");
            if (args.Length < 3)
                throw new Exception("Not enough arguments for this command!");

            var mediator = InsectPlayerMediator.Mediator;
            if (mediator is null)
            {
                throw new Exception("Player invalid!");
            }

            if (mediator != null)
                mediator.transform.position = new Vector3(int.Parse(args[1].Trim()), int.Parse(args[2].Trim()), mediator.transform.position.z);
        }
        
        private void C_MovePlayerOnVector(string[] args)
        {
            if (!_cheatMode) return;
            
            if (args.Length > 3)
                throw new Exception("A lot of arguments for this command!");
            if (args.Length < 3)
                throw new Exception("Not enough arguments for this command!");

            var mediator = InsectPlayerMediator.Mediator;
            if (mediator is null)
            {
                throw new Exception("Player invalid!");
            }
            
            if (mediator != null)
                mediator.transform.position += new Vector3(int.Parse(args[1].Trim()), int.Parse(args[2].Trim()), 0);
        }
        
        private void C_SetLanguage(string[] args)
        {
            if (!_cheatMode) return;
            
            if (args.Length < 2)
                throw new Exception("Not enough arguments for this command!");

            if (args.Length > 2)
                throw new Exception("A lot of arguments for this command!");
            
            LocalizeManager.Manager.ChangeLanguage(int.Parse(args[1].Trim()) == 0 ? LanguageEnum.ENG : LanguageEnum.RU);
        }

        private void C_GetAllScenes(string[] args)
        {
            if (!_cheatMode) return;
            if (args.Length > 1)
                throw new Exception("A lot of arguments for this command!");

            var scenes = "[0] Main Menu\n";
            scenes += "[1] Prologue\n";
            scenes += "[2] Ghetto\n";
            scenes += "[3] IntroTextScene\n";
            scenes += "[4] Nachalo Puti\n";
            scenes += "[5] Trubi\n";
            scenes += "[6] Pobeg\n";
            scenes += "[7] Test Room\n";
            scenes += "[8] TitlesScene";

            throw new Exception(scenes);
        }

        private void C_CheatMode(string[] args)
        {
            if (args.Length < 2)
                throw new Exception("Not enough arguments for this command!");

            if (args.Length > 2)
                throw new Exception("A lot of arguments for this command!");
            
            var argType = int.TryParse(args[1], out var cheatState);
            if (argType)
            {
                if (cheatState is < 0 or > 1)
                    throw new Exception("Not correct second argument!");
                _cheatMode = cheatState == 1;
                InGameConsole.Console.cheatMode = _cheatMode;
            }
            else
            {
                throw new Exception("Not correct second argument!");
            }
        }

        public string ProcessConsoleInput(string input)
        {
            var inputValues = input.Split(" ");

            if (_commands.ContainsKey(inputValues[0]))
            {
                return _commands[inputValues[0]].Execute(inputValues);
            }
            
            return "Unknown command!";
        }
    }
}