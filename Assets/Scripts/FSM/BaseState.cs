using Interfaces;

namespace FSM
{
    public abstract class BaseState
    {
        public string name;
        public IEntity entity;
        public BaseState(string name, IEntity entity)
        {
            this.name = name;
            this.entity = entity;
        }
        
        public virtual void Enter() { }
        public void Exit() { }
        public void Update() { }
        public void FixedUpdate() { }
    }
}