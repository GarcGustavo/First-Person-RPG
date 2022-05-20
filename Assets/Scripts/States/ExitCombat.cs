using System.Collections;
using Base_Classes;
using Flockaroo;
using UnityEngine;

namespace States
{
    public class ExitCombat : State
    {
        private GameManager gameManager;
    
        public ExitCombat(GameManager gameManager) : base(gameManager)
        {
            this.gameManager = gameManager;
        }

        public override IEnumerator Enter()
        {
            Debug.Log("Exiting combat!");
        
            var camera = gameManager.GetCamera();
            var vanGogh = camera.GetComponent<VanGoghEffect>();
        
            vanGogh.MasterFade = 0f;
            yield return new WaitForSeconds(2f);
            gameManager.SetState(new Exploring(gameManager));
        }
    }
}
