public interface ISavableRoot
{
    public SavableObj savableObj { get; }
    public string saveRootName { get; }
    public string saveFilePath { get; }
}
