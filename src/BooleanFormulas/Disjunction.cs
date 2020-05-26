using System;
using System.Linq;
using System.Collections.Generic;

namespace BoolForms
{
    public class Disjunction : NAryOperator
    {
        public Disjunction(params BooleanExpression[] expressions) : base(expressions)
        { }

        public Disjunction(IEnumerable<BooleanExpression> expressions) : base(expressions)
        { }

        public override BooleanExpression Flatten()
        {
            var flattenedOperands = this.GetOperands().Select(op => op.Flatten());

            var resultOperands = new List<BooleanExpression>();

            foreach (BooleanExpression expr in flattenedOperands)
            {
                if (expr is Disjunction disj)
                {
                    foreach (BooleanExpression op in disj.GetOperands())
                    {
                        resultOperands.Add(op);
                    }
                }
                else
                {
                    resultOperands.Add(expr);
                }
            }

            var result = new Disjunction(resultOperands);
            return result;
        }

        /// <summary>
        /// Applies all simplifications applicable to Disjunctions:
        /// Associativity/Flattening: A | (B | C) = A | B | C
        /// Idempotence: A|A = A
        /// Annihilation: A|1 = 1
        /// Identity: A|0 = A
        /// Complementation: A|!A = 1
        /// TODO: Elimination: (A&B) | (A&!B) = A
        /// TODO: Absorption: A | (A&B) = A, A | (!A & B) = A | B
        /// </summary>
        /// <returns></returns>
        public override BooleanExpression Simplify()
        {
            // simplify operands and apply idempotence (remove duplicate element)
            var newOperands = this.GetOperands().Select(op => op.Simplify()).Distinct();

            // apply annihilation
            if (newOperands.Any(x => x is Constant con && con.GetValue()))
            {
                return BooleanExpression.TRUE;
            }

            // apply identity
            newOperands = newOperands.Where(op => !(op is Constant con && !con.GetValue()));

            // apply complementation
            if (newOperands.Any(op => newOperands.Contains(new Negation(op))))
            {
                return BooleanExpression.TRUE;
            }

            // TODO: apply elimination
            // TODO: apply absorption

            // remove operator with one operand: (A) = A
            if (newOperands.Count() == 1)
            {
                return newOperands.First();
            }
            // remove operator with zero operands: Empty disjunction is false
            else if (newOperands.Count() == 0)
            {
                return FALSE;
            } else
            {
                return new Disjunction(newOperands);
            }
        }

        public override BooleanExpression ToNNF()
        {
            return new Disjunction(this.operands.Select(op => op.ToNNF()));
        }

        public override string ToString()
        {
            return "(" + String.Join(" | ", this.operands.Select(op => op.ToString())) + ")";
        }

        internal override bool Is_DNF_Helper(bool seenNegation, bool seenDisjunction, bool seenConjunction)
        {
            if (seenNegation | seenDisjunction | seenConjunction)
            {
                return false;
            }

            return this.operands.All(op => op.Is_DNF_Helper(seenNegation, true, seenConjunction));
        }

        internal override BooleanExpression ToDNF_Helper()
        {
            var newOperands = this.operands.Select(op => op.ToDNF_Helper());
            return new Disjunction(newOperands).Flatten();
        }
    }
}