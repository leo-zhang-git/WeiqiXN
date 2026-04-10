public class UserComponentBase : SavableObj
{
    [SkipSavableCheck]
    public User owner;

    public UserComponentBase(User owner)
    {
        this.owner = owner;
        owner.compList.Add(this);
    }

    public virtual void OnDestroy()
    {

    }
}
