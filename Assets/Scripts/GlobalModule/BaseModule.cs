public abstract class BaseModule
{
    public abstract void Init();

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void LateUpdate() { }

    public virtual void OnDestroy() { }

    public BaseModule()
    {
        Init();
    }
}