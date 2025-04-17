using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public string slotType; //슬롯에 장착 가능한 타입
    public Image iconImage; //슬롯에 표시될 아이템 아이콘
    private InventoryItem equippedItem; //현재 장착된 아이템

    public void Equip(InventoryItem item) //아이템 장착 함수
    {
        if (item.itemType == ItemType.Weapon)
        {
            equippedItem = item;
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }
    }

    public InventoryItem GetEquippedItem() //장착된 아이템 반환
    {
        return equippedItem;
    }
}