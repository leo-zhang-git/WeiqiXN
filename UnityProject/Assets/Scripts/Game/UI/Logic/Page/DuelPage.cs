public class DuelPage : UIPageWithBinder<DuelPageUI>
{
    public override string pageName => UIPage.GetPageName<DuelPage>();

    protected override void OnLoaded()
    {
        base.OnLoaded();

        binder.btn_save.onClick.AddListener(OnClickBtnSave);
        binder.btn_exit.onClick.AddListener(OnClickBtnExit);
    }

    public void OnClickBtnSave()
    {
        EmitSystemEvent(new OnSaveDuelScene());
    }

    public void OnClickBtnExit()
    {
        Global.Instance.sceneManager.EnterMainScene(SceneConfig.MAIN_MENU_SCENE_TYPE_ID);
    }
}