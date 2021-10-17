using System;
using System.Collections.Generic;
using services;
using services.Seed;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ChaosLevel
{
    public static readonly int Border = 8;
    public static readonly Vector2 MapDimensions = new Vector2(18, 14);
    public List<ChaosTeam> teams = new List<ChaosTeam>();
    public List<ChaosEntity> entities = new List<ChaosEntity>();
    public void InitLevel()
    {
        teams.Add(new ChaosTeam("Team 1"));
        teams.Add(new ChaosTeam("Team 2"));

        for (int x = 0 - Border; x < MapDimensions.x + Border; x++)
        {
            for (int y = 0 - Border; y < MapDimensions.y + Border; y++)
            {
                TileBase tile;
                Tilemap tilemap;
                Boolean isOutsideOfBattleField = (
                    x < 0 ||
                    x >= MapDimensions.x ||
                    y < 0 ||
                    y >= MapDimensions.y
                );
                if (isOutsideOfBattleField) {
                    tile = GameManager.instance.PrefabManager.GetTile("CloudTile");
                    tilemap = GameManager.instance.wallTilemap;
                }
                else
                {
                    tile = GameManager.instance.PrefabManager.GetTile("BoatTopTile");
                    tilemap = GameManager.instance.floorTilemap;
                    
                }
            
                tilemap.SetTile(
                    new Vector3Int(x, y, 0),
                    tile
                );
                if (!isOutsideOfBattleField)
                {
                    ChaosTeam team = null;
                    int? placeY = null;
                    if (y == 0)
                    {
                        placeY = y;
                        team = teams[0];
                    }else if (y == MapDimensions.y - 1)
                    {
                        placeY = y;
                        team = teams[1];
                    }

                    if (team != null)
                    {
                        if ((x % 4) == 0)
                        {
                            SpawnNPC(x, y, team);
                        }
                    }
                    

                    ChaosSeed chaosSeed = GameManager.instance.ChaosSeed.Spawn("_" + x + "," + y);
                    if (y == MapDimensions.y / 2)
                    {
                        int val = chaosSeed.Next(0, 7);
                        if (val == 1)
                        {
                            SwordMeeleWeaponItem swordObject = GameManager.instance.PrefabManager
                                .Get("SwordMeeleWeaponItem").GetComponent<SwordMeeleWeaponItem>();

                            swordObject.transform.localPosition = new Vector3(x, y, -2);
                            swordObject.Init();
                            entities.Add(swordObject);
                        }
                        else if (val == 2)
                        {
                            MedkitItem medkitItem = GameManager.instance.PrefabManager.Get("MedkitItem")
                                .GetComponent<MedkitItem>();

                            medkitItem.transform.localPosition = new Vector3(x, y, -2);
                            medkitItem.Init();
                            entities.Add(medkitItem);
                        } else if (val == 3)
                        {
                            TireShieldItem chaosShieldItem = GameManager.instance.PrefabManager.Get("TireShieldItem")
                                .GetComponent<TireShieldItem>();

                            chaosShieldItem.transform.localPosition = new Vector3(x, y, -2);
                            chaosShieldItem.Init();
                            entities.Add(chaosShieldItem);
                        }
                    }
                }

            }
        }
        GameManager.instance.Camera.transform.localPosition = new Vector3(
            MapDimensions.x / 2,
            MapDimensions.y / 2,
            -10
        );
    }

    public NPCEntity SpawnNPC(int x, int y, ChaosTeam team)
    {
        NPCEntity npcEntity = GameManager.instance.PrefabManager.Get("NPCEntity")
            .GetComponent<NPCEntity>();
                     
        npcEntity.transform.localPosition = new Vector3(x, y, 0);
        npcEntity.Init();
        npcEntity.SetTeam(team);
        //boatObject.SetBoatData(boatData);
        entities.Add(npcEntity);
        return npcEntity;
    }

    public void Tick()
    {
        //TODO Move this somewhere better
        Dictionary<ChaosTeam, int> teamCounts = new Dictionary<ChaosTeam, int>();
        teams.ForEach((team =>
        {
            teamCounts.Add(team, 0);    
        }));
        
        GameManager.instance.level.entities.ForEach((entity =>
        {
            NPCEntity testNPCEntity = entity.GetComponent<NPCEntity>();
            if (!testNPCEntity)
            {
                return;
            }

            if (!testNPCEntity.isAlive)
            {
                return;
            }

            int count = teamCounts[testNPCEntity.GetTeam()];
            teamCounts[testNPCEntity.GetTeam()] = count + 1;
        }));
        bool triggerReset = false;
        teams.ForEach((team =>
        {
            if (teamCounts[team] == 0)
            {
                triggerReset = true;
                Debug.Log(team.name + " LOSES");
                
            }  
        }));
        if (triggerReset)
        {
            CleanUp();
            InitLevel();
        }
    }

    public void CleanUp()
    {
        entities.ForEach((entity) =>
        {
            entity.CleanUp();
           //  Object.Destroy(entity.gameObject);
           entity.gameObject.SetActive(false);
        });
        entities.Clear();
        teams.Clear();
    }

    public Vector3Int GetRandomFloorTileVec()
    {
      return new Vector3Int(
            (int) Random.Range(0, MapDimensions.x),
            (int) Random.Range(0, MapDimensions.y),
                0
        );
    }

    public ChaosEntity FindClosestEntity(Vector3 vector3, Func<ChaosEntity, bool> test)
    {
        ChaosEntity closestEntity = null;
        float closestEntityDist = 99999;
     
        entities.ForEach((entity => {
            if (!test(entity))
            {
                return;
            }
            float currEnemyDist = (entity.transform.position - vector3).sqrMagnitude;
            if (
                !closestEntity ||
                currEnemyDist < closestEntityDist  
            ) {
                closestEntity = entity;
                closestEntityDist = currEnemyDist;
            }
        })); 
        return closestEntity;
    }
}
