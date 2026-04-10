public class SavableField<TValue>
{
    public TValue value { get; set; }

    public SavableField(TValue value)
    {
        this.value = value;
    }
}

public static class SavableFieldFactory
{
    public static SavableField<int> CreateIntField(int value)
    {
        return new SavableField<int>(value);
    }

    public static SavableField<float> CreateFloatField(float value)
    {
        return new SavableField<float>(value);
    }

    public static SavableField<bool> CreateBoolField(bool value)
    {
        return new SavableField<bool>(value);
    }

    public static SavableField<string> CreateStringField(string value)
    {
        return new SavableField<string>(value);
    }
}
