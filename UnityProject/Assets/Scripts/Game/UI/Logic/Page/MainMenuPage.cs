using System.IO;

public class MainMenuPage : UIPageWithBinder<MainMenuPageUI>
{
    public override string pageName => UIPage.GetPageName<MainMenuPage>();

    protected override void OnLoaded()
    {
        base.OnLoaded();

        binder.btn_continue.onClick.AddListener(OnClickBtnContinue);
        binder.btn_new_game.onClick.AddListener(OnClickBtnNewGame);
        binder.btn_exit.onClick.AddListener(OnClickBtnExit);
        binder.btn_user_info.onClick.AddListener(OnClickBtnUserInfo);
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        binder.btn_continue.interactable = File.Exists(GameSaveConfig.GetDuelSceneSavePath(0));
    }

    public void OnClickBtnContinue()
    {
        string saveFilePath = GameSaveConfig.GetDuelSceneSavePath(0);
        if (File.Exists(saveFilePath)) {
            Global.Instance.sceneManager.EnterMainScene(SceneConfig.DUEL_SCENE_TYPE_ID, saveFilePath);
        }
    }

    public void OnClickBtnNewGame()
    {
        Global.Instance.sceneManager.EnterMainScene(SceneConfig.DUEL_SCENE_TYPE_ID);
    }

    public void OnClickBtnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    public void OnClickBtnUserInfo()
    {
        Global.Instance.uiManager.ShowPage<UserInfoPopup>();
    }
}