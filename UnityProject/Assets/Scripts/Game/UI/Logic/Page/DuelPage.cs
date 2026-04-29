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

    protected override void OnShow()
    {
        base.OnShow();

        DuelScene scene = Global.Instance.sceneManager.mainScene as DuelScene;
        if (scene != null) {
            if (scene.compDuel.duelFSM.curState != null) {
                binder.txt_cur_state.text = scene.compDuel.duelFSM.curState.stateName;
            }
        }
    }

    public void OnDuelStateChanged(OnDuelStateChanged evt)
    {
        binder.txt_cur_state.text = evt.curStateName;
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