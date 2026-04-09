public static class ReddotConst
{
    public readonly static string ReddotKeySpliter = "$";

    public static string ConcatReddotKey(string[] keys)
    {
        if (keys.Length == 0) {
            return string.Empty;
        }

        return string.Join(ReddotKeySpliter, keys);
    }
}

public static class ReddotKeyDefine
{

}
