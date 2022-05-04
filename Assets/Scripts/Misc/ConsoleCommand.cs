using System;

namespace Misc
{
    public class ConsoleCommand
    {
        public ConsoleCommand(string command, Action<string[]> commandAction)
        {
            Command = command;
            CommandAction = commandAction;
            CommandResText = string.Empty;
            CommandFailText = string.Empty;
        }

        public ConsoleCommand(string command, Action<string[]> commandAction, string commandResText):this(command, commandAction)
        {
            CommandResText = commandResText;
        }
        
        public ConsoleCommand(string command, Action<string[]> commandAction, string commandResText, string commandFailText):this(command, commandAction, commandResText)
        {
            CommandFailText = commandFailText;
        }

        public string Execute(string[] args)
        {
            try
            {
                CommandAction?.Invoke(args);
                return CommandResText;
            }
            catch (Exception e)
            {
                return CommandFailText +$" {e.Message}";
            }
        }
        
        public string Command { get; private set; }
        public string CommandResText { get; private set; }
        public string CommandFailText { get; private set; }
        public Action<string[]> CommandAction { get; private set; }
    }
}