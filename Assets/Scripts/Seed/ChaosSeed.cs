﻿using System;

namespace services.Seed
{
    [Serializable]
    public class ChaosSeed
    {
        private string _seedBase;
        private int _seedMax = 9999;
        private System.Random _pseudoRandom;
        private ChaosSeed _parent;
        private int _seed;
        public ChaosSeed(string seedBase)
        {
            _seedBase = seedBase;
            Init();
        }

        private void Init()
        {
            string uniqueStr = "";
            if (_parent != null)
            {
                uniqueStr += _parent.SeedBase + ":";
            }

            uniqueStr += _seedBase;
            _pseudoRandom = new System.Random((uniqueStr).GetHashCode());
            _seed = _pseudoRandom.Next(0, _seedMax);
        }

 
    public int SeedMax => _seedMax;


        public ChaosSeed Spawn(string strExtra)
        {
            ChaosSeed child = new ChaosSeed(
                SeedBase + "_" + strExtra
            )
            {
                _parent = this,
                _seedMax = SeedMax
            };
            child.Init();
          return child;
        }

        public string SeedBase => _seedBase;


        public int Seed => _seed;

        public int Next(int minVal, int maxVal)
        {
           return _pseudoRandom.Next(minVal, maxVal);
        }

        public int Next(Func<int, int> func)
        {
            return func(Seed);
        }

        public int Roll(int optionCount, int offset)
        {
            return Roll(optionCount, offset, 1);
        }

        public int Roll(int optionCount, int offset, int divideBy)
        {
            return Math.Abs((_seed - offset)/divideBy) % optionCount;
        }
        
    }
}