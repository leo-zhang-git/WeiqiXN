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
        DuelScene scene = Global.Instance.sceneManager.mainScene as DuelScene;
        if (scene != null) {
            if (scene.compDuel.duelFSM.isActivated) {
                binder.txt_cur_state.text = scene.compDuel.duelFSM.curState.stateName;
                switch (scene.compDuel.duelFSM.curState.stateName) {
                    case DuelStateDefine.STATE_TURN_INPUT:
                        Player player = scene.GetEntity<Player>(scene.compDuel.curTurnPlayerGuid.value);
                        if (player != null) {
                            binder.txt_cur_player.text = player.guid;
                            binder.txt_turn_time.text = player.compDuelInfo.turnLeftTimes.value.ToString();
                        }
                        break;
                }
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