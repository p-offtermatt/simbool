using System;
using System.Linq;
using System.Collections.Generic;

namespace BoolForms
{
    public class Conjunction : NAryOperator
    {
        public Conjunction(params BooleanExpression[] expressions) : base(expressions)
        { }

        public Conjunction(IEnumerable<BooleanExpression> expressions) : base(expressions)
        { }

        public override BooleanExpression Flatten()
        {
            var flattenedOperands = this.GetOperands().Select(op => op.Flatten());

            var resultOperands = new List<BooleanExpression>();

            foreach (BooleanExpression expr in flattenedOperands)
            {
                if (expr is Conjunction conj)
                {
                    foreach (BooleanExpression op in conj.GetOperands())
                    {
                        resultOperands.Add(op);
                    }
                }
                else
                {
                    resultOperands.Add(expr);
                }
            }

            var result = new Conjunction(resultOperands);
            return result;
        }

        /// <summary>
        /// Applies all simplifications applicable to Conjunctions:
        /// Associativity/Flattening: A & (B & C) = A & B & C
        /// Idempotence: A&A = A
        /// Annihilation: A&0 = 0
        /// Identity: A&1 = A
        /// Complementation: A&!A = 0
        /// TODO: Elimination: (A|B) & (A|!B) = A
        /// TODO: Absorption: A & (A|B) = A, A & (!A | B) = A & B
        /// </summary>
        /// <returns></returns>
        public override BooleanExpression Simplify()
        {
            // simplify operands and apply idempotence (remove duplicate element)
            var newOperands = this.GetOperands().Select(op => op.Simplify()).Distinct();

            // apply annihilation
            if (newOperands.Any(x => x is Constant con && !con.GetValue()))
            {
                return BooleanExpression.FALSE;
            }

            // apply identity
            newOperands = newOperands.Where(op => !(op is Constant con && con.GetValue()));

            // apply complementation
            if (newOperands.Any(op => newOperands.Contains(new Negation(op))))
            {
                return BooleanExpression.FALSE;
            }

            // TODO: apply elimination
            // TODO: apply absorption

            // remove operator with one operand: (A) = A
            if (newOperands.Count() == 1)
            {
                return newOperands.First();
            }
            else
            {
                return new Conjunction(newOperands);
            }
        }

        public override BooleanExpression ToNNF()
        {
            return new Conjunction(this.operands.Select(op => op.ToNNF()));
        }

        public override string ToString()
        {
            return "(" + String.Join(" & ", this.operands.Select(op => op.ToString())) + ")";
        }

        internal override bool Is_DNF_Helper(bool seenNegation, bool seenDisjunction, bool seenConjunction)
        {
            if (seenNegation | seenConjunction)
            {
                return false;
            }
            return this.operands.All(op => op.Is_DNF_Helper(seenNegation, seenDisjunction, true));
        }

        internal override BooleanExpression ToDNF_Helper()
        {
            var newOperands = this.operands.Select(x => x.ToDNF_Helper());

            // flatten a conjunction over the operands to get operands without superfluous operator levels,
            // i.e. when one of the operands was a disjunction, but becomes a conjunction due to distributing
            // when putting it into DNF, we need to flatten in order to get all conjunction operands into the same layer
            var newOperandsList = ((Conjunction)new Conjunction(newOperands).Flatten()).GetOperands().ToList();

            var disjunctions = newOperands.Where(op => op is Disjunction).Cast<Disjunction>();

            if (!disjunctions.Any())
            { // no operand is a disjunction, so we can't distribute anything.
                return new Conjunction(newOperands);
            }
            else
            {
                Disjunction disj = disjunctions.First();

                // disjunction will be distributed, so remove it
                newOperandsList.Remove(disj);

                var distributedOperandLists = new List<List<BooleanExpression>>();

                foreach (BooleanExpression op in disj.GetOperands())
                {
                    distributedOperandLists.Add(newOperandsList.Append(op).ToList());
                }

                BooleanExpression resultDisj = new Disjunction(distributedOperandLists.Select(ops => new Conjunction(ops).ToDNF_Helper()));
                resultDisj = resultDisj.Flatten().Simplify().ToDNF_Helper().Simplify();
                return resultDisj;
            }
        }
    }
}