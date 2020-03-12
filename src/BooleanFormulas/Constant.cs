using System;

namespace BoolForms
{
    public class Constant : BooleanExpression
    {
        private readonly bool Value;

        public bool GetValue()
        {
            return Value;
        }
        
        public Constant(bool value)
        {
            this.Value = value;
        }

        public override bool Equals(object other)
        {
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }
            else
            {
                return this.Value == ((Constant)other).Value;
            }

        }

        public override BooleanExpression Flatten()
        {
            return this;
        }

        public override int GetHashCode()
        {
            return this.Value ? 1 : 0;
        }

        public override BooleanExpression Simplify()
        {
            return this;
        }

        public override BooleanExpression ToNNF()
        {
            return this;
        }

        public override string ToString()
        {
            return this.Value ? "True" : "False";
        }

        internal override bool Is_NNF_Helper(bool seenNegation)
        {
            return true;
        }

        internal override BooleanExpression ToDNF_Helper()
        {
            return this;
        }

        internal override bool Is_DNF_Helper(bool seenNegation, bool seenDisjunction, bool seenConjunction)
        {
            return true;
        }

        public override int Length()
        {
            return 0;
        }
    }
}