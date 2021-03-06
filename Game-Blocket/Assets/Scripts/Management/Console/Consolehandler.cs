using System;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handlers the Chat
/// TODO: Change to Chathandler, Multiplayer
/// </summary>
public static class ConsoleHandler{

    public static GameObject VisibleChatGO { get; private set; }

    /// <summary>Used for imputfields</summary>
    /// <param name="str">Input</param>
    public static void Handle(string str){
        str = PrepareString(str, out bool isCommand);
        if (string.IsNullOrEmpty(str))
            return;
        if (isCommand){
            Command command = Command.GetCommand(str.Substring(1));
            if(command == null)
                PrintToChat($"Command: \"{str}\" not found");
            command?.action(str);
        }
        else
            PrintToChat(str);
    }

    /// <summary>Self explaining</summary>
    /// <param name="str">String to print</param>
    /// <exception cref="NullReferenceException">If Prefab has no Text Component</exception>
    private static void PrintToChat(string str){
        if(DebugVariables.ShowChatEntries)
            Debug.Log($"Chat: {str}");

        if (VisibleChatGO != null)
            UnityEngine.Object.Destroy(VisibleChatGO);
        GameObject textGO = UnityEngine.Object.Instantiate(PrefabAssets.Singleton.consoleText, UIInventory.Singleton.chatParent.parent.position, Quaternion.identity ,UIInventory.Singleton.chatParent.parent);
        Text text = textGO.TryGetComponent(out Text t) ? t : throw new NullReferenceException();
        text.text = GetPlayerPrefix() + str;
        VisibleChatGO = textGO;
    }



    /// <summary>
    /// TODO: Use real Playernames<br></br><seealso cref="PlayerProfile"/>
    /// </summary>
    /// <returns></returns>
    private static string GetPlayerPrefix(){
        return "Dev > ";
    }
    
    /// <summary>Prepares a string and checks if it is a command</summary>
    /// <param name="str">Input</param>
    /// <param name="isCommand">if the chat is a command (with "/" before)</param>
    /// <returns>prepared string</returns>
    private static string PrepareString(string str, out bool isCommand){
        isCommand = str.StartsWith(@"/");
        if (isCommand)
            str = str.ToLower();
        return str.Trim();
    }


    private class Command {

        public static List<Command> Commands { get; } = new List<Command>()
        {
            new Command("ping"){ action = (str) => PrintToChat("Pong")},
            new Command("summon"){ action = (str) => { 
                // /summon 1 ~ ~ ~
                string[] arguments = CutCmd(str).Split(' ');
                try{
                    //Enemy id
                    int enemyId = int.Parse(arguments[0]);
                    //X
                    int x = int.Parse(arguments[1]);
                    //Y
                    int y = int.Parse(arguments[2]);
                    //Z
                    int z = int.Parse(arguments[3]);
                    
                    //Forget enemyhandler lol
                }catch(Exception e){
                    PrintToChat(str: e.ToString());
}
} },
            new Command("kill"){ action = (x) => {
                GlobalVariables.LocalPlayer.transform.position = new Vector3(0, 10);
            } },
            new Command("gamemode"){ action = (x) => {
                sbyte str = (sbyte.TryParse(x.Split(' ')[1], out sbyte res) ? res : (sbyte)-1) ;
                switch(str){
                    case 0: PlayerVariables.Gamemode = Gamemode.SURVIVAL;
                    break;
                    case 1: PlayerVariables.Gamemode = Gamemode.CREATIVE;
                    break;
                    case -1:
                        string s = x.Split(' ')[1];
                        switch (s.ToUpper()) {
                            case "CREATIVE":
                                PlayerVariables.Gamemode = Gamemode.CREATIVE;
                                break;
                            case "SURVIVAL":
                                PlayerVariables.Gamemode = Gamemode.SURVIVAL;
                                break;
                        }
                    break;
                    default:PrintToChat($"Gamemode {str} not found!");
                    break;
                }
            }, Synms = { "gm", "gamem" } }
            ,new Command("give"){ action = (x) => {
                uint id = (uint.TryParse(x.Split(' ')[1], out uint res) ? res : 0) ;
                if(id ==0){ PrintToChat($"Item {x.Split(' ')[1]} does not exist!"); return; }
                ushort count = ushort.TryParse(x.Split(' ')[2], out ushort countres) ? countres : (ushort)0 ;
                if(count ==0){ PrintToChat($"Item {id} could not be added with the Count - {count} !"); return; }
                Item i = ItemAssets.Singleton.GetItemFromItemID(id) ?? null;
                if(i==null)
                    PrintToChat($"Item {x.Split(' ')[1]} does not exist!");
                else
                    Inventory.Singleton.AddItem(i,count,out ushort itemCountNotAdded);
            }
            },new Command("collision")
            {
                action = (x) => {
                string s = x.Split(' ')[1];
                    switch (s)
                    {
                        case "true":
                            GlobalVariables.LocalPlayer.GetComponentInChildren<BoxCollider2D>().enabled = true;
                            break;
                        case "false":
                            GlobalVariables.LocalPlayer.GetComponentInChildren<BoxCollider2D>().enabled = false;
                            break;
                        case "t":
                            GlobalVariables.LocalPlayer.GetComponentInChildren<BoxCollider2D>().enabled = true;
                            break;
                        case "f":
                            GlobalVariables.LocalPlayer.GetComponentInChildren<BoxCollider2D>().enabled = false;
                            break;
                    }
                }
            },
            new Command("timeset"){action = (x) => {
                ClockHandler clock = ClockHandler.Singleton;
                string str;
                if(x.Split(' ').Length > 1)
                {
                    str = x.Split(' ')[1];
                }
                else
                {
                    PrintToChat("No daytime defined");
                    return;
                }
                if (str.Equals("day"))
                {
                    if(clock.hours >= DayNightCycle.Singleton.dawnTo)
                        clock.days++;
                    clock.seconds = 0;
                    clock.minutes = 0;
                    clock.hours = DayNightCycle.Singleton.dawnTo; 
                }
                else
                if (str.Equals("night"))
                {
                    if(clock.hours >= DayNightCycle.Singleton.duskTo)
                        clock.days++;
                    clock.seconds = 0;
                    clock.minutes = 0;
                    clock.hours = DayNightCycle.Singleton.duskTo;
                }
                else
                {
                    PrintToChat($"\"{str}\" is not a daytime");
                }
            } }
        };

        //Returns only the value after
        public static string CutCmd(string cmd){
            if (cmd.StartsWith(@"/"))
                cmd = cmd.Substring(1);
            return cmd.Trim();
        }

        public static Command GetCommand(string fullCommand){
            string cmdName = CutCmd(fullCommand.Split(' ')[0]);
            foreach(Command com in Commands)
                if(com.name == cmdName || com.Synms.Contains(cmdName))
                    return com;
            return null;
        }
    
        public List<string> Synms { get; set; } = new List<string>();

        public string name;

        public Action<string> action;
        
        public Command(List<string> prefix){
            name = prefix[0];
            Synms.AddRange(prefix);
        }
        public Command(string prefix) {
            name = prefix;
        }
        public Command(){}
    }

}
