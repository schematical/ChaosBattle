using UnityEngine;


    public class ChaosHealingItem: ChaosItem
    {
        private ParticleSystem _particalSystem;

        public ChaosHealingItem(): base()
        {
            InitStat(ChaosEntityStatType.HealthRecovered, 15);
            InitStat(ChaosEntityStatType.Range, 2);
            InitStat(ChaosEntityStatType.Windup, .5f);
            InitStat(ChaosEntityStatType.Cooldown, 3);
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
                ParticleSystem.MainModule main = _particalSystem.main;
                main.startColor = Color.green;
                _particalSystem.Emit((int)GetStatVal(ChaosEntityStatType.HealthRecovered));
                ChaosInteraction chaosInteraction = new ChaosInteraction(
                    ChaosInteractionType.Heal,
                    (int) GetStatVal(ChaosEntityStatType.HealthRecovered),
                    HeldByNpcEntity,
                    ((NPCEntity)target)
                );
                ((NPCEntity) target).TakeHeal(
                    chaosInteraction
                );
                HeldByNpcEntity.AddInteraction(chaosInteraction);
            }
        }
    }
