using System.Collections;

namespace Base_Classes
{
    public abstract class State
    {
        protected readonly GameManager GameManager;
    
        public State(GameManager gameManager)
        {
            GameManager = gameManager;
        }
    
        public virtual IEnumerator Enter()
        {
            yield break;
        }
    
        public virtual IEnumerator CheckState()
        {
            yield break;
        }
    
        public virtual IEnumerator Move()
        {
            yield break;
        }
    
        public virtual IEnumerator Action()
        {
            yield break;
        }
    
        public virtual IEnumerator Heal()
        {
            yield break;
        }
    
        public virtual IEnumerator Attack()
        {
            yield break;
        }
    
        public virtual IEnumerator Pause()
        {
            yield break;
        }
    
    }
}
