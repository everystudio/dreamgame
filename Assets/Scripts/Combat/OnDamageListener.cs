using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDamageListener : MonoBehaviour, IDamageable
{
	[SerializeField]
	private Health[] targets;

	[SerializeField]
	private float m_fDelay;

	[SerializeField]
	private UnityEventVector2 onDamaged;

	private UnityEventFloat onHealthChanged;



	private void Awake()
	{
		for (int i = 0; i < targets.Length; i++)
		{
			targets[i].AddListener(this);
		}
	}

	public void OnDamaged(DamageInfo _damageInfo)
	{
		StartCoroutine(DispatchAction(_damageInfo.Causelocation, _damageInfo));
	}
	private IEnumerator DispatchAction(Vector2 _location, DamageInfo _damageInfo)
	{
		yield return (m_fDelay == 0f) ? null : new WaitForSeconds(m_fDelay);
		onDamaged.Invoke(_location);
		onHealthChanged?.Invoke((_damageInfo.MinHP > 0) ? (_damageInfo.MinHP / _damageInfo.MaxHP) : 0);
	}

}
