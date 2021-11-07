using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageEffect_Base : MonoBehaviour, IDamageable
{
	protected Health m_health;

	public abstract void OnDamaged(DamageInfo _damageInfo);

	private void Awake()
	{
		m_health = GetComponent<Health>();
		m_health?.AddListener(this);
	}
}
