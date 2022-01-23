using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using DG.Tweening;

public class CombatHandler : StateMachineBase<CombatHandler>
{
	private bool m_bEnableBattle;
	[SerializeField] private EventGameObject m_combatBegin;

	private void Start()
	{
		SetState(new Standby(this));
	}

	private void OnCollisionEnter2D(Collision2D _collision)
	{
		if (m_bEnableBattle && _collision.gameObject.CompareTag("Enemy"))
		{
			//Debug.Log("hit_enemy");
			m_combatBegin.Invoke(_collision.gameObject);
		}
	}


	public void OnEndCombat()
	{
		m_bEnableBattle = false;
		SetState(new Waiting(this));
	}

	private class Standby : StateBase<CombatHandler>
	{
		public Standby(CombatHandler _machine) : base(_machine)
		{
		}
		public override void OnEnterState()
		{
			machine.m_bEnableBattle = true;
		}
	}

	private class Waiting : StateBase<CombatHandler>
	{
		public Waiting(CombatHandler _machine) : base(_machine)
		{
		}
		public override void OnEnterState()
		{
			base.OnEnterState();
			machine.m_bEnableBattle = false;

			SpriteRenderer r = machine.GetComponent<SpriteRenderer>();
			if (r != null)
			{
				r.DOFade(0f, 0.1f).SetLoops(10, LoopType.Yoyo).OnComplete(() => {
					machine.SetState(new Standby(machine));
				});
			}
			else
			{
				machine.SetState(new Standby(machine));
			}
		}

	}
}
