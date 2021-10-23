using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CTSerializerHelper
{
    //public List<string> fitnessRules = new List<string>();
    public CTSerializerHelper(){
        
    }
    /*public BotFitnessController.BotFitnessRule InitFitnessRule(FitnessManagerConfigData.BotFitnessRuleData botFitnessRuleData, BotFitnessController _botFitnessController){
        switch(botFitnessRuleData.eventType){
            case(FitnessManagerConfigData.BotFitnessRuleAction.TILE_ENTER):
            //case("TileEnterFitnessRule"):
                TileEnterFitnessRule tileEnterFitnessRule = new TileEnterFitnessRule();
                tileEnterFitnessRule.Init(_botFitnessController, botFitnessRuleData);
                return tileEnterFitnessRule;
            case (FitnessManagerConfigData.BotFitnessRuleAction.COLLECT):
                EntityCollectFitnessRule entityCollectFitnessRule = new EntityCollectFitnessRule();
                entityCollectFitnessRule.Init(_botFitnessController, botFitnessRuleData);
                return entityCollectFitnessRule;


            case (FitnessManagerConfigData.BotFitnessRuleAction.HAS_NOT_MOVED):
                HasNotMovedFitnessRule hasNotMovedFitnessRule = new HasNotMovedFitnessRule();
                hasNotMovedFitnessRule.Init(_botFitnessController, botFitnessRuleData);
                return hasNotMovedFitnessRule;


            case (FitnessManagerConfigData.BotFitnessRuleAction.HEALTH_CHANGE):
                HealthChangeFitnessRule healthChangeFitnessRule = new HealthChangeFitnessRule();
                healthChangeFitnessRule.Init(_botFitnessController, botFitnessRuleData);
                return healthChangeFitnessRule;

        }
        throw new System.Exception("No FitnessRule found with type: " + botFitnessRuleData.eventType);
    }*/
}
