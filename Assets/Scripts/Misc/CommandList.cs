using System;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Misc
{
    public class CommandList : MonoBehaviour
    {
        private Dictionary<string, ConsoleCommand> _commands;
        private bool _cheatMode;
        
        [SerializeField] private TextMeshProUGUI fieldPlayerPos;

        public MainPlayerHandler player;
        
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
            if (fieldPlayerPos != null && player != null && fieldPlayerPos.gameObject.activeSelf)
            {
                fieldPlayerPos.text = "Player Pos: " + player.transform.position;
            }
        }

        private void LoadCommands()
        {
            ConsoleCommand temp = new ConsoleCommand("set_scene", C_LoadScene, "Succesfully load scene!", "Error when loading!");
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
            
            temp = new ConsoleCommand("get_player", C_GetPlayer);
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

            if (player is null)
            {
                throw new Exception("Player invalid!");
            }

            Debug.Log(args[1]);
            Debug.Log(args[2]);
            player.transform.position = new Vector3(int.Parse(args[1]), int.Parse(args[2]), player.transform.position.z);
        }
        
        private void C_MovePlayerOnVector(string[] args)
        {
            if (!_cheatMode) return;
            
            if (args.Length > 3)
                throw new Exception("A lot of arguments for this command!");
            if (args.Length < 3)
                throw new Exception("Not enough arguments for this command!");

            if (player is null)
            {
                throw new Exception("Player invalid!");
            }

            player.transform.position += new Vector3(int.Parse(args[1]), int.Parse(args[2]), 0);
        }

        private void C_GetAllScenes(string[] args)
        {
            if (!_cheatMode) return;
            if (args.Length > 1)
                throw new Exception("A lot of arguments for this command!");

            var scenes = string.Empty;
            var num = 0;
            foreach(var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                    scenes += $"[{num++}] {scene.path}\n";
            }

            throw new Exception(scenes);
        }

        private void C_GetPlayer(string[] args)
        {
            if (args.Length > 1)
                throw new Exception("A lot of arguments for this command!");
            
            var p = FindObjectOfType<MainPlayerHandler>();
            if (p != null) player = p;
            Debug.Log(p);
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