namespace Currency
{
    /// <summary>
    /// Валюта
    /// </summary>
    public abstract class Currency
    {
        public decimal Cost { get; set; }
        public override string ToString() => Cost.ToString("F2");
    }

    /// <summary>
    /// <inheritdoc cref="Currency"/>
    /// <para> Рубли</para>
    /// <para>Можно неявно указывать стоимость в <see cref="decimal"/></para>
    /// </summary>
    public class Rub : Currency
    {
        public static implicit operator decimal(Rub value) => value.Cost;
        public static implicit operator Rub(decimal value) => new Rub { Cost = value };
        public override string ToString() => $"{Cost:F2} руб.";
    }


    /// <summary>
    /// <inheritdoc cref="Currency"/>
    /// <para> Евро</para>
    /// <para>Можно неявно указывать стоимость в <see cref="decimal"/></para>
    /// </summary>
    public class Usd : Currency
    {
        public static implicit operator decimal(Usd value) => value.Cost;
        public static implicit operator Usd(decimal value) => new Usd { Cost = value };
        public override string ToString() => $"${Cost:F2}";
    }


    /// <summary>
    /// Валютный курс
    /// <para>
    /// Устанавливает соотношение одной валюты к другой. 
    /// </para>
    /// <para>
    /// Можно неявно указать сколько стоит 1 единица валюты относительно второй, для примера:
    ///  </para>
    /// <para><see cref="Course"/> CurrentCource = (<see cref="Rub"/>)100; //Означает что 1 евро стоит 100 рублей. </para>
    /// <para><see cref="Course"/> CurrentCource = (<see cref="Usd"/>)0.1; //Означает что 1 рубль стоит 1 цент. </para>
    /// </summary>
    public class Course
    {
        private Rub _rub;
        private Usd _usd;
        private Course(Rub rub, Usd usd)
        {
            _rub = rub;
            _usd = usd;
        }

        public static implicit operator Rub(Course value) => value._rub / value._usd;
        public static implicit operator Usd(Course value) => value._usd / value._rub;
        public static implicit operator Course(Rub value) => new Course(value, 1);
        public static implicit operator Course(Usd value) => new Course(1, value);

        public override string ToString() => _rub > _usd
            ? $"Курс {_usd} по {_rub}"
            : $"Курс {_rub} по {_usd}";
    }
}
