public class DuelSetupPopup : UIPageWithBinder<DuelSetupPopupUI>
{
    public override string pageName => UIPage.GetPageName<DuelSetupPopup>();

    protected override void OnLoaded()
    {
        base.OnLoaded();

        binder.btn_9x9.onClick.AddListener(OnClickBtn9x9);
        binder.btn_13x13.onClick.AddListener(OnClickBtn13x13);
        binder.btn_19x19.onClick.AddListener(OnClickBtn19x19);
        binder.btn_close.onClick.AddListener(OnClickBtnClose);
    }

    public void OnClickBtn9x9()
    {
        SceneCreateParams sceneCreateParams = new SceneCreateParams()
        {
            duelSceneCreateParamas = new DuelSceneCreateParamas()
            {
                boardCfgId = "9x9",
            }
        };
        Global.Instance.sceneManager.EnterMainScene(SceneConfig.DUEL_SCENE_TYPE_ID, sceneCreateParams);
    }

    public void OnClickBtn13x13()
    {
        SceneCreateParams sceneCreateParams = new SceneCreateParams()
        {
            duelSceneCreateParamas = new DuelSceneCreateParamas()
            {
                boardCfgId = "13x13",
            }
        };
        Global.Instance.sceneManager.EnterMainScene(SceneConfig.DUEL_SCENE_TYPE_ID, sceneCreateParams);
    }

    public void OnClickBtn19x19()
    {
        SceneCreateParams sceneCreateParams = new SceneCreateParams()
        {
            duelSceneCreateParamas = new DuelSceneCreateParamas()
            {
                boardCfgId = "19x19",
            }
        };
        Global.Instance.sceneManager.EnterMainScene(SceneConfig.DUEL_SCENE_TYPE_ID, sceneCreateParams);
    }

    public void OnClickBtnClose()
    {
        ClosePage();
    }
}