using System;
using System.Collections.Generic;

public class ReddotNode
{
    public string reddotKey;
    public int reddotCount;
    public ReddotNode parentNode;
    public Dictionary<string, ReddotNode> childNodes = new Dictionary<string, ReddotNode>();
    public HashSet<Action<int>> countChangeHandlers = new HashSet<Action<int>>();

    public ReddotNode(string reddotKey, ReddotNode parentNode = null)
    {
        this.reddotKey = reddotKey;
        this.parentNode = parentNode;
        if (parentNode != null) {
            parentNode.childNodes.TryAdd(reddotKey, this);
        }
    }

    public void ChangeNodeCount(int newCount)
    {
        if (childNodes.Count > 0) {
            Logger.LogError("Only leaf reddot node is allowed to change count!", ("reddotKey", reddotKey), ("newCount", newCount.ToString()));
            return;
        }
        if (newCount < 0) {
            Logger.LogError("Count value of reddot node should not be nagative!", ("reddotKey", reddotKey), ("newCount", newCount.ToString()));
            return;
        }

        if (reddotCount != newCount) {
            reddotCount = newCount;
            foreach (var handler in countChangeHandlers) {
                try {
                    handler.Invoke(reddotCount);
                }
                catch (Exception ex) {
                    Logger.LogError("Invoke reddot count change handler error.", ("reddotKey", reddotKey), ("err", ex.Message));
                }
            }

            if (parentNode != null) {
                parentNode.UpdateNodeCount();
            }
        }
    }

    public void UpdateNodeCount()
    {
        int newCount = 0;
        foreach (var childNode in childNodes.Values) {
            newCount += childNode.reddotCount;
        }
        reddotCount = newCount;

        foreach (var handler in countChangeHandlers) {
            try {
                handler.Invoke(reddotCount);
            }
            catch (Exception ex) {
                Logger.LogError("Invoke reddot count change error.", ("reddotKey", reddotKey), ("err", ex.Message));
            }
        }

        if (parentNode != null) {
            parentNode.UpdateNodeCount();
        }
    }

    public void ClearNodeCount()
    {
        // 广度便利子节点
        Stack<ReddotNode> stack = new Stack<ReddotNode>();
        foreach (var childNode in childNodes.Values) {
            stack.Push(childNode);
        }

        try {
            while (stack.Count > 0) {
                ReddotNode node = stack.Pop();
                if (node != null) {
                    if (node.childNodes.Count > 0) {
                        foreach (var childNode in node.childNodes.Values) {
                            stack.Push(childNode);
                        }
                    } else {
                        node.ChangeNodeCount(0);
                    }
                }
            }
        }
        catch (Exception ex) {
            Logger.LogError("Clear node count error.", ("reddotKey", reddotKey), ("err", ex.Message));
        }
    }

    public void RegisterReddotHandler(Action<int> handler, bool autoInvokeOnce = true)
    {
        try {
            if (autoInvokeOnce) {
                handler.Invoke(reddotCount);
            }
            countChangeHandlers.Add(handler);
        }
        catch (Exception ex) {
            Logger.LogError("Register reddot handler error.", ("reddotKey", reddotKey), ("err", ex.Message));
        }
    }

    public void UnRegisterReddotHandler(Action<int> handler)
    {
        if (countChangeHandlers.Contains(handler)) {
            countChangeHandlers.Remove(handler);
        }
    }
}
