public class SavableField<TValue>
{
    public TValue value { get; set; }

    private SavableField(TValue value)
    {
        this.value = value;
    }

    public SavableField<int> CreateIntField(int value)
    {
        return new SavableField<int>(value);
    }

    public SavableField<float> CreateFloatField(float value)
    {
        return new SavableField<float>(value);
    }

    public SavableField<bool> CreateBoolField(bool value)
    {
        return new SavableField<bool>(value);
    }

    public SavableField<string> CreateStringField(string value)
    {
        return new SavableField<string>(value);
    }
}
