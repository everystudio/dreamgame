using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using Chronos;

public class CombatSystem : StateMachineBase<CombatSystem>
{
	[SerializeField] private EventBool m_combatEnd;

	[SerializeField] private GameObject m_prefCloud;
	[SerializeField] private GameObject m_objPlayer;

	private EnemyBase m_enemyBattle;
	private GameObject m_combatCloud;

	private Timeline m_timelineCombat;

	private bool m_bCombatCancelRequest;

	private void Start()
	{
		m_timelineCombat = GetComponent<Timeline>();
		if(m_timelineCombat == null)
		{
			m_timelineCombat = gameObject.AddComponent<Timeline>();
			m_timelineCombat.mode = TimelineMode.Global;
			m_timelineCombat.globalClockKey = "Combat";
		}

		SetState(new CombatSystem.Standby(this));
	}

	public void OnStartInt(int _iArg)
	{
		OnStartBattle(null);
	}

	public void OnStartBattle(GameObject _enemy)
	{
		if (m_enemyBattle == null)
		{
			//Debug.Log(_enemy);
			//Debug.Log(_enemy.name);
			m_enemyBattle = _enemy.GetComponent<EnemyBase>();
			//Debug.Log(m_enemyBattle);
			SetState(new CombatSystem.FightingBegin(this, m_enemyBattle));
		}
		else
		{
			Debug.LogError($"now fighting:{m_enemyBattle.name}");
		}
	}
	public void OnEndBattle()
	{
		if(m_enemyBattle != null) {
			SetState(new CombatSystem.Standby(this));
		}
		else
		{
			Debug.LogError($"not fighting");
		}
	}

	public void OnCancelBattle(string _strResult)
	{
		Debug.Log($"CancelBattle:{_strResult}");
		if (m_enemyBattle != null)
		{
			m_bCombatCancelRequest = true;
		}
		else
		{
			Debug.Log("not fighting");
		}
	}

	private class Standby : StateBase<CombatSystem>
	{
		public Standby(CombatSystem _machine) : base(_machine)
		{
		}
		public override void OnEnterState()
		{
			base.OnEnterState();
			machine.m_bCombatCancelRequest = false;
			machine.m_enemyBattle = null;
			Timekeeper.instance.Clock("Field").localTimeScale = 1f;
			Timekeeper.instance.Clock("Combat").localTimeScale = 0f;
			if (machine.m_combatCloud != null)
			{
				Destroy(machine.m_combatCloud);
			}
		}
	}

	private class FightingBegin : StateBase<CombatSystem>
	{
		private EnemyBase enemyBattle;
		private float m_fTimer;

		public FightingBegin(CombatSystem _combatSystem, EnemyBase _enemyBattle):base(_combatSystem)
		{
			m_fTimer = 0f;
			enemyBattle = _enemyBattle;
		}
		public override void OnEnterState()
		{
			base.OnEnterState();
			Timekeeper.instance.Clock("Field").localTimeScale = 0f;
			Timekeeper.instance.Clock("Combat").localTimeScale = 1f;

			if (machine.m_combatCloud == null)
			{
				machine.m_combatCloud = Instantiate(machine.m_prefCloud);
				machine.m_combatCloud.transform.position = enemyBattle.transform.position;
			}
		}
		public override void OnUpdateState()
		{
			// Ç±ÇÃèàóùÇ¢ÇÁÇÒÇ»
			base.OnUpdateState();
			m_fTimer += machine.m_timelineCombat.deltaTime;
			if( 0f < m_fTimer)
			{
				machine.SetState(new CombatSystem.FightTurnStart(machine));
			}
		}
		public override void OnExitState()
		{
			base.OnExitState();
		}
	}

	private class FightTurnStart : StateBase<CombatSystem>
	{
		private float m_fTimer;
		public FightTurnStart(CombatSystem _machine) : base(_machine)
		{
			m_fTimer = 0f;
		}

		public override void OnEnterState()
		{
			base.OnEnterState();
		}
		public override void OnUpdateState()
		{
			base.OnUpdateState();
			m_fTimer += machine.m_timelineCombat.deltaTime;
			if (0.5f < m_fTimer)
			{
				machine.SetState(new CombatSystem.FightTurnPlayer(machine));
			}
		}
	}

	private class FightTurnPlayer : StateBase<CombatSystem>
	{
		public FightTurnPlayer(CombatSystem _machine) : base(_machine)
		{
		}
		public override IEnumerator OnEnterStateEnumerator()
		{
			yield return new WaitForSeconds(1f);

			machine.m_enemyBattle.GetComponent<Health>().Damage(
				1f,
				machine.m_enemyBattle.transform.position,
				machine.m_objPlayer
				);

			yield return new WaitForSeconds(1f);

			if (machine.m_enemyBattle.GetComponent<Health>().IsDead)
			{
				machine.SetState(new CombatSystem.FightingEnd(machine));
			}
			else
			{
				machine.SetState(new CombatSystem.FightTurnEnemy(machine));
			}
		}
	}

	private class FightTurnEnemy : StateBase<CombatSystem>
	{
		public FightTurnEnemy(CombatSystem _machine) : base(_machine){}
		public override IEnumerator OnEnterStateEnumerator()
		{
			yield return new WaitForSeconds(1f);

			Health playerHealth = machine.m_objPlayer.GetComponent<Health>();

			playerHealth.Damage(
				1f,
				machine.m_objPlayer.transform.position,
				machine.m_enemyBattle.gameObject
				);

			yield return new WaitForSeconds(1f);

			if (playerHealth.IsDead)
			{
				machine.SetState(new CombatSystem.FightingEnd(machine));
			}
			else
			{
				machine.SetState(new CombatSystem.FightTurnEnd(machine));
			}
		}
	}

	private class FightTurnEnd : StateBase<CombatSystem>
	{
		public FightTurnEnd(CombatSystem _machine) : base(_machine)
		{
		}

		public override void OnEnterState()
		{
			base.OnEnterState();
			if (machine.m_bCombatCancelRequest)
			{
				machine.OnEndBattle();
			}
			else
			{
				machine.SetState(new CombatSystem.FightTurnStart(machine));
			}
		}
	}
	private class FightingEnd : StateBase<CombatSystem>
	{
		public FightingEnd(CombatSystem _machine) : base(_machine)
		{
		}

		public override void OnEnterState()
		{
			base.OnEnterState();
			machine.m_combatEnd.Invoke(true);
			machine.OnEndBattle();
		}
	}


}
