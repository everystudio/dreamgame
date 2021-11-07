using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DamageEffect_Shake : DamageEffect_Base
{
	public float m_fShakePower = 0.5f;
	public override void OnDamaged(DamageInfo _damageInfo)
	{
		transform.DOShakePosition(0.5f, m_fShakePower);
	}
}
