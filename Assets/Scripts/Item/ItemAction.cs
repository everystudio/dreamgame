using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemAction : ScriptableObject
{
	public virtual void ItemAcquisitionAction(Inventory _userInventory, int _iItemIndex) { }
	public virtual void ItemActiveAction(Inventory _userInventory, int _iItemIndex) { }
	public virtual void ItemUnactiveAction(Inventory _userInventory, int _iItemIndex) { }
    public virtual void ItemRemoveAction(Inventory _userInventory, int _iItemIndex) { }
}
