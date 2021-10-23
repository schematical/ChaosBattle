using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : CTBaseMonoBehavior {
    public GameObject attachedGameObject;
 
    public SpriteRenderer spriteRenderer;
    public Vector3 startScale;
    public Color color;

    // Use this for initialization
    void Start () {
        startScale = transform.localScale;
    }
	
    public override string _class_name
    {
        get
        {
            return "HighlightObject";
        }
    }
    // Update is called once per frame
    void Update () {
        
        if(attachedGameObject == null){
            return;
        }
        if (!attachedGameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            attachedGameObject = null;
            return;
        }
        transform.position = attachedGameObject.transform.position;
        transform.rotation = attachedGameObject.transform.rotation;
    }
    public void MakeMaskable(){
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }
    public void Attach(GameObject _attachedGameObject)
    {
        Attach(_attachedGameObject, CTColor.BrightBlue1);
       
    }
    public void Attach(GameObject _attachedGameObject, Color _color){
        attachedGameObject = _attachedGameObject;
        color = _color;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer attachedSpriteRender = attachedGameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = attachedSpriteRender.sprite;
        spriteRenderer.color = color;
        //SpriteMask spriteMask = attachedGameObject.AddComponent<SpriteMask>();
        //spriteMask.sprite = attachedSpriteRender.sprite;
      
    }
    public void DestroyEvent(WorldEvent worldEvent){
        attachedGameObject = null;
        gameObject.SetActive(false);
        //Destroy(gameObject);
        //Destroy(this);
    }
}