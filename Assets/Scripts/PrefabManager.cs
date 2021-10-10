﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PrefabManager : MonoBehaviour {

    protected Dictionary<String, List<GameObject>> pool = new Dictionary<String, List<GameObject>>();
    protected Dictionary<String, GameObject> indexedPrefabs = new Dictionary<String, GameObject>();
    public List<GameObject> prefabs = new List<GameObject>();
    public GameObject defaultParent;
    public List<Sprite> sprites = new List<Sprite>();
    public List<TileBase> tiles = new List<TileBase>();
    private Dictionary<String, TileBase> _tiles = new Dictionary<string, TileBase>();
    public Dictionary<String, Sprite> spriteDict = new Dictionary<String, Sprite>();
    // Use this for initialization
    public void Init () {
        //Debug.Log("Prefab Count: " + prefabs.Count);
        foreach (GameObject prefab in prefabs)
        {
            //Debug.Log("Start: " + prefab.name);
            indexedPrefabs.Add(prefab.name, prefab);
            //GameManager.instance.networkManager.spawnPrefabs.Add(prefab);
        }

        foreach (Sprite sprite in sprites)
        {
            spriteDict.Add(sprite.name, sprite);
        }
        foreach (TileBase tile in tiles)
        {
            _tiles.Add(tile.name, tile);
        }
		
    }

    public TileBase GetTile(string tileName)
    {
        if (!_tiles.ContainsKey(tileName))
        {
            throw new Exception("No valid tile found named: " + tileName);
        }
        return _tiles[tileName];
    }
    public GameObject Get(String prefabName)
    {
        return Get(prefabName, defaultParent);
    }
	
    public GameObject Get(String prefabName, GameObject parentObject)
    {
        if (!indexedPrefabs.ContainsKey(prefabName))
        {
            Debug.LogError("Debug: ");
            foreach (KeyValuePair<string,GameObject> keyValuePair in indexedPrefabs)
            {
                Debug.Log("Prefab - " + keyValuePair.Key + " -- " + keyValuePair.Value.name);
            }
            throw new Exception("No prefab with name: " + prefabName);
        }

        if (pool.ContainsKey(prefabName))
        {
            List<GameObject> gameObjects = pool[prefabName];
            foreach (GameObject gameObject in gameObjects)
            {
                if (!gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(true);
                    gameObject.transform.SetParent(parentObject.transform, false);
                    return gameObject;
                }
            }
        }

        GameObject prefab = indexedPrefabs[prefabName];
        GameObject newGameObject = GameObject.Instantiate<GameObject>(prefab, parentObject.transform);
        if (!pool.ContainsKey(prefabName))
        {
            pool.Add(prefabName, new List<GameObject>());
        }
        pool[prefabName].Add(newGameObject);
        return newGameObject;

    }
    
    public GameObject GetRandomPrefabByTag(String prefabTag, GameObject parentObject)
    {
        List<GameObject> foundPrefabs = new List<GameObject>();
        foreach (GameObject prefab in prefabs)
        {
            if (prefab.tag.Equals(prefabTag))
            {
                foundPrefabs.Add(prefab);
            }
        }

        if (foundPrefabs.Count.Equals(0))
        {
            throw new Exception("No prefabs found with Tag: " + prefabTag);
        }
        int index = (int) Random.Range(0, foundPrefabs.Count);

        GameObject selectedObject = foundPrefabs[index];
        return Get(selectedObject.name);
    }
	
    // Update is called once per frame
    void Update () {
		
    }
}