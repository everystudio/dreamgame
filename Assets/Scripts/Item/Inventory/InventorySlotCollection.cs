using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotCollection : MonoBehaviour, 
	ILoadItem, 
	IUseItem, 
	IRemoveItem,
	ISelectItem, 
	IInventoryLoaded
{
	private Dictionary<int, InventorySlot> m_slots = new Dictionary<int, InventorySlot>();
	private bool m_bInitialized = false;

    public void OnInventoryLoaded(Inventory _inventory)
    {
        if (!m_bInitialized)
        {
            InventorySlot[] getSlots = GetComponentsInChildren<InventorySlot>(true);

            for (int i = 0; i < getSlots.Length; i++)
            {
                m_slots.Add(getSlots[i].GetSlotIndex(), getSlots[i]);
            }
            m_bInitialized = true;
        }

        foreach(KeyValuePair<int,InventorySlot> kvp in m_slots)
		{
            kvp.Value.OnInventoryInitialized(_inventory);
        }
        /*
        for (int i = 0; i < m_slots.Count; i++)
        {
            Debug.Log(m_slots[0]);
            Debug.Log(m_slots[i]);
            m_slots[i].OnInventoryInitialized(_inventory);
        }
        */
    }

    public void OnItemLoaded(int _iIndex, ItemData _data, int _iAmount)
    {
        if (!_data.HasSlot)
        {
            return;
        }
        GetSlot(_iIndex)?.OnItemLoaded(_iIndex, _data, _iAmount);
    }
    public void OnItemSelect(int _iIndex, bool _bSelected)
    {
        GetSlot(_iIndex)?.OnItemSelect(_iIndex, _bSelected);
    }
    public void OnRemoveItem(int _iIndex)
    {
        GetSlot(_iIndex)?.OnRemoveItem(_iIndex);
    }
    public void OnUseItem(int _iIndex, ItemData _data, int _iAmount, int _iReduce = 1)
    {
        GetSlot(_iIndex)?.OnUseItem(_iIndex, _data, _iAmount, _iReduce);
    }
    private InventorySlot GetSlot(int _iIndex)
    {
        InventorySlot slot;
        m_slots.TryGetValue(_iIndex, out slot);
        return slot;
    }
}
