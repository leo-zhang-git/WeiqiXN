public abstract class BaseTimer
{
    public string timerId;
    public System.Object owner;

    public BaseTimer(System.Object owner)
    {
        this.owner = owner;
    }

    public abstract void OnTimerStart();

    public abstract void OnTimerUpdate();

    public abstract void OnTimerEnd();
}
