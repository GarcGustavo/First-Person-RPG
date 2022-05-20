using System.Collections;
using Base_Classes;
using PlayerComponents;
using Scriptable_Objects;
using UnityEngine;

namespace States
{
    public class Exploring : State
    {
        private GameManager _gameManager;
        private Player _player;
        private PlayerData _playerData;
    
        public Exploring(GameManager gameManager) : base(gameManager)
        {
            _gameManager = gameManager;
        }

        public override IEnumerator Enter()
        {
            //Debug.Log("Explore state");
            _player = _gameManager.GetPlayer();
            _playerData = _gameManager.GetPlayerData();
            if (_playerData.spawned)
            {
                //playerData.touchedEnemy = false;
                //_player.SetMovement(true);
            }
            yield break;
        }

        public override IEnumerator CheckState()
        {
            //if (!_player._touchedEnemy) yield break;
            //TODO: Fix player collision not triggering setstate
            Debug.Log("Stopping!");
            _gameManager.SetState(new EnterCombat(_gameManager));

            yield break;
        }
    
        public override IEnumerator Action()
        {
            return base.Action();
        }

        public override IEnumerator Attack()
        {
            return base.Attack();
        }

        public override IEnumerator Heal()
        {
            return base.Heal();
        }

        public override IEnumerator Pause()
        {
            return base.Pause();
        }
    
    }
}
