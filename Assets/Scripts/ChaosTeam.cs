
using System;
using System.Collections.Generic;

public class ChaosTeam
{
    public List<NPCEntity> entities;
    public String name;
    public ChaosTeam(String name)
    {
        this.name = name;
    }

    public void AttachEntity(NPCEntity npcEntity)
    {
        entities.Add(npcEntity);
    } 
}
