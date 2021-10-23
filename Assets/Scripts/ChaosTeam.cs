
using System;
using System.Collections.Generic;

public class ChaosTeam
{
    public List<ChaosNPCEntity> entities;
    public String name;
    public ChaosTeam(String name)
    {
        this.name = name;
    }

    public void AttachEntity(ChaosNPCEntity chaosNpcEntity)
    {
        entities.Add(chaosNpcEntity);
    } 
}
