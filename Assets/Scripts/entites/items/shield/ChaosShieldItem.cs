﻿using UnityEngine;


    public class ChaosShieldItem: ChaosItem
    {
        private ParticleSystem _particalSystem;

        public ChaosShieldItem(): base()
        {
            InitStat(ChaosEntityStatType.StunDuration, 5);
            InitStat(ChaosEntityStatType.Range, 2);
            InitStat(ChaosEntityStatType.Windup, 1);
            InitStat(ChaosEntityStatType.Cooldown, 7);
        }

        public virtual void Start()
        {
            _particalSystem = GetComponent<ParticleSystem>();
        }

        public override void ApplyActionAnimation(ActionPhase actionPhase)
        {
            JointMotor2D motor  = new JointMotor2D();
            if (actionPhase.Equals(UsePrimaryItemActionPhase.Windup))
            {
                motor.motorSpeed = -100f;
                motor.maxMotorTorque = 1000;
                GetHoldingEntity().handJoint.motor = motor;
            }else if (actionPhase.Equals(UsePrimaryItemActionPhase.Acting))
            {
                //  this.GetComponent<Rigidbody2D>().angularVelocity = 10f;
                motor.motorSpeed = 10000;
                motor.maxMotorTorque = 1000;
                GetHoldingEntity().handJoint.motor = motor;
            } else if (actionPhase.Equals(UsePrimaryItemActionPhase.Cooldown)) {
                //  this.GetComponent<Rigidbody2D>().angularVelocity = 10f;
                motor.motorSpeed = (GetHoldingEntity().handJoint.motor.motorSpeed * 3) / 4;
                motor.maxMotorTorque = 1000;
                GetHoldingEntity().handJoint.motor = motor;
                 
            }
        }
        public override void Use(ChaosEntity target)
        {
            if (target is NPCEntity)
            {
                _particalSystem.Emit((int)GetStatVal(ChaosEntityStatType.StunDuration));
                target.GetComponent<Rigidbody2D>().velocity =
                    (target.transform.position - HeldByNpcEntity.transform.position) * -2;
                ChaosInteraction chaosInteraction = new ChaosInteraction(
                    ChaosInteractionType.Stun,
                    (int) GetStatVal(ChaosEntityStatType.StunDuration),
                    HeldByNpcEntity,
                    ((NPCEntity)target)
                );
                ((NPCEntity) target).TakeStun(
                    chaosInteraction
                );
                HeldByNpcEntity.AddInteraction(chaosInteraction);
            }
        }
    }
