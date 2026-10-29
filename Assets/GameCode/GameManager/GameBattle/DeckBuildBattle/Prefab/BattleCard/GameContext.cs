using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class GameContext
{
    [SerializeReference] public List<GameModify> modifyHistory = new();
}
