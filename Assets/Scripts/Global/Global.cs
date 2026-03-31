using System.Collections.Generic;

public class Global
{
    public static Global _instance;
    public static Global Instance
    {
        get
        {
            if (Global._instance == null) {
                Global._instance = new Global();
            }
            return Global._instance;
        }
    }
    public List<ModuleBase> moduleList = new List<ModuleBase>();

    public ResourceManager resourceManager;
    public TimerManager timerManager;
    public EventManager eventManager;
    public UIManager uiManager;
    public SceneManager sceneManager;

    public void Start()
    {
        resourceManager = new ResourceManager();
        moduleList.Add(resourceManager);

        timerManager = new TimerManager();
        moduleList.Add(timerManager);

        eventManager = new EventManager();
        moduleList.Add(eventManager);

        uiManager = new UIManager();
        moduleList.Add(uiManager);

        sceneManager = new SceneManager();
        moduleList.Add(sceneManager);
    }

    public void Update()
    {
        foreach (var module in moduleList) {
            module.Update();
        }
    }

    public void FixedUpdate()
    {
        foreach (var module in moduleList) {
            module.FixedUpdate();
        }
    }

    public void LateUpdate()
    {
        foreach (var module in moduleList) {
            module.LateUpdate();
        }
    }

    public void Destroy()
    {
        foreach (var module in moduleList) {
            module.OnDestroy();
        }
    }
}