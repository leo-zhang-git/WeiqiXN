public class DuelPage : UIPageWithBinder<DuelPageUI>
{
    public override string pageName => UIPage.GetPageName<DuelPage>();

    protected override void OnLoaded()
    {
        base.OnLoaded();

        RegisterSystemEvent<OnDuelStateChanged>(OnDuelStateChanged);

        binder.btn_save.onClick.AddListener(OnClickBtnSave);
        binder.btn_exit.onClick.AddListener(OnClickBtnExit);
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        RefreshDebugPanel();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        RefreshDebugPanel();
    }

    public void OnDuelStateChanged(OnDuelStateChanged evt)
    {
        RefreshDebugPanel();
    }

    public void RefreshDebugPanel()
    {
        var mainScene = Global.Instance.sceneManager.mainScene;
        var compDuel = mainScene.GetComponent<SceneComponentDuel>();
        if (compDuel != null && compDuel.duelFSM.isActivated) {
            binder.txt_cur_state.text = compDuel.duelFSM.curState.stateName;
            switch (compDuel.duelFSM.curState.stateName) {
                case DuelStateDefine.STATE_TURN_INPUT:
                    Player player = mainScene.GetEntity<Player>(compDuel.curTurnPlayerGuid.value);
                    if (player != null) {
                        binder.txt_cur_player.text = player.guid;
                        var compDuelInfo = player.GetComponent<ComponentDuelInfo>();
                        if (compDuelInfo != null) {
                            binder.txt_turn_time.text = compDuelInfo.turnLeftTimes.value.ToString();
                        }
                    }
                    break;
            }
        }
    }

    public void OnClickBtnSave()
    {
        EmitSystemEvent(new OnSaveDuelScene());
    }

    public void OnClickBtnExit()
    {
        Global.Instance.sceneManager.EnterMainScene(SceneConfig.MAIN_MENU_SCENE_TYPE_ID, SceneCreateParams.Default);
    }
}