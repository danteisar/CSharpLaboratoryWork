namespace Store.Exercises.Exercise6;

internal class Exercise61 : ExerciseBase, IExercise
{
    public virtual string[] Code => ["public class BaseStorage",
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
                                     "}"];
    public virtual string[] TestCode => ["ItemsStorage itemsStorage = new();",
                                         "itemsStorage.AddAll([1, 2, 3]);",
                                         "Console.WriteLine(itemsStorage.Amount);"];
    public override string[] Variants => ["0", "3", "6", "9"];

    private class BaseStorage
    {
        private readonly List<double> _list = [];
        public virtual void Add(double value) => _list.Add(value);
        public virtual void AddAll(IEnumerable<double> values)
        {
            foreach (var element in values)
                Add(element);
        }
    }

    private class ItemsStorage: BaseStorage
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