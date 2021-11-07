using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect_BecomeInvulnerable : MonoBehaviour,IDamageable
{
	protected float m_fInvulnerabilityTime = 0.5f;
	private Health m_health;
	private void Awake()
	{
		m_health = GetComponent<Health>();
		m_health?.AddListener(this);
	}
	public void OnDamaged(DamageInfo _info)
	{
		m_health.SetInvulnerable(m_fInvulnerabilityTime);
	}
}
