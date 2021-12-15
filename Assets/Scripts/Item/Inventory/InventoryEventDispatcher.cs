using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryEventDispatcher
{
    private List<IInventoryLoaded> m_inventoryLoadInterfaces = new List<IInventoryLoaded>();
    private List<ILoadItem> m_loadInterfaces = new List<ILoadItem>();
    private List<ISelectItem> m_selectInterfaces = new List<ISelectItem>();
    private List<IMoveItem> m_moveInterfaces = new List<IMoveItem>();
    private List<IUseItem> m_useInterfaces = new List<IUseItem>();
    private List<IRemoveItem> m_removeInterfaces = new List<IRemoveItem>();
    private List<IDropItem> m_dropInterfaces = new List<IDropItem>();

    public InventoryEventDispatcher(GameObject _target, Inventory _inventory)
    {
        AssignInterfaces<IInventoryLoaded>(ref m_inventoryLoadInterfaces, _target);
        AssignInterfaces<ILoadItem>(ref m_loadInterfaces, _target);
        AssignInterfaces<ISelectItem>(ref m_selectInterfaces, _target);
        AssignInterfaces<IMoveItem>(ref m_moveInterfaces, _target);
        AssignInterfaces<IUseItem>(ref m_useInterfaces, _target);
        AssignInterfaces<IRemoveItem>(ref m_removeInterfaces, _target);
        AssignInterfaces<IDropItem>(ref m_dropInterfaces, _target);

        for (int i = 0; i < m_inventoryLoadInterfaces.Count; i++)
        {
            m_inventoryLoadInterfaces[i].OnInventoryLoaded(_inventory);
        }

        for (int i = 0; i < m_loadInterfaces.Count; i++)
        {
            foreach (KeyValuePair<int, InventoryItem> item in _inventory.Items)
            {
                m_loadInterfaces[i].OnItemLoaded(item.Key, item.Value.Data, item.Value.Amount);
            }
        }
    }

    private void AssignInterfaces<T>(ref List<T> _list, GameObject _targetGameObject)
    {
        List<T> getInterfaces = new List<T>();
        _targetGameObject.GetComponentsInChildren<T>(true, getInterfaces);

        if (getInterfaces.Count > 0)
        {
            _list.AddRange(getInterfaces);
        }
    }

    public void DispatchInventoryInitialized(Inventory _inventory)
    {
        int count = m_inventoryLoadInterfaces.Count;

        for (int i = 0; i < count; i++)
        {
            m_inventoryLoadInterfaces[i].OnInventoryLoaded(_inventory);
        }
    }
    public void DispatchItemLoad(int _iSlotIndex, ItemData _data, int _iAmount)
    {
        int count = m_loadInterfaces.Count;

        for (int i = 0; i < count; i++)
        {
            m_loadInterfaces[i].OnItemLoaded(_iSlotIndex, _data, _iAmount);
        }
    }

    public void DispatchSelectItem(int _iIndex, bool _bSelected)
    {
        int count = m_selectInterfaces.Count;

        for (int i = 0; i < count; i++)
        {
            m_selectInterfaces[i].OnItemSelect(_iIndex, _bSelected);
        }
    }
    public void DispatchMoveItem(int _iFromIndex, int _iToIndex)
    {
        int count = m_moveInterfaces.Count;

        for (int i = 0; i < count; i++)
        {
            m_moveInterfaces[i].OnMoveItem(_iFromIndex, _iToIndex);
        }
    }
    public void DispatchRemoveItem(int _iIndex)
    {
        int count = m_removeInterfaces.Count;

        for (int i = 0; i < count; i++)
        {
            m_removeInterfaces[i].OnRemoveItem(_iIndex);
        }
    }
    public void DispatchDropItem(int _iIndex, LootableItem _lootable)
    {
        foreach( IDropItem drop in m_dropInterfaces)
		{
            drop.OnDropItem(_iIndex, _lootable);
        }
    }


}
