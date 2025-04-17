using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryItem itemData; //슬롯에 담긴 아이템
    public Image iconImage; //아이템 아이콘 이미지
    private Transform originalParent; //원래 부모 위치 기억

    public void Setup(InventoryItem item) //슬롯 세팅(아이템 연결)
    {
        itemData = item;
        iconImage.sprite = item.icon;
        iconImage.enabled = true;
    }

    public void OnBeginDrag(PointerEventData eventData) //드래그 시작 시 호출
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root); // UI 맨 위로 올림
    }

    public void OnDrag(PointerEventData eventData) //드래그 중 마우스 따라다님
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) //드래그 끝났을 때
    {
        if (eventData.pointerEnter != null) //드롭 위치가 장비슬롯인지 확인
        {
            EquipmentSlot slot = eventData.pointerEnter.GetComponent<EquipmentSlot>();
            if (slot != null)
            {
                slot.Equals(itemData); //장비장착
                Destroy(gameObject); //인벤토리 슬롯 제거
                return;
            }
        }

        //실패 시 원래 위치로
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
    }

}