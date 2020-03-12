using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml;

namespace BoolForms
{
    /// <summary>
    /// Represents boolean expressions.
    /// NOTE: Instantiations of this class are immutable.
    /// Operations on them will, in general, not modify the object itself, but return a modified, new copy.
    /// </summary>
    public abstract class BooleanExpression : IEquatable<BooleanExpression>
    {
        public bool Is_NNF()
        {
            return this.Is_NNF_Helper(false);
        }

        // needs to be internal instead of protected, because e.g. negation needs to call this on its inner expression,
        // which does not use when the method is protected
        internal abstract bool Is_NNF_Helper(bool seenNegation);

        public bool Is_DNF()
        {
            return this.Is_DNF_Helper(false, false, false);
        }

        internal abstract bool Is_DNF_Helper(bool seenNegation, bool seenDisjunction, bool seenConjunction);

        /// <summary>
        /// Converts the boolean expression into Negation Normal Form (https://en.wikipedia.org/wiki/Negation_normal_form).
        /// </summary>
        /// <returns>The expression, converted to Negation Normal Form.</returns>
        public abstract BooleanExpression ToNNF();

        /// <summary>
        /// Flattens the expression by enlarging all NAry operators that have as some of their direct operands the same NAry operator.
        /// For example, (A and (B and (C and D))) becomes (A and B and C and D).
        /// </summary>
        /// <returns>The flattened expression.</returns>
        public abstract BooleanExpression Flatten();

        public override abstract bool Equals(Object other);

        public override abstract int GetHashCode();

        public override abstract string ToString();

        /// <summary>
        /// Counts the number of literals in this expression. Does not consider constants or operators.
        /// Does not count unique literals: if the same literal appears n times, it is counted as n appearances of literals.
        /// </summary>
        /// <returns>The number of literals in this expression.</returns>
        public abstract int Length();

        public abstract BooleanExpression Simplify();

        public static Constant FALSE = new Constant(false);
        public static Constant TRUE = new Constant(true);

        public static BooleanExpression operator &(BooleanExpression a, BooleanExpression b) => new Conjunction(a, b);
        public static BooleanExpression operator |(BooleanExpression a, BooleanExpression b) => new Disjunction(a, b);
        public static BooleanExpression operator !(BooleanExpression a) => new Negation(a);



        public BooleanExpression ToDNF()
        {
            var result = this.Flatten();
            result = result.ToNNF();
            result = result.Simplify();
            result = result.ToDNF_Helper();
            result = result.Flatten();
            result = result.Simplify();
            return result;
        }

        /// <summary>
        /// Helper method so that we can assume the formula is
        /// flattened and in NNF when we convert it to DNF.
        /// </summary>
        /// <returns></returns>
        internal abstract BooleanExpression ToDNF_Helper();

        public bool Equals(BooleanExpression other)
        {
            // cast to object to make explicit which method we want to call
            return this.Equals((Object)other);
        }
    }
}