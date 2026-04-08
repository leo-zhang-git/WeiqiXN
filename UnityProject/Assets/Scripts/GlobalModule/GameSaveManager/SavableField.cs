public class SavableField<TValue>
{
    public TValue value { get; set; }

    private SavableField(TValue value)
    {
        this.value = value;
    }

    public SavableField<int> Create(int value)
    {
        return new SavableField<int>(value);
    }

    public SavableField<float> Create(float value)
    {
        return new SavableField<float>(value);
    }

    public SavableField<bool> Create(bool value)
    {
        return new SavableField<bool>(value);
    }

    public SavableField<string> Create(string value)
    {
        return new SavableField<string>(value);
    }
}
