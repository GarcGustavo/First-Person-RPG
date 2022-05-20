using System.Collections;
using Base_Classes;

namespace States
{
    public class PlayerTurn : State
    {
        public PlayerTurn(GameManager gameManager) : base(gameManager)
        {
        
        }

        public override IEnumerator Enter()
        {
            return base.Enter();
        }

        public override IEnumerator Move()
        {
            return base.Move();
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
