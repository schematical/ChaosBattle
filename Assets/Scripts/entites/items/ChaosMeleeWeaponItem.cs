using UnityEngine;

namespace services
{
    public class ChaosMeleeWeaponItem: ChoasItem
    {
        public ChaosMeleeWeaponItem(): base()
        {
            InitStat(ChaosEntityStatType.Attack, 5);
            InitStat(ChaosEntityStatType.MeleeRange, 2);
            InitStat(ChaosEntityStatType.Windup, 1);
            InitStat(ChaosEntityStatType.Cooldown, 2);
        }

        public override void ApplyActionAnimation(ActionPhase actionPhase)
        {
            JointMotor2D motor  = new JointMotor2D();
            switch (actionPhase)
            {
                case(ActionPhase.Windup):
                    //this.GetComponent<Rigidbody2D>().angularVelocity = -1f; 
         
                    motor.motorSpeed = -100f;
                    motor.maxMotorTorque = 1000;
                    GetHoldingEntity().handJoint.motor = motor;
                    break;
                case(ActionPhase.Acting):
                   //  this.GetComponent<Rigidbody2D>().angularVelocity = 10f;
                   motor.motorSpeed = 10000;
                    motor.maxMotorTorque = 1000;
                    GetHoldingEntity().handJoint.motor = motor;
                    break;
                case(ActionPhase.Cooldown):
                    //  this.GetComponent<Rigidbody2D>().angularVelocity = 10f;
                    motor.motorSpeed = (GetHoldingEntity().handJoint.motor.motorSpeed * 3) / 4;
                    motor.maxMotorTorque = 1000;
                    GetHoldingEntity().handJoint.motor = motor;
                    break;
            }
        }
    }
}