using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAction
{
    public const string NONE = "none";
    public const string MOVE = "move";
    public const string ATTACK = "attack";
    public const string END_LEVEL = "end_level";
    
    public const string INTERACTION = "interaction";

   [HideInInspector] public string name = NONE;
   [HideInInspector] public Cell cellFrom;
   [HideInInspector] public Cell cellTo;

}
