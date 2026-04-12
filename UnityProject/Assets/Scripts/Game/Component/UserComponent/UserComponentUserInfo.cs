using UnityEngine;

public class UserComponentUserInfo : UserComponentBase
{
    public SavableField<string> userId = SavableFieldFactory.CreateStringField(string.Empty);
    public SavableField<int> winCount = SavableFieldFactory.CreateIntField(0);
    public SavableField<int> loseCount = SavableFieldFactory.CreateIntField(0);

    public UserComponentUserInfo(User owner) : base(owner)
    {

    }

    public void CreateNewUser()
    {
        // 随机生成9位数id
        userId.value = Random.Range(100000, 999999).ToString();
        winCount.value = 0;
        loseCount.value = 0;
    }
}
