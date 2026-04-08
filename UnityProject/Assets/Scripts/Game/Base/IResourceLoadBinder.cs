using System.Collections.Generic;

public interface IResourceLoadBinder
{
    public string binderId { get; }
    public HashSet<string> loadHandlerIds { get; }
    public void OnResourceBinderDestroyed();
}