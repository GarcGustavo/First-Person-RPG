using System;
using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Builders.Maze;
using DungeonArchitect.Editors;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonManager : DungeonEventListener
{
    private GameManager _manager;

    private void Start()
    {
        _manager = GameManager.GetInstance();
    }

    public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
    {
        _manager = GameManager.GetInstance();
        _manager.dungeonBuilt = true;
    }

    public override void OnDungeonDestroyed(Dungeon dungeon)
    {
        _manager = GameManager.GetInstance();
        _manager.dungeonBuilt = false;
    }

    public void GenerateNewDungeon()
    {
        var dungeon = _manager.GetDungeon();
        dungeon.DestroyDungeon();
        dungeon.RandomizeSeed();
        dungeon.Build();
    }
    
}
