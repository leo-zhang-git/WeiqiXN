using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UILogicBase : ITimerAttacher, IEventReceiver, IResourceLoadBinder
{
    public bool isLoaded;
    private GameObject _gameObject;
    public GameObject gameObject
    {
        get
        {
            if (!isLoaded) {
                Logger.LogError("Try get gameObject before ui resource is loaded", ("uiClsName", GetType().Name));
                return null;
            }
            return _gameObject;
        }
    }

    public virtual void onUnityResourceLoaded(GameObject uiGameObject)
    {
        this._gameObject = uiGameObject;
    }

    protected virtual void OnLoaded()
    {

    }

    protected virtual void OnOpen()
    {

    }

    protected virtual void OnShow()
    {

    }

    protected virtual void OnHide()
    {

    }

    protected virtual void OnClose()
    {
        OnTimerAttacherDestroyed();
        OnEventReceiverDestroyed();
        OnResourceBinderDestroyed();
    }

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

    public void OnEventReceiverDestroyed()
    {
        Global.Instance.eventManager.UnregisterEventsByReceiver(this);
    }
    #endregion

    #region Resource
    private string _binderId;
    public string binderId
    {
        get
        {
            if (string.IsNullOrEmpty(_binderId)) {
                _binderId = $"{GetType().Name}_{ResourceManager.ResourceBinderInstanceIds}";
                ResourceManager.ResourceBinderInstanceIds += 1;
            }
            return _binderId;
        }
    }

    private HashSet<string> _loadHandlerIds = new HashSet<string>();
    public HashSet<string> loadHandlerIds => _loadHandlerIds;

    public void OnResourceBinderDestroyed()
    {
        Global.Instance.resourceManager.OnResourceBinderDestroyed(binderId);
    }
    #endregion
}
