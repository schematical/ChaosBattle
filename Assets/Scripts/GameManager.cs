using System;
using System.Collections;
using System.Collections.Generic;
using services.Seed;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PrefabManager PrefabManager;
    public Camera Camera;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public RuleTile floorTile;
    public ChaosLevel level = new ChaosLevel();

    public ChaosSeed ChaosSeed;
    public MenuManager menuManager;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        ChaosSeed = new ChaosSeed(DateTime.Now.ToString());//"x");
        PrefabManager.Init();
        level.InitLevel();
    }

    // Update is called once per frame
    void Update()
    {

        level.Tick();
    }
}
