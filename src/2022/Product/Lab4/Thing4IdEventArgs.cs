namespace Product.Lab4;

public sealed class Thing4IdEventArgs : EventArgs
{
    public int OldId { get; }   
    public int NewId { get; }

    public Thing4IdEventArgs(int oldId, int newId)
    {
        OldId = oldId;
        NewId = newId;
    }
}