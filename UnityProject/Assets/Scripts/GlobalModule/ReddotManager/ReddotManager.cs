using System;
using System.Collections.Generic;

public class ReddotManager : ModuleBase
{
    public Dictionary<string, ReddotNode> nodeDict = new Dictionary<string, ReddotNode>();

    public override void Init()
    {

    }

    public ReddotNode CreateReddotNode(string reddotKey)
    {
        string[] keys = reddotKey.Split(ReddotConst.ReddotKeySpliter);
        string key = keys[0];
        ReddotNode parent = null;

        for (int i = 0; i < keys.Length - 1; i++) {
            ReddotNode node;
            if (!nodeDict.TryGetValue(key, out node)) {
                node = new ReddotNode(key, parent);
                nodeDict.TryAdd(key, node);
            }
            parent = node;
            key = ReddotConst.ConcatReddotKey(new string[] { keys[i], keys[i + 1] });
        }

        ReddotNode newNode = new ReddotNode(reddotKey, parent);
        nodeDict.TryAdd(reddotKey, newNode);
        return newNode;
    }

    public void RegisterReddotHandler(string reddotKey, Action<int> handler, bool autoInvokeOnce = false)
    {
        ReddotNode node;
        if (!nodeDict.TryGetValue(reddotKey, out node)) {
            node = CreateReddotNode(reddotKey);
        }
        node.RegisterReddotHandler(handler, autoInvokeOnce);
    }

    public void UnRegisterReddotHandler(string reddotKey, Action<int> handler)
    {
        if (nodeDict.TryGetValue(reddotKey, out ReddotNode node)) {
            node.UnRegisterReddotHandler(handler);
        }
    }

    public void ChangeReddotCount(string reddotKey, int newCount)
    {
        if (nodeDict.TryGetValue(reddotKey, out ReddotNode node)) {
            node.ChangeNodeCount(newCount);
        }
    }

    public int GetReddotCount(string reddotKey)
    {
        ReddotNode node;
        if (!nodeDict.TryGetValue(reddotKey, out node)) {
            node = CreateReddotNode(reddotKey);
        }
        return node.reddotCount;
    }

    public void RemoveReddotNode(string reddotKey)
    {
        try {
            if (nodeDict.TryGetValue(reddotKey, out ReddotNode node)) {
                if (node.childNodes.Count == 0) {
                    ChangeReddotCount(reddotKey, 0);
                } else {
                    foreach (var childKV in node.childNodes) {
                        RemoveReddotNode(childKV.Key);
                    }
                }
                nodeDict.Remove(reddotKey);
            }
        }
        catch (Exception ex) {
            Logger.LogError("Remove reddot node error.", ("reddotKey", reddotKey), ("err", ex.Message));
        }
    }

    public void ClearReddotNode(string reddotKey)
    {
        if (nodeDict.TryGetValue(reddotKey, out ReddotNode node)) {
            node.ClearNodeCount();
        }
    }

    public void ClearAllNodes()
    {
        nodeDict.Clear();
    }
}
