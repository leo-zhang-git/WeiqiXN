using System;
using System.Collections.Generic;

public class SceneBase : ITimerAttacher, IEventReceiver
{
    public readonly SceneDataType configData;
    public bool isLoaded;
    public Dictionary<string, EntityBase> entityDict = new Dictionary<string, EntityBase>();
    public Dictionary<string, HashSet<EntityBase>> entityTypeDict = new Dictionary<string, HashSet<EntityBase>>();
    public List<SceneComponentBase> compList = new List<SceneComponentBase>();

    protected UnityEngine.SceneManagement.Scene unityScene;
    private List<SystemBase> systemList = new List<SystemBase>();
    private HashSet<string> systemNames = new HashSet<string>();

    public SceneBase(SceneDataType configData)
    {
        this.configData = configData;
    }

    #region LifeCycle
    public void OnUnitySceneLoaded(UnityEngine.SceneManagement.Scene unityScene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        this.unityScene = unityScene;
        isLoaded = true;
        OnSceneLoaded();
        Logger.LogInfo("Unity scene load success.", ("sceneTypeId", configData.id), ("unitySceneName", unityScene.name));
    }

    public virtual void OnSceneLoaded()
    {

    }

    public virtual void OnSceneInit()
    {
        // Add systems
    }

    protected virtual void OnUpdate()
    {
        foreach (var system in systemList) {
            system.OnUpdate();
        }
    }

    public virtual void OnSceneExit()
    {
        foreach (var entity in entityDict.Values) {
            entity.Destroy();
        }
        foreach (var comp in compList) {
            comp.OnDestroy();
        }

        if (!isLoaded) {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnUnitySceneLoaded;
        }
        OnTimerAttacherDestroyed();
        OnEventReceiverDestroyed();
    }

    public void Update()
    {
        if (!isLoaded) {
            return;
        }

        OnUpdate();
    }
    #endregion

    #region Timer
    private List<string> _attachedTimerIds = new List<string>();
    public List<string> attachedTimerIds => _attachedTimerIds;

    public void SetSecondTimeout(float targetSeconds, Action timerCB)
    {
        Global.Instance.timerManager.SetSecondTimeout(this, targetSeconds, timerCB);
    }

    public void SetSecondInterval(float intervalSeconds, Action timerCB, int targetRepeatTimes = -1, float firstDelaySeconds = 0)
    {
        Global.Instance.timerManager.SetSecondInterval(this, intervalSeconds, timerCB, targetRepeatTimes, firstDelaySeconds);
    }

    public void SetFrameTimeout(int targetFrames, Action timerCB)
    {
        Global.Instance.timerManager.SetFrameTimeout(this, targetFrames, timerCB);
    }

    public void SetFrameInterval(int intervalFrames, Action timerCB, int targetRepeatTimes = -1, int firstDelayFrames = 0)
    {
        Global.Instance.timerManager.SetFrameInterval(this, intervalFrames, timerCB, targetRepeatTimes, firstDelayFrames);
    }

    public void OnTimerAttacherDestroyed()
    {
        Global.Instance.timerManager.RemoveTimersByAttacher(this);
    }
    #endregion

    #region Event
    private List<ISystemEventHandler> _registeredSystemEventHandlers = new List<ISystemEventHandler>();
    private List<IEntityEventHandler> _registeredEntityEventHandlers = new List<IEntityEventHandler>();
    public List<ISystemEventHandler> registeredSystemEventHandlers => _registeredSystemEventHandlers;
    public List<IEntityEventHandler> registeredEntityEventHandlers => _registeredEntityEventHandlers;

    public void EmitSystemEvent<TEvent>(TEvent systemEvent) where TEvent : SystemEventBase
    {
        Global.Instance.eventManager.EmitSystemEvent(systemEvent);
    }

    public void RegisterSystemEvent<TEvent>(Action<TEvent> eventCB) where TEvent : SystemEventBase
    {
        Global.Instance.eventManager.RegisterSystemEvent(this, eventCB);
    }

    public void UnregisterSystemEvent(ISystemEventHandler handler)
    {
        Global.Instance.eventManager.UnregisterSystemEvent(handler);
    }

    public void EmitEntityEvent<TEntity, TEvent>(TEntity entity, TEvent entityEvent) where TEntity : EntityBase where TEvent : EntityEventBase
    {
        Global.Instance.eventManager.EmitEntityEvent(entity, entityEvent);
    }

    public void RegisterEntityEvent<TEntity, TEvent>(Action<TEntity, TEvent> eventCB) where TEntity : EntityBase where TEvent : EntityEventBase
    {
        Global.Instance.eventManager.RegisterEntityEvent(this, eventCB);
    }

    public void UnregisterEntityEvent(IEntityEventHandler handler)
    {
        Global.Instance.eventManager.UnregisterEntityEvent(handler);
    }

    public void OnEventReceiverDestroyed()
    {
        Global.Instance.eventManager.UnregisterEventsByReceiver(this);
    }
    #endregion

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnUnitySceneLoaded;
        try {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(configData.unitySceneName);
            Logger.LogInfo("Load scene async start.", ("sceneTypeId", configData.id), ("unitySceneName", configData.unitySceneName));
        }
        catch (Exception ex) {
            Logger.LogError("Load unity scene async error.", ("unitySceneName", configData.unitySceneName), ("exception", ex.Message));
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnUnitySceneLoaded;
        }
    }

    protected void AddSystem(SystemBase system)
    {
        if (systemNames.Contains(system.systemName)) {
            Logger.LogError("Duplicated system add to same scene. systemName:{system.systemName}", ("systemName", system.systemName));
            return;
        }
        systemList.Add(system);
        systemNames.Add(system.systemName);
    }

    public void AddEntity(EntityBase entity)
    {
        if (entityDict.ContainsKey(entity.guid)) {
            Logger.LogError("Duplicated entity guid, add entity failed.", ("guid", entity.guid));
            return;
        }
        entityDict[entity.guid] = entity;

        HashSet<EntityBase> entSet;
        if (!entityTypeDict.TryGetValue(entity.GetEntityType(), out entSet)) {
            entSet = new HashSet<EntityBase>();
            entityTypeDict[entity.GetEntityType()] = entSet;
        }
        entSet.Add(entity);
        Logger.LogInfo("Add entity success.", ("guid", entity.guid));
    }

    public void RemoveEntity(EntityBase entity)
    {
        if (!entityDict.ContainsKey(entity.guid)) {
            Logger.LogError("Target entity not in scene, remove entity failed.", ("guid", entity.guid), ("sceneTypeId", configData.id));
            return;
        }
        entityDict.Remove(entity.guid);

        HashSet<EntityBase> entSet;
        if (entityTypeDict.TryGetValue(entity.GetEntityType(), out entSet)) {
            entSet.Remove(entity);
        }
        Logger.LogInfo("Remove entity success.", ("guid", entity.guid), ("sceneTypeId", configData.id));
    }

    public EntityBase GetEntity(string guid)
    {
        if (entityDict.TryGetValue(guid, out var entity)) {
            return entity;
        }

        return null;
    }

    public TEntity GetEntity<TEntity>(string guid) where TEntity : EntityBase
    {
        if (entityDict.TryGetValue(guid, out var entity)) {
            if (entity.GetEntityType() == EntityBase.GetEntityType<TEntity>()) {
                return (TEntity)entity;
            }
        }

        return null;
    }
}
