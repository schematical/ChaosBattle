using UnityEngine;

public abstract class ChaosMeleeWeaponItem : ChaosItem
{
    public ChaosMeleeWeaponItem() : base()
    {
        InitStat(ChaosEntityStatType.Attack, 20);
        InitStat(ChaosEntityStatType.Range, 3);
        InitStat(ChaosEntityStatType.Windup, .5f);
        InitStat(ChaosEntityStatType.Cooldown, 1);
    }

    public override void ApplyActionAnimation(ActionPhase actionPhase)
    {
        if (GetHoldingEntity() == null)
        {
            return;
        }
        JointMotor2D motor = new JointMotor2D();
        if (actionPhase.Equals(UsePrimaryItemActionPhase.Windup))
        {
            motor.motorSpeed = -100f;
            motor.maxMotorTorque = 1000;
            GetHoldingEntity().handJoint.motor = motor;
        }
        else if (actionPhase.Equals(UsePrimaryItemActionPhase.Acting))
        {
            //  this.GetComponent<Rigidbody2D>().angularVelocity = 10f;
            motor.motorSpeed = 10000;
            motor.maxMotorTorque = 1000;
            GetHoldingEntity().handJoint.motor = motor;
        }
        else if (actionPhase.Equals(UsePrimaryItemActionPhase.Cooldown))
        {
            //  this.GetComponent<Rigidbody2D>().angularVelocity = 10f;
            motor.motorSpeed = (GetHoldingEntity().handJoint.motor.motorSpeed * 3) / 4;
            motor.maxMotorTorque = 1000;
            GetHoldingEntity().handJoint.motor = motor;
        }
    }

    public override void Use(ChaosEntity target)
    {
        if (target is ChaosNPCEntity)
        {
            target.GetComponent<Rigidbody2D>().velocity =
                (target.transform.position - HeldByChaosNpcEntity.transform.position) * -2;
            ChaosInteraction chaosInteraction = new ChaosInteraction(
                ChaosInteractionType.MeeleAttack,
                (int) GetStatVal(ChaosEntityStatType.Attack),
                HeldByChaosNpcEntity,
                ((ChaosNPCEntity)target)
            );
            ((ChaosNPCEntity) target).TakeDamage(
                chaosInteraction
            );
            HeldByChaosNpcEntity.AddInteraction(chaosInteraction);
        }
    }
}