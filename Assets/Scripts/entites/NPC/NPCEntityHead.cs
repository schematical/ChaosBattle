
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCEntityHead: MonoBehaviour, IPointerClickHandler
{
    private ChaosNPCEntity _chaosNpcEntity;
    public void SetNPCEntity(ChaosNPCEntity chaosNpcEntity)
    {
        _chaosNpcEntity = chaosNpcEntity;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.menuManager.chaosEntityDetailPanel.Show(_chaosNpcEntity);
    }
}
