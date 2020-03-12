namespace BoolForms
{
    public abstract class UnaryOperator : BooleanExpression
    {
        public readonly BooleanExpression InnerExpression;

        public UnaryOperator(BooleanExpression innerExpression)
        {
            this.InnerExpression = innerExpression;
        }

        public int GetArity()
        {
            return 1;
        }

        public override bool Equals(object other)
        {
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }
            else
            {
                return this.InnerExpression.Equals(((UnaryOperator)other).InnerExpression);
            }
        }

        public int? hashCode = null;
        public override int GetHashCode()
        {
            if (this.hashCode.HasValue)
            {
                return this.hashCode.Value;
            }
            else
            {
                return this.GetType().GetHashCode() * this.InnerExpression.GetHashCode();
            }
        }

        public override int Length()
        {
            return InnerExpression.Length();
        }
    }
}