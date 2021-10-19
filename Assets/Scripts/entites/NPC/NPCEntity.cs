using System;
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
    public ChaosItem primaryHeldItem;
    public bool isAlive = true;
    public Color bodyColor = Color.green;
    public HingeJoint2D handJoint;
    private BrainBase brain;
    private BaseAction currAction = null;
    private ParticleSystem _particalSystem;
    private List<BaseAction> actionHistory = new List<BaseAction>();
    private List<ChaosInteraction> _interactions = new List<ChaosInteraction>();
    private Animator _animatior;
    public SpriteRenderer HeadSpriteRenderer { get; set; }
    public SpriteRenderer BodySpriteRenderer { get; set; }
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
        _animatior = GetComponent<Animator>();


    }

    public void Init()
    {
        
        SetStatVal(ChaosEntityStatType.Health, GetStatVal(ChaosEntityStatType.MaxHealth));
        SetStatVal(ChaosEntityStatType.StunDuration, 0);
        isAlive = true;
        brain = new BasicBrainV1(this);
        NPCEntityHead = GameManager.instance.PrefabManager.Get("NPCEntityHead").GetComponent<NPCEntityHead>();
        NPCEntityHead.transform.localPosition = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y + .5f,
            -1
        );
        NPCEntityHead.SetNPCEntity(this);
        HeadSpriteRenderer = NPCEntityHead.GetComponent<SpriteRenderer>();
        handJoint = GetComponents<HingeJoint2D>()[1];
        handJoint.enabled = false;
        
        Rigidbody2D headRigidbody2D = NPCEntityHead.GetComponent<Rigidbody2D>();
        GetComponents<Joint2D>()[0].connectedBody = headRigidbody2D;
        primaryHeldItem = null;
        currAction = null;
        actionHistory.Clear();
        
        BodySpriteRenderer = GetComponent<SpriteRenderer>();

        Color c = HeadSpriteRenderer.color;
        c.a = 1;
        HeadSpriteRenderer.color = c;
        SpriteRenderer bodySpriteRenderer = GetComponent<SpriteRenderer>();
        c = bodySpriteRenderer.color;
        c.a = 1;
        bodySpriteRenderer.color = c;
        GetComponent<PolygonCollider2D>().isTrigger = false;
        // NPCEntityHead.GetComponent<BoxCollider2D>().isTrigger = false;
        if (_animatior)
        {
            _animatior.enabled = true;
        }
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
        HeadSpriteRenderer.color = new Color(
            1, //bodyColor.r + redPct,
            1 - redPct, //bodyColor.g * (1 - redPct),
            1 - redPct// bodyColor.b * (1 - redPct)
        );

        
        float stunDuration = GetStatVal(ChaosEntityStatType.StunDuration);
        if (IsStunned())
        {
            float newStunDuration = stunDuration - Time.deltaTime;
            if (newStunDuration > 0)
            {
                SetStatVal(ChaosEntityStatType.StunDuration, stunDuration - Time.deltaTime);
                _rigidbody2D.rotation = 90;
                return;
            } else
            {
                _animatior.enabled = true;
                _rigidbody2D.rotation = 0;
             
            }
           
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
        actionHistory.Add(currAction);
    }

    public List<BaseAction> GetActionHistory()
    {
        return actionHistory;
    }

    private void MarkDead()
    {
        if (!isAlive)
        {
            throw new Exception("Your already dead");
        }

        isAlive = false;
        GetComponents<Joint2D>()[0].connectedBody = null;
        Color c = HeadSpriteRenderer.color;
        c.a = .25f;
        HeadSpriteRenderer.color = c;
        SpriteRenderer bodySpriteRenderer = GetComponent<SpriteRenderer>();
        c = bodySpriteRenderer.color;
        c.a = .25f;
        bodySpriteRenderer.color = c;
        GetComponent<PolygonCollider2D>().isTrigger = true;
        // NPCEntityHead.GetComponent<BoxCollider2D>().isTrigger = true;
        
        SetPrimaryHeldItem(null);
        _animatior.enabled = false;
    }

    public void TakeDamage(ChaosInteraction chaosInteraction)
    {
        _particalSystem.Emit((int)chaosInteraction.Amount);
        int health = (int) GetStatVal(ChaosEntityStatType.Health);
        SetStatVal(ChaosEntityStatType.Health, health - (int)chaosInteraction.Amount);
        AddInteraction(chaosInteraction);
    }

    public void AddInteraction(ChaosInteraction chaosInteraction)
    {
        _interactions.Add(chaosInteraction);
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
    }

    private void OnCollisionExit2D(Collision2D other)
    {
    }

    public float speed
    {
        get { return 2f; }
    }

    public void SetTeam(ChaosTeam chaosTeam)
    {
        this.chaosTeam = chaosTeam;
        if (this.chaosTeam.name == "Team 1")
        {
            // NPCEntityHead.GetComponent<SpriteRenderer>().color = Color.blue;
            bodyColor = Color.blue;
        }

        BodySpriteRenderer.color = bodyColor;
    }

    public ChaosTeam GetTeam()
    {
        return chaosTeam;
    }

    public void SetPrimaryHeldItem(ChaosItem chaosItem)
    {  
  
        if (chaosItem)
        {
          
            chaosItem.SetHoldingEntity(this);
            handJoint.connectedBody = chaosItem.GetComponent<Rigidbody2D>();

            handJoint.enabled = true;
        }
        else
        {
            if (primaryHeldItem)
            {
                primaryHeldItem.SetHoldingEntity(null);
            }

            handJoint.connectedBody = null;

            handJoint.enabled = false;
        }
        primaryHeldItem = chaosItem;
    }

    public override void CleanUp()
    {
        base.CleanUp();
        // Destroy(NPCEntityHead);
        NPCEntityHead.gameObject.SetActive(false);
    }

    public void TakeHeal(ChaosInteraction chaosInteraction)
    {

        int hitPoints = (int)chaosInteraction.Amount;
        int health = (int) GetStatVal(ChaosEntityStatType.Health);
        int newHealth = health + hitPoints;
        if (newHealth > GetStatVal(ChaosEntityStatType.MaxHealth))
        {
            newHealth = (int)GetStatVal(ChaosEntityStatType.MaxHealth);
        }
        SetStatVal(ChaosEntityStatType.Health, newHealth);
        AddInteraction(chaosInteraction);
    }

    public void TakeStun(ChaosInteraction chaosInteraction)
    {
       float stunDuration = chaosInteraction.Amount;
       SetStatVal(ChaosEntityStatType.StunDuration, stunDuration);
       AddInteraction(chaosInteraction);
       _animatior.enabled = false;
    }
    public override string GetDebugString()
    {
        return "Action: " + currAction.GetDebugString();
    }

    public bool IsStunned()
    {
        float stunDuration = GetStatVal(ChaosEntityStatType.StunDuration);
        return stunDuration > 0;
    }

    public List<ChaosInteraction> GetInteractions()
    {
        return _interactions;
    }
}