using System.Linq;
using System.Collections.Generic;

namespace BoolForms
{
    public class Negation : UnaryOperator
    {
        public Negation(BooleanExpression innerExpression) : base(innerExpression)
        { }

        internal override bool Is_NNF_Helper(bool seenNegation)
        {
            if (seenNegation)
            {
                // no double negation allowed in nnf
                return false;
            }
            else
            {
                return InnerExpression.Is_NNF_Helper(true);
            }
        }

        public override BooleanExpression ToNNF()
        {
            if (InnerExpression is Negation neg)
            {
                return neg.InnerExpression.ToNNF();
            }
            else if (InnerExpression is NAryOperator nary)
            {
                List<BooleanExpression> newOperands = nary.GetOperands().Select(op => new Negation(op).ToNNF()).ToList();

                if (nary is Disjunction)
                {
                    return new Conjunction(newOperands);
                }
                else // nary is Conjunction
                {
                    return new Disjunction(newOperands);
                }
            }
            else if (InnerExpression.Equals(FALSE))
            {
                return TRUE;
            }
            else if (InnerExpression.Equals(TRUE))
            {
                return FALSE;
            }
            else
            {
                return new Negation(this.InnerExpression.Simplify());
            }
        }

        public override BooleanExpression Flatten()
        {
            return new Negation(this.InnerExpression.Flatten());
        }

        public override string ToString()
        {
            return "!" + InnerExpression.ToString();
        }

        public override BooleanExpression Simplify()
        {
            var newInnerExpression = InnerExpression.Simplify();
            if (newInnerExpression.Equals(FALSE))
            {
                return TRUE;
            }
            if (newInnerExpression.Equals(TRUE))
            {
                return FALSE;
            }
            return new Negation(InnerExpression.Simplify());
        }

        internal override BooleanExpression ToDNF_Helper()
        {
            return new Negation(this.InnerExpression.ToDNF_Helper());
        }

        internal override bool Is_DNF_Helper(bool seenNegation, bool seenDisjunction, bool seenConjunction)
        {
            if (seenNegation)
            {
                return false;
            }
            return InnerExpression.Is_DNF_Helper(true, seenDisjunction, seenConjunction);
        }
    }
}