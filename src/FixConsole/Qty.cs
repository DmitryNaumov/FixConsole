namespace FixConsole
{
    public struct Qty
    {
        private readonly double _value;

        public Qty(double value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator Qty(double value)
        {
            return new Qty(value);
        }

        public static Qty operator +(Qty lhs, Qty rhs)
        {
            return new Qty(lhs._value + rhs._value);
        }

    }
}