using System;
using System.Collections;
using System.Collections.Generic;
using services;
using UnityEngine;

public class NPCEntity : ChaosEntity, iNavagatable
{
    private GameObject NPCEntityHead;
    private Rigidbody2D _rigidbody2D;
    public PathFinder PathFinder;
    private Vector2Int lastVector2Int;
    public ChaosTeam chaosTeam;
    public ChoasItem primaryHeldItem;
    public bool isAlive = true;
    public Color bodyColor = Color.green;
    public Joint2D handJoint;
    NPCEntity(): base()
    {
        InitStat(ChaosEntityStatType.Attack, 5);
    }
    // Start is called before the first frame update
    void Start()
    {
        
        PathFinder = new PathFinder(this,  null);
        _rigidbody2D = GetComponent<Rigidbody2D>();
        NPCEntityHead = GameManager.instance.PrefabManager.Get("NPCEntityHead");
        NPCEntityHead.transform.localPosition = new Vector3(
            transform.localPosition.x, 
            transform.localPosition.y + .5f,
            0
        );
        Rigidbody2D headRigidbody2D = NPCEntityHead.GetComponent<Rigidbody2D>();
       
        GetComponents<Joint2D>()[0].connectedBody = headRigidbody2D;
        handJoint = GetComponents<Joint2D>()[1];
        handJoint.enabled = false;
        PathFinder.navigateTo(GameManager.instance.level.swordObject.gameObject);
        InitStat(ChaosEntityStatType.Health, 100);
    }

    public Rigidbody2D GetRigidbody2D()
    {
        return _rigidbody2D;
    }
    // Update is called once per frame
    void Update()
    {
          PathFinder.tickNavigate();
          /*if (primaryHeldItem)
          {
              primaryHeldItem.transform.localPosition = new Vector3(
                  this.transform.localPosition.x,
                  this.transform.localPosition.y,
               this.transform.localPosition.z - 1
              );
          }*/

          if (
              isAlive &&
              GetStatVal(ChaosEntityStatType.Health) <= 0)
          {
              MarkDead();
          }

          float redPct = 1 - GetStatVal(ChaosEntityStatType.Health) / 100;
          GetComponent<SpriteRenderer>().color = new Color(
              bodyColor.r + redPct,
              bodyColor.g * (1 - redPct),
              bodyColor.b * (1 - redPct)
          );
          // SetStatVal(ChaosEntityStatType.Health, GetStatVal(ChaosEntityStatType.Health) - 1);
    }

    private void MarkDead()
    {
        if (!isAlive)
        {
            throw new Exception("Your already dead");
        }
        isAlive = false;
        GetComponents<Joint2D>()[0].connectedBody = null;
        
    }
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        
    }
    private void OnCollisionExit2D(Collision2D other)
    {
   
        
      
    }
    public float speed
    {
        get { return 1f; }
    }

    public void SetTeam(ChaosTeam chaosTeam)
    {
        this.chaosTeam = chaosTeam;
        if (this.chaosTeam.name == "Team 1")
        {
            // NPCEntityHead.GetComponent<SpriteRenderer>().color = Color.blue;
            bodyColor = Color.blue;
        }
    }

    public void SetPrimaryHeldItem(ChoasItem choasItem)
    {
        primaryHeldItem = choasItem;
        choasItem.SetHoldingEntity(this);
        handJoint.connectedBody = primaryHeldItem.GetComponent<Rigidbody2D>();
        
        handJoint.enabled = true;
    }
}
