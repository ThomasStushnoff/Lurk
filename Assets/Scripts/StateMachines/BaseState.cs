namespace StateMachines
{
    public abstract class BaseState<T>
    {
        public string Name;
        protected T Owner;
        
        public BaseState(string name, T owner)
        {
            Name = name;
            Owner = owner;
        }
        
        public virtual void EnterState() { }
        
        public virtual void ExitState() { }
        
        public abstract void UpdateState();
    }
}