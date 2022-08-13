using System;

namespace UnlockGames.BA.Game.DialogueSystem
{
    //TODO: Make deprecate
    [Flags]
    public enum NodeType
    {
        none = 0, 
        consumable = 1<<1,
        quest = 1<<2
    }
}