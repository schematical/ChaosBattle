
public class ChaosBattleBasicScoreCounter: ChaosScoreCounterBase
{
    public ChaosBattleBasicScoreCounter(ChaosLevel chaosLevel) : base(chaosLevel)
    {
    }

    public override int GetEntityScore(NPCEntity npcEntity)
    {
        int score = 0;
        npcEntity.GetInteractions().ForEach(chaosInteraction =>
        {
            bool isSender = (!chaosInteraction.Sender.Equals(npcEntity));
            bool wasTeamMate = chaosInteraction.Sender.GetTeam().Equals(chaosInteraction.Reciver.GetTeam());
            
      
            if (chaosInteraction.InteractionType.Equals(ChaosInteractionType.Heal))
            {
                if (isSender)
                {
                    if (wasTeamMate)
                    {
                        score += 10;
                    }
                    else
                    {
                        score -= 10;
                    }
                }
            }
            else
            {
                if (isSender)
                {
                    if (wasTeamMate)
                    {
                        score -= 10;
                    }
                    else
                    {
                        score += 10;
                    }
                }
            }
        });
        return score;
    }
}
