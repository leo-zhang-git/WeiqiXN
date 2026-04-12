public class UserInfoPopup : UIPageWithBinder<UserInfoPopupUI>
{
    public override string pageName => UIPage.GetPageName<UserInfoPopup>();

    protected override void OnLoaded()
    {
        base.OnLoaded();

        binder.btn_close.onClick.AddListener(OnClickBtnClose);
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        binder.txt_user_id.text = User.Instance.compUserInfo.userId.value;
        binder.txt_win_count.text = User.Instance.compUserInfo.winCount.value.ToString();
        binder.txt_lose_count.text = User.Instance.compUserInfo.loseCount.value.ToString();
    }

    public void OnClickBtnClose()
    {
        ClosePage();
    }
}