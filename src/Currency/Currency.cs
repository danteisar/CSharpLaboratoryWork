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
    /// <para><see cref="Cource"/> CurrentCource = (<see cref="Rub"/>)100; //Означает что 1 евро стоит 100 рублей. </para>
    /// <para><see cref="Cource"/> CurrentCource = (<see cref="Usd"/>)0.1; //Означает что 1 рубль стоит 1 цент. </para>
    /// </summary>
    public class Cource
    {
        private Rub _rub;
        private Usd _usd;
        private Cource(Rub rub, Usd usd)
        {
            _rub = rub;
            _usd = usd;
        }

        public static implicit operator Rub(Cource value) => value._rub / value._usd;
        public static implicit operator Usd(Cource value) => value._usd / value._rub;
        public static implicit operator Cource(Rub value) => new Cource(value, 1);
        public static implicit operator Cource(Usd value) => new Cource(1, value);

        public override string ToString() => _rub > _usd
            ? $"Курс {_usd} по {_rub}"
            : $"Курс {_rub} по {_usd}";
    }
}
