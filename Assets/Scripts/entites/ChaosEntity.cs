using System;
using System.Collections.Generic;
using UnityEngine;

namespace services
{
    public class ChaosEntity: MonoBehaviour
    {
        public Dictionary<ChaosEntityStatType, ChaosEntityStat> stats = new Dictionary<ChaosEntityStatType,ChaosEntityStat>();
        private NPCEntity heldByNPCEntity;

        protected ChaosEntityStat InitStat(ChaosEntityStatType type, float val) 
        {
            ChaosEntityStat stat = new ChaosEntityStat(type, val);
            stats.Add(type, stat);
            return stat;
        }

        public float GetStatVal(ChaosEntityStatType type)
        {
            if (!stats.ContainsKey(type))
            {
                throw new Exception("No `stat` of type: " + type.ToString());
            }

            return stats[type].GetVal();
        }
        public void SetStatVal(ChaosEntityStatType type, float val)
        {
            if (!stats.ContainsKey(type))
            {
                throw new Exception("No `stat` of type: " + type.ToString());
            }

            stats[type].SetVal(val);
        }

        public virtual void CleanUp()
        {
            
        }
    }
}