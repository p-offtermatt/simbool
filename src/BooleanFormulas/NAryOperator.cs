using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BoolForms
{
    public abstract class NAryOperator : BooleanExpression
    {

        protected readonly ReadOnlyCollection<BooleanExpression> operands;

        public int GetArity() => this.operands.Count;

        public ReadOnlyCollection<BooleanExpression> GetOperands()
        {
            return operands;
        }

        public NAryOperator(params BooleanExpression[] expressions) : this(expressions != null ? expressions.AsEnumerable() : null)
        { }

        public NAryOperator(IEnumerable<BooleanExpression> expressions)
        {
            if (expressions != null)
            {
                this.operands = new ReadOnlyCollection<BooleanExpression>(expressions.ToList());
            }
            else
            {
                throw new NullReferenceException("Expressions cannot be null.");
            }
        }

        public override bool Equals(object other)
        {
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }
            else
            {
                var otherOperands = ((NAryOperator)other).GetOperands();
                if (this.operands.Count != otherOperands.Count)
                {
                    return false;
                }
                return Utils.UnorderedEnumerableEquals<BooleanExpression>(this.operands, otherOperands);
            }
        }

        private int? hashCode = null;
        public override int GetHashCode()
        {
            if (this.hashCode.HasValue)
            {
                return this.hashCode.Value;
            }
            else
            {
                return this.GetType().GetHashCode() * this.operands.Aggregate(0, (acc, op) => acc += op.GetHashCode());
            }
        }

        internal override bool Is_NNF_Helper(bool seenNegation)
        {
            if (seenNegation)
            {
                // non-literal observed after negation, not allowed!
                return false;
            }
            else
            {
                return this.operands.All(op => op.Is_NNF_Helper(false));
            }
        }

        public override int Length()
        {
            return operands.Aggregate(
                0,
                (acc, op) => acc + op.Length());
        }
    }
}