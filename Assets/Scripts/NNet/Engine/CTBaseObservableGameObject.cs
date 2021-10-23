using UnityEngine;
using System.Collections.Generic;


public abstract class CTBaseObservableGameObject: CTBaseMonoBehavior
{
    protected bool diagnosticInit = false;
    protected Color preDiagnosticColor;
    protected HighlightObject highlightObject;

    public bool activeInHierarchy{
        get{
            return gameObject.activeInHierarchy;
        }
    }
    /*
    public static List<string> globalAttributeIds = new List<string>();
    protected List<ObservableAttribute> observableAttributes = new List<ObservableAttribute>();
    public void AddObservableAttribute(string attribueId, ObservableAttribute observableAttribute){
        if(!globalAttributeIds.Contains(attribueId)){
            globalAttributeIds.Add(attribueId);
        }
        observableAttribute.Attach(this);
        observableAttribute.attribueId = attribueId;
        observableAttributes.Add(observableAttribute);
    }
    public float GetObservableAttributeValue(string attribueId){
        foreach(ObservableAttribute observableAttribute in observableAttributes){
            if(observableAttribute.attribueId == attribueId){
                
                return OnObserve(observableAttribute);//observableAttribute.GetValue();
            }

        }
        return -1;
    }
	public override void OnDestroy()
	{
        base.OnDestroy();
        foreach (ObservableAttribute observableAttribute in observableAttributes)
        {
            observableAttribute.Detach();

        }
        observableAttributes.Clear();
	}*/
    public float Observe(string attributeId, ChaosEntity observedEntity){
        return OnObserve(attributeId, observedEntity);
    }
    public virtual float OnObserve(string attributeId, ChaosEntity observerEntity/*ObservableAttribute observableAttribute*/){
        string realTypeAttributeId = CTOA._IS_REAL_TYPE_ + _class_name;
        if(attributeId == realTypeAttributeId){
            return 1;
        }

        return CTOA.__ZERO_VALUE;
    }
    public void SetDiagnosticsMode(bool isOn){
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(isOn){
            //Set the color Alpha to 50%
            preDiagnosticColor = spriteRenderer.color;
            diagnosticInit = true;
            Color color = new Color();
            color.r = spriteRenderer.color.r;
            color.g = spriteRenderer.color.g;
            color.b = spriteRenderer.color.b;
            color.a = .25f;
            spriteRenderer.color = color;

        }else{
            if (diagnosticInit)
            {
                spriteRenderer.color = preDiagnosticColor;
            }
            ClearDiagnostic();
           
        }
    }
    public void MarkDiagnostic(Color color){
        if (highlightObject == null)
        {
            GameObject instance = GameManager.instance.PrefabManager.Get("HighlightObject");// , transform.position, Quaternion.identity);
            instance.transform.position = transform.position;
            highlightObject = instance.GetComponent<HighlightObject>();/*
            highlightObject.transform.SetParent(GameManager.instance.level.boardHolder, false);*/
            highlightObject.transform.localScale = transform.localScale;
            // GameManager.instance.level.AddHighlighedObservibal(this);
        }
        highlightObject.Attach(gameObject, color);
       
    }
    public void ClearDiagnostic()
    {
        if (highlightObject != null)
        {

            highlightObject.gameObject.SetActive(false);
            highlightObject = null;
        }
        //GameManager.instance.diagnosticManager.RemoveHighlighedObservibal(this);
    }
}
