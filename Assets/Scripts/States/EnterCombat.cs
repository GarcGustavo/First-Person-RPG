using System.Collections;
using Base_Classes;
using Flockaroo;
using PlayerComponents;
using Scriptable_Objects;
using UnityEngine;

namespace States
{
    public class EnterCombat : State
    {
        private GameManager gameManager;
        private Camera camera;
        private VanGoghEffect vanGogh;
        private Player player;
        private PlayerData playerData;

        public EnterCombat(GameManager gameManager) : base(gameManager)
        {
            this.gameManager = gameManager;
            camera = gameManager.GetCamera();
            vanGogh = camera.GetComponent<VanGoghEffect>();
        }

        public override IEnumerator Enter()
        {
            Debug.Log("Entered combat!");
            player = gameManager.GetPlayer();
            playerData = gameManager.GetPlayerData();
            var enemy = gameManager.GetEnemies();
        
            //player._touchedEnemy = false;
            //player._canMove = false;

            //vanGogh.MasterFade = 0.9f;
        
            yield return new WaitForSeconds(2f);

            // if (playerData.speed >= enemy[0].Speed)
            // {
            //     Debug.Log("Player moves first!");
            //     gameManager.SetState(new PlayerTurn(gameManager));
            // }
            // else
            // {
            //     Debug.Log("Enemy moves first!");
            //     gameManager.SetState(new EnemyTurn(gameManager));
            // }
        }

        public override IEnumerator CheckState()
        {
            Debug.Log("Checking enter combat state!");
            //TODO: Tie into player sanity logic
            //var _sanity = gameManager.playerData.health;
            vanGogh.MasterFade += (vanGogh.MasterFade>2)?0:Mathf.Lerp(0, 1f,Time.deltaTime);
            yield break;
        }
    }
}
