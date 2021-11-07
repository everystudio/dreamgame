using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect_BecomeInvulnerable : DamageEffect_Base
{
	protected float m_fInvulnerabilityTime = 0.5f;
	public override void OnDamaged(DamageInfo _info)
	{
		m_health.SetInvulnerable(m_fInvulnerabilityTime);
	}
}
