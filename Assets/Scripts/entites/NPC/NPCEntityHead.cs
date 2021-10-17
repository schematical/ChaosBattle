
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCEntityHead: MonoBehaviour, IPointerClickHandler
{
    private NPCEntity _npcEntity;
    public void SetNPCEntity(NPCEntity npcEntity)
    {
        _npcEntity = npcEntity;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.menuManager.chaosEntityDetailPanel.Show(_npcEntity);
    }
}
