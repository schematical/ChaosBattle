﻿using System.Collections.Generic;
using services;
using services.Seed;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ChaosLevel
{
    public static readonly Vector2 MapDimensions = new Vector2(16, 16);
    public SwordObject swordObject;
    public List<ChaosTeam> teams = new List<ChaosTeam>();
    public List<ChaosEntity> entities = new List<ChaosEntity>();
    public void InitLevel()
    {
        teams.Add(new ChaosTeam("Team 1"));
        teams.Add(new ChaosTeam("Team 2"));
        
        swordObject = GameManager.instance.PrefabManager.Get("SwordObject").GetComponent<SwordObject>();
             
        swordObject.transform.localPosition = new Vector3(
            MapDimensions.x / 2, 
            MapDimensions.y / 2,
            0
        );
        entities.Add(swordObject);
        for (int x = 0; x < MapDimensions.x; x++)
        {
            for (int y = 0; y < MapDimensions.y; y++)
            {
                RuleTile _cloudTile = GameObject.Instantiate(GameManager.instance.floorTile);
                GameManager.instance.floorTilemap.SetTile(
                    new Vector3Int(x, y, 0),
                    _cloudTile
                );
                
                
                ChaosSeed chaosSeed = GameManager.instance.ChaosSeed.Spawn("_" + x + "," + y);
                  
                int val = chaosSeed.Next(0, 50);
                if (val < 2)
                {
                    
                    /*float rotation = chaosSeed.Next(0, 3);
                       
                    IslandObject islandObject = GameManager.instance.PrefabManager.Get("IslandObject").GetComponent<IslandObject>();
                    IslandData islandData = new IslandData();
                    islandData.Init( chaosSeed);
                    islandObject.SetIslandData(islandData);
                    islandObject.transform.Rotate(new Vector3(0,0, (float)(rotation * 90)));
                    islandObject.transform.localPosition = new Vector3(x, y, 0);
                    _islandObjects.Add(islandObject);
                }else if (val.Equals(3))
                { */
                    // BoatData boatData = BoatBuilder.BuildRandomBoat(chaosSeed);

                    NPCEntity npcEntity = GameManager.instance.PrefabManager.Get("NPCEntity").GetComponent<NPCEntity>();
             
                    npcEntity.transform.localPosition = new Vector3(x, y, 0);
                    npcEntity.SetTeam(teams[val]);
                    //boatObject.SetBoatData(boatData);
                    entities.Add(npcEntity);

                }
                
            }
        }
        GameManager.instance.Camera.transform.localPosition = new Vector3(
            MapDimensions.x / 2,
            MapDimensions.y / 2,
            -10
        );
    }
}
