using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using Chronos;

public class CombatSystem : StateMachineBase<CombatSystem>
{
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
		Timekeeper.instance.Clock("Field").localTimeScale = 0f;
		Timekeeper.instance.Clock("Combat").localTimeScale = 1f;
	}
	public void OnEndBattle(GameObject _enemy)
	{
		Timekeeper.instance.Clock("Field").localTimeScale = 1f;
		Timekeeper.instance.Clock("Combat").localTimeScale = 0f;
	}

	private class Standby : StateBase<CombatSystem>
	{
		public Standby(CombatSystem _machine) : base(_machine)
		{
		}
	}
}
