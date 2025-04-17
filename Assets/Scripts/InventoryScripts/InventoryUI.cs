using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public GameObject itemListPanel; //아이템 목록이 나올 패널
    public GameObject itemSlotPrefab; //슬롯 프리팹(ItemSlot.cs 포함)
    public EquipmentSlot weaponSlot; //무기 장비 슬롯


    private void Awake()
    {
        Instance = this;
    }

    public void UpdateInventoryUI() //인벤토리 UI 갱신
    {
        foreach (Transform child in itemListPanel.transform) //기존 슬롯 제거
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem item in InventoryManager.Instance.inventory) //인벤토리 아이템을 슬롯으로 생성
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, itemListPanel.transform);
            ItemSlot slot = slotObj.GetComponent<ItemSlot>();
            slot.Setup(item);
        }
    }
}