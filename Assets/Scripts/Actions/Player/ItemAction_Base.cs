using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemAction_Base : ScriptableObject
{
	public abstract IEnumerator UseAction();
}
