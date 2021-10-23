
using System;
using UnityEngine;


public abstract class ChaosItem: ChaosEntity
{
    private ChaosNPCEntity _heldByChaosNpcEntity;

    public ChaosNPCEntity HeldByChaosNpcEntity => _heldByChaosNpcEntity;

    private void OnCollisionEnter2D(Collision2D other)
    {
        ChaosNPCEntity chaosNpcEntity = other.collider.GetComponent<ChaosNPCEntity>();
        if (chaosNpcEntity != null)
        {
            OnCollisionEnterNPCEntity(chaosNpcEntity);
        }
    
    }

    private void OnCollisionEnterNPCEntity(ChaosNPCEntity chaosNpcEntity)
    {
        if (IsCurrentlyHeld())
        {
            return; // We are alreadying being held
        }
        if (chaosNpcEntity.primaryHeldItem)
        {
            return; // They are already holding something.
        }
        chaosNpcEntity.SetPrimaryHeldItem(this);
    }

    public bool IsCurrentlyHeld()
    {
        return !!_heldByChaosNpcEntity;
    }

    public void Init()
    {
        _heldByChaosNpcEntity = null;
    }

    private void OnCollisionExit2D(Collision2D other)
    {

    
  
    }

    public void SetHoldingEntity(ChaosNPCEntity chaosNpcEntity)
    {
        _heldByChaosNpcEntity = chaosNpcEntity;
    }

    public ChaosNPCEntity GetHoldingEntity()
    {
        return _heldByChaosNpcEntity;
    }

    public virtual void ApplyActionAnimation(ActionPhase actionPhase)
    {
        
    }

    public virtual void Use(ChaosEntity target)
    {
        throw new Exception("The `ChaosItem.Use` method needs to be overwritten.");
    }
}
