namespace BoolForms
{
    public class Literal : BooleanExpression
    {
        public readonly string Name;

        public Literal(string name)
        {
            this.Name = name;
        }

        public override bool Equals(object other)
        {
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }
            else
            {
                return this.Name == ((Literal)other).Name;
            }
        }

        public override BooleanExpression Flatten()
        {
            return this;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override BooleanExpression Simplify()
        {
            return this;
        }

        internal override BooleanExpression ToDNF_Helper()
        {
            return this;
        }

        public override BooleanExpression ToNNF()
        {
            return this;
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal override bool Is_NNF_Helper(bool seenNegation)
        {
            return true;
        }

        internal override bool Is_DNF_Helper(bool seenNegation, bool seenDisjunction, bool seenConjunction)
        {
            return true;
        }

        public override int Length()
        {
            return 1;
        }
    }
}