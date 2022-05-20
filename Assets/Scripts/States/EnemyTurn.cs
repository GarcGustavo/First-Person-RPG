using System.Collections;
using System.Collections.Generic;
using Base_Classes;
using PlayerComponents;
using UnityEngine;

namespace States
{
	public class EnemyTurn : State
	{
		private GameManager gameManager;
		private Player _player;
		private List<Enemy> _enemies;
		public EnemyTurn(GameManager gameManager) : base(gameManager)
		{
			this.gameManager = gameManager;
			_player = this.gameManager.GetPlayer();
			_enemies = this.gameManager.GetEnemies();
		}

		public override IEnumerator Enter()
		{
			Debug.Log("Enemy Turn");
			yield break;
		}

		public override IEnumerator CheckState()
		{
			Debug.Log("Enemy Turn");

			// if (gameManager.GetPlayerData().health <= 0 || _enemies[0].Health <= 0)
			// {
			// 	gameManager.SetState(new ExitCombat(gameManager));
			// }
		
			yield break;
		}

		public override IEnumerator Action()
		{
			return base.Action();
		}
    
		public override IEnumerator Attack()
		{
			yield break;
		}
    
		public override IEnumerator Heal()
		{
			yield break;
		}

		public override IEnumerator Pause()
		{
			return base.Pause();
		}
	}
}
