namespace Store.Exercises;

internal class Exercise6 : ExerciseBase, IExercise
{
    public int Number => 6;

    public string[] Code => ["public class BaseStorage",
                             "{",
                             "    private readonly List<double> _list = [];",
                             "    public virtual void Add(double value) => _list.Add(value);",
                             "    public virtual void AddAll(IEnumerable<double> values)",
                             "    {",
                             "        foreach (var element in values)",
                             "            Add(element);",
                             "    }",
                             "}",
                             "public class ItemsStorage: BaseStorage",
                             "{",
                             "    public int Amount { get; private set; } = 0;",
                             "    public override void Add(double value)",
                             "    {",
                             "        Amount++;",
                             "        base.Add(value);",
                             "    }",
                             "    public override void AddAll(IEnumerable<double> values)",
                             "    {",
                             "        Amount += values.Count();",
                             "        base.AddAll(values);",
                             "    }",
                             "}",
                             " ",
                             "ItemsStorage itemsStorage = new();",
                             "itemsStorage.AddAll([1, 2, 3]);",
                             "Console.WriteLine(itemsStorage.Count);"];

    public string[] Variants => [];

    public class BaseStorage
    {
        private readonly List<double> _list = [];
        public virtual void Add(double value) => _list.Add(value);
        public virtual void AddAll(IEnumerable<double> values)
        {
            foreach (var element in values)
                Add(element);
        }
    }

    public class ItemsStorage: BaseStorage
    {
        public int Amount { get; private set; } = 0;
        public override void Add(double value)
        {
            Amount++;
            base.Add(value);
        }
        public override void AddAll(IEnumerable<double> values)
        {
            Amount += values.Count();
            base.AddAll(values);
        }
    }

    public override string Exercise()
    {
        ItemsStorage itemsStorage = new();
        itemsStorage.AddAll([1, 2, 3]);
        return itemsStorage.Amount.ToString();
    }
}