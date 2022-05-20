using System;
using UnityEngine;

namespace Base_Classes
{
	public class Exit : GridUnit
	{
		private GameManager _localManager;

		private void Awake()
		{
			_localManager = GameManager.GetInstance();
		}

		private void LookAtPlayer()
		{
			
		}

		private void LoadNextFloor()
		{
			_localManager.endRound.Invoke();
		}
		public void InitializeUnit()
		{
			//Debug.Log("Initializing unit " + name);
		}
	}
}