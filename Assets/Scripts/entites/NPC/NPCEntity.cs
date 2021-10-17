﻿using System;
using System.Collections;
using System.Collections.Generic;
using services;
using UnityEngine;

public class NPCEntity : ChaosEntity, iNavagatable
{
    private NPCEntityHead NPCEntityHead;
    private Rigidbody2D _rigidbody2D;
    public PathFinder PathFinder;
    private Vector2Int lastVector2Int;
    public ChaosTeam chaosTeam;
    public ChoasItem primaryHeldItem;
    public bool isAlive = true;
    public Color bodyColor = Color.green;
    public HingeJoint2D handJoint;
    private BrainBase brain;
    private BaseAction currAction = null;
    private ParticleSystem _particalSystem;

    NPCEntity() : base()
    {
        InitStat(ChaosEntityStatType.MaxHealth, 100);
        InitStat(ChaosEntityStatType.Health, GetStatVal(ChaosEntityStatType.MaxHealth));
        InitStat(ChaosEntityStatType.StunDuration, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
   
    
        PathFinder = new PathFinder(this, null);
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _particalSystem = GetComponent<ParticleSystem>();
        
   
    }

    public void Init()
    {
        SetStatVal(ChaosEntityStatType.Health, 100);
        isAlive = true;
        brain = new BasicBrainV1(this);
        NPCEntityHead = GameManager.instance.PrefabManager.Get("NPCEntityHead").GetComponent<NPCEntityHead>();
        NPCEntityHead.transform.localPosition = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y + .5f,
            0
        );
        NPCEntityHead.SetNPCEntity(this);
        handJoint = GetComponents<HingeJoint2D>()[1];
        handJoint.enabled = false;
        
        Rigidbody2D headRigidbody2D = NPCEntityHead.GetComponent<Rigidbody2D>();
        GetComponents<Joint2D>()[0].connectedBody = headRigidbody2D;
        primaryHeldItem = null;
        currAction = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            return;
        }

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

        
        float stunDuration = GetStatVal(ChaosEntityStatType.StunDuration);
        if (stunDuration > 0)
        {
            SetStatVal(ChaosEntityStatType.StunDuration, stunDuration - Time.deltaTime);
            Debug.Log("Stunned: " + stunDuration);
            _rigidbody2D.rotation = 90;
            return;
        }

        if (stunDuration <= 0)
        {
            _rigidbody2D.rotation = 0;
            // SetStatVal(ChaosEntityStatType.StunDuration, 0);
        }
        brain.tick();
        currAction?.tick();
        PathFinder.tickNavigate();
        /*if (primaryHeldItem)
        {
            primaryHeldItem.transform.localPosition = new Vector3(
                this.transform.localPosition.x,
                this.transform.localPosition.y,
             this.transform.localPosition.z - 1
            );
        }*/

     
        // SetStatVal(ChaosEntityStatType.Health, GetStatVal(ChaosEntityStatType.Health) - 1);
    }

    public BaseAction GetCurrentAction()
    {
        return currAction;
    }

    public void SetCurrentAction(BaseAction baseAction)
    {
        currAction = baseAction;
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

    public void TakeDamage(int hitPoints)
    {
        _particalSystem.Emit(hitPoints);
        int health = (int) GetStatVal(ChaosEntityStatType.Health);
        SetStatVal(ChaosEntityStatType.Health, health - hitPoints);
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

    public ChaosTeam GetTeam()
    {
        return chaosTeam;
    }

    public void SetPrimaryHeldItem(ChoasItem choasItem)
    {
        primaryHeldItem = choasItem;
        choasItem.SetHoldingEntity(this);
        handJoint.connectedBody = primaryHeldItem.GetComponent<Rigidbody2D>();

        handJoint.enabled = true;
    }

    public override void CleanUp()
    {
        base.CleanUp();
        // Destroy(NPCEntityHead);
        NPCEntityHead.gameObject.SetActive(false);
    }

    public void TakeHeal(int hitPoints)
    {
      
        int health = (int) GetStatVal(ChaosEntityStatType.Health);
        int newHealth = health + hitPoints;
        if (newHealth > GetStatVal(ChaosEntityStatType.MaxHealth))
        {
            newHealth = (int)GetStatVal(ChaosEntityStatType.MaxHealth);
        }
        SetStatVal(ChaosEntityStatType.Health, newHealth);
    }

    public void TakeStun(int stunDuration)
    {
       
        SetStatVal(ChaosEntityStatType.StunDuration, stunDuration);

    }
    public override string GetDebugString()
    {
        return "Action: " + currAction.GetDebugString();
    }
}