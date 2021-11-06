using UnityEngine;


    public class MoneyItem: ChaosItem
    {


        public MoneyItem(): base()
        {
            InitStat(ChaosEntityStatType.MoneyValue, 10);
        }
        public override string _class_name
        {
            get { return "MoneyItem"; }
        }

        public virtual void Start()
        {
        }

        public override void ApplyActionAnimation(ActionPhase actionPhase)
        {
            /*JointMotor2D motor  = new JointMotor2D();
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
                 
            }*/
        }
        public override void Use(ChaosEntity target)
        {
            if (target is VendingMachineEntity)
            {
               
                /*ChaosInteraction chaosInteraction = new ChaosInteraction(
                    ChaosInteractionType.Purchase,
                    (int) GetStatVal(ChaosEntityStatType.MoneyValue),
                    HeldByChaosNpcEntity,
                    ((VendingMachineEntity)target)
                );
                ((VendingMachineEntity) target).TakeHeal(
                    chaosInteraction
                );
                HeldByChaosNpcEntity.AddInteraction(chaosInteraction);*/
                Debug.Log("Big Spender!");
            }
            else
            {
                Debug.Log("Trying to use Money on a non VendingMachine");
            }
        }
    }
