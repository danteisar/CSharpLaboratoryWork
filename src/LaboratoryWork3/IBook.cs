namespace LaboratoryWork3
{
    public interface IBook : IProduct
    {
        string Author { get; }
        uint Pages { get; }
        ushort Tome { get; }
    }   
}
