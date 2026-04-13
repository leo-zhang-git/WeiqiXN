using System;

public abstract class SystemBase
{
    public abstract string systemName { get; }
    private SceneBase scene;

    public SystemBase(SceneBase scene)
    {
        this.scene = scene;
    }

    public static string GetSystemName<TSystem>() where TSystem : SystemBase
    {
        return typeof(TSystem).Name;
    }

    public virtual void Init()
    {

    }

    public virtual void OnUpdate()
    {

    }

    #region Timer
    public void SetSecondTimeout(float targetSeconds, Action timerCB)
    {
        scene.SetSecondTimeout(targetSeconds, timerCB);
    }

    public void SetSecondInterval(float intervalSeconds, Action timerCB, int targetRepeatTimes = -1, float firstDelaySeconds = 0)
    {
        scene.SetSecondInterval(intervalSeconds, timerCB, targetRepeatTimes, firstDelaySeconds);
    }

    public void SetFrameTimeout(int targetFrames, Action timerCB)
    {
        scene.SetFrameTimeout(targetFrames, timerCB);
    }

    public void SetFrameInterval(int intervalFrames, Action timerCB, int targetRepeatTimes = -1, int firstDelayFrames = 0)
    {
        scene.SetFrameInterval(intervalFrames, timerCB, targetRepeatTimes, firstDelayFrames);
    }
    #endregion
}

public abstract class SystemFixed<TScene> : SystemBase where TScene : SceneBase
{
    public TScene scene;

    protected SystemFixed(TScene scene) : base(scene)
    {
        this.scene = scene;
    }
}

