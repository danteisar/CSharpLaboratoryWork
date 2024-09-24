using System;

namespace Store.Exercises.Exercise6;

internal class Exercise64: ExerciseBase, IExercise
{
    public string[] Code => ["public class BaseStorage",
                             "{",
                             "    public int Amount { get; private set; } = 0;",
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
                             "    public new int Amount { get; private set; } = 0;",
                             "    public override Add(double value)",
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
                             "BaseStorage itemsStorage = new ItemsStorage();",
                             "itemsStorage.AddAll([1, 2, 3]);",
                             "Console.WriteLine(itemsStorage.Count);"];

    public string[] Variants => ["0", "3", "6", "9"];

    public class BaseStorage
    {
        public int Amount { get; private set; } = 0;
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
        public new int Amount { get; private set; } = 0;
        public override void Add(double value)
        {
            Amount++;
            base.Add(value);
        }
        public override void AddAll(IEnumerable<double> values)
        {
            Amount -= values.Count();
            base.AddAll(values);
        }
    }

    public override string Exercise()
    {
        BaseStorage itemsStorage = new ItemsStorage();
        itemsStorage.AddAll([1, 2, 3]);
        return itemsStorage.Amount.ToString();
    }
}