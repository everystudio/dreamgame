using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using Chronos;

public class CombatSystem : StateMachineBase<CombatSystem>
{
	[SerializeField] private EventBool m_combatEnd;

	private EnemyBase m_enemyBattle;

	private void Start()
	{
		SetState(new CombatSystem.Standby(this));
	}

	public void OnStartInt(int _iArg)
	{
		OnStartBattle(null);
	}

	public void OnStartBattle(GameObject _enemy)
	{
		m_enemyBattle = _enemy.GetComponent<EnemyBase>();
		Timekeeper.instance.Clock("Field").localTimeScale = 0f;
		Timekeeper.instance.Clock("Combat").localTimeScale = 1f;
	}
	public void OnEndBattle()
	{
		m_enemyBattle = null;
		Timekeeper.instance.Clock("Field").localTimeScale = 1f;
		Timekeeper.instance.Clock("Combat").localTimeScale = 0f;
		m_combatEnd.Invoke(true);
	}

	public void OnCancelBattle(string _strResult)
	{
		Debug.Log($"CancelBattle:{_strResult}");
		OnEndBattle();

	}

	private class Standby : StateBase<CombatSystem>
	{
		public Standby(CombatSystem _machine) : base(_machine)
		{
		}
	}
}
