using Xunit;
using Xunit.Extensions;
using System.Linq;
using BoolForms;
using static BoolForms.BooleanExpression;
using Xunit.Abstractions;

namespace Testing
{


    public class BasicClassHierarchyTest
    {
        Literal A = new Literal("A");
        Literal B = new Literal("B");
        Literal C = new Literal("C");
        Literal D = new Literal("D");
        Literal E = new Literal("E");

        [Fact]
        public void TestBaseClassTyping()
        {
            Assert.True(A is BooleanExpression);
            Assert.True(A is Literal);

            BooleanExpression negation = new Negation(A);
            Assert.True(negation is BooleanExpression);
            Assert.True(negation is UnaryOperator);
            Assert.True(negation is Negation);

            BooleanExpression disjunction = new Disjunction(A, A, A);
            Assert.True(disjunction is BooleanExpression);
            Assert.True(disjunction is NAryOperator);
            Assert.True(disjunction is Disjunction);
        }

        [Fact]
        public void TestArity()
        {
            for (int i = 0; i < 200; i++)
            {
                BooleanExpression[] operands = new BooleanExpression[i];
                for (int j = 0; j < i; j++)
                {
                    operands[j] = A;
                }
                Disjunction dis = new Disjunction(operands);
                Assert.Equal(i, dis.GetArity());
            }
        }

        [Fact]
        public void TestEquality()
        {
            Assert.Equal(A, A);

            Literal APrime = new Literal("A");
            Assert.Equal(A, APrime);

            Conjunction conj = new Conjunction(A, A, APrime);
            Assert.NotEqual<BooleanExpression>(conj, A);

            Conjunction conj1 = new Conjunction(A, A, APrime);
            Assert.Equal(conj1, conj);

            Conjunction conj2 = new Conjunction(APrime, A, A);
            Assert.Equal(conj1, conj2);

            Literal B = new Literal("B");

            Conjunction conj3 = new Conjunction(B, A, APrime);
            Assert.NotEqual<BooleanExpression>(conj1, conj3);

            Negation neg1 = new Negation(A);
            Negation neg2 = new Negation(new Literal("A"));
            Assert.Equal(neg1, neg2);
            Assert.Equal(neg2, neg1);
        }

        [Fact]
        public void TestHashCode()
        {
            Negation neg1 = new Negation(new Literal("A"));
            Negation neg2 = new Negation(new Literal("A"));

            int hashCode1 = neg1.GetHashCode();
            int hashCode2 = neg2.GetHashCode();
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void TestEqualityDifferentTypes()
        {
            Negation neg = new Negation(A);
            Conjunction conj = new Conjunction(A, neg);
            Disjunction disj = new Disjunction(A, neg);

            Assert.NotEqual<BooleanExpression>(A, neg);
            Assert.NotEqual<BooleanExpression>(A, conj);
            Assert.NotEqual<BooleanExpression>(A, disj);

            Assert.NotEqual<BooleanExpression>(neg, A);
            Assert.NotEqual<BooleanExpression>(neg, disj);
            Assert.NotEqual<BooleanExpression>(neg, conj);

            Assert.NotEqual<BooleanExpression>(conj, A);
            Assert.NotEqual<BooleanExpression>(conj, neg);
            Assert.NotEqual<BooleanExpression>(conj, disj);

            Assert.NotEqual<BooleanExpression>(disj, A);
            Assert.NotEqual<BooleanExpression>(disj, neg);
            Assert.NotEqual<BooleanExpression>(disj, conj);
        }

        [Fact]
        public void TestNestedOperatorsEquality()
        {

            Conjunction conj1 = new Conjunction(A, B);
            Disjunction disj1 = new Disjunction(C, D);
            Disjunction disj2 = new Disjunction(disj1, E);
            Conjunction conj2 = new Conjunction(conj1, disj2);

            Conjunction expected = new Conjunction(A, B, new Disjunction(C, D, E));

            Assert.NotEqual(expected, conj1);
        }

        [Fact]
        public void TestUnorderedEquality()
        {
            Conjunction conj1 = new Conjunction(A, B);
            Conjunction conj2 = new Conjunction(B, A);

            Assert.Equal(conj1, conj2);
        }

        [Fact]
        public void TestLength1()
        {
            BooleanExpression expr = A | B & C | A | B & !C;
            Assert.Equal(6, expr.Length());
        }

        [Fact]
        public void TestLength2()
        {
            BooleanExpression expr = !!(!!!A & BooleanExpression.FALSE);
            Assert.Equal(1, expr.Length());
        }
    }

    // tests examples from https://en.wikipedia.org/wiki/Negation_normal_form
    // some omitted due to lack of support for all operators
    public class TestNNF
    {
        Literal A = new Literal("A");
        Literal B = new Literal("B");
        Literal C = new Literal("C");
        Literal D = new Literal("D");

        [Fact]
        public void TrueExample1()
        {
            Disjunction disj = new Disjunction(A, B);
            Conjunction conj = new Conjunction(disj, C);
            Assert.True(conj.Is_NNF());
        }

        [Fact]
        public void TrueExample2()
        {
            Disjunction disj = new Disjunction(new Negation(B), C);
            Conjunction conj = new Conjunction(A, disj, new Negation(C));
            Disjunction outerDisj = new Disjunction(conj, D);
            Assert.True(outerDisj.Is_NNF());
        }

        [Fact]
        public void TrueExample3()
        {
            Disjunction disj = new Disjunction(A, new Negation(B));
            Assert.True(disj.Is_NNF());
        }

        [Fact]
        public void TrueExample4()
        {
            Conjunction conj = new Conjunction(A, new Negation(B));
            Assert.True(conj.Is_NNF());
        }

        [Fact]
        public void FalseExample1()
        {
            Negation neg = new Negation(new Disjunction(A, B));
            Assert.False(neg.Is_NNF());
        }

        [Fact]
        public void FalseExample2()
        {
            Negation neg = new Negation(new Conjunction(A, B));
            Assert.False(neg.Is_NNF());
        }

        [Fact]
        public void FalseExample3()
        {
            Negation neg = new Negation(new Disjunction(A, new Negation(C)));
            Assert.False(neg.Is_NNF());
        }

        // and some more tests for good measure

        [Fact]
        public void NestedExample()
        {
            Disjunction disj = new Disjunction(A, B);
            Disjunction disj1 = new Disjunction(disj, A);
            Conjunction conj = new Conjunction(disj1, A);
            Conjunction conj1 = new Conjunction(new Negation(conj), A);

            Assert.False(conj1.Is_NNF());
            Assert.True(conj.Is_NNF());
        }

        [Fact]
        public void LiteralExample()
        {
            Assert.True(A.Is_NNF());
            Assert.True(new Negation(A).Is_NNF());
            Assert.False(new Negation(new Negation(A)).Is_NNF());
        }
    }

    public class TestFlatten
    {
        Literal A = new Literal("A");
        Literal B = new Literal("B");
        Literal C = new Literal("C");
        Literal D = new Literal("D");
        Literal E = new Literal("E");
        Literal F = new Literal("F");
        Literal G = new Literal("G");


        [Fact]
        public void SimpleFlatten()
        {
            Conjunction conj = new Conjunction(A, B);
            Conjunction conj1 = new Conjunction(C, conj);

            Conjunction expected = new Conjunction(C, A, B);
            Conjunction actual = ((Conjunction)conj1.Flatten());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NestedFlatten()
        {
            Conjunction opConj1 = new Conjunction(A, B);
            Disjunction disj = new Disjunction(C, B);
            Conjunction opConj2 = new Conjunction(opConj1, disj);

            Conjunction conj1 = (Conjunction)new Conjunction(opConj2, A, B).Flatten();

            Conjunction expectedConj = (Conjunction)new Conjunction(A, B, new Disjunction(C, B), A, B);

            Assert.Equal(expectedConj, conj1);
        }

        [Fact]
        public void MultiNestedFlatten()
        {
            Conjunction opConj1 = new Conjunction(A, B);
            Disjunction disj = new Disjunction(C, B);
            Conjunction opConj2 = new Conjunction(opConj1, disj, disj);

            Conjunction conj1 = (Conjunction)new Conjunction(opConj2, opConj1).Flatten();

            Conjunction expectedConj = (Conjunction)new Conjunction(A, B, new Disjunction(C, B), new Disjunction(C, B), A, B);

            Assert.Equal(expectedConj, conj1);
        }

        [Fact]
        public void MultiNestedMultiOperatorFlatten()
        {
            Conjunction conj1 = new Conjunction(A, B);
            Disjunction disj1 = new Disjunction(C, D);
            Disjunction disj2 = new Disjunction(disj1, E);
            Conjunction conj2 = new Conjunction(conj1, disj2);

            Conjunction expected = new Conjunction(A, B, new Disjunction(C, D, E));
            BooleanExpression actual = conj2.Flatten();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FlattenWithNegation()
        {
            Conjunction conj1 = new Conjunction(new Negation(A), new Negation(B));
            Conjunction conj2 = new Conjunction(new Negation(conj1), conj1);

            BooleanExpression actual = conj2.Flatten();

            Conjunction expected = new Conjunction(new Negation(new Conjunction(new Negation(A), new Negation(B))),
                                                   new Negation(A),
                                                   new Negation(B));

            Assert.Equal(expected, actual);
        }
    }

    public class TestSimplification
    {
        Literal A = new Literal("A");
        Literal B = new Literal("B");
        Literal C = new Literal("C");
        Literal D = new Literal("D");
        Literal E = new Literal("E");
        Literal F = new Literal("F");
        Literal G = new Literal("G");

        [Fact]
        public void TestConjunctionIdentity()
        {
            BooleanExpression actual = new Conjunction(A, BooleanExpression.TRUE).Simplify();
            BooleanExpression expected = A;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestDisjunctionIdentity()
        {
            BooleanExpression actual = new Disjunction(A, BooleanExpression.FALSE).Simplify();
            BooleanExpression expected = A;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestConjunctionAnnihilation()
        {
            BooleanExpression actual = new Conjunction(A, BooleanExpression.FALSE).Simplify();
            BooleanExpression expected = BooleanExpression.FALSE;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestDisjunctionAnnihilation()
        {
            BooleanExpression actual = new Disjunction(A, BooleanExpression.TRUE).Simplify();
            BooleanExpression expected = BooleanExpression.TRUE;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestConjunctionIdempotence()
        {
            BooleanExpression actual = new Conjunction(A, A).Simplify();
            BooleanExpression expected = A;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestDisjunctionIdempotence()
        {
            BooleanExpression actual = new Disjunction(A, A).Simplify();
            BooleanExpression expected = A;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestTrueConstantNegation()
        {
            BooleanExpression actual = new Negation(BooleanExpression.FALSE).Simplify();
            BooleanExpression expected = BooleanExpression.TRUE;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestFalseConstantNegation()
        {
            BooleanExpression actual = new Negation(BooleanExpression.TRUE).Simplify();
            BooleanExpression expected = BooleanExpression.FALSE;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestNestedFormula()
        {
            Conjunction conj1 = new Conjunction(A, BooleanExpression.FALSE); //simplifies to A
            Disjunction disj = new Disjunction(conj1, A, A); // simplifies to A | A | A = A
            Conjunction conj2 = new Conjunction(disj, new Negation(new Negation(BooleanExpression.TRUE))); // simplifies to A & !!TRUE = A
            BooleanExpression actual = conj2.Simplify();
            BooleanExpression expected = A;
            Assert.Equal(expected, actual);
        }
    }

    public class TestDNF
    {

        private readonly ITestOutputHelper output;

        public TestDNF(ITestOutputHelper output)
        {
            this.output = output;
        }
        static Literal A = new Literal("A");
        static Literal B = new Literal("B");
        static Literal C = new Literal("C");
        static Literal D = new Literal("D");
        static Literal E = new Literal("E");
        static Literal F = new Literal("F");


        Negation Aneg = new Negation(A);
        Negation Bneg = new Negation(B);
        Negation Cneg = new Negation(C);
        Negation Dneg = new Negation(D);
        Negation Eneg = new Negation(E);


        [Fact]
        public void TestDNFIdempotence()
        {
            Conjunction conj1 = new Conjunction(A, Aneg);
            Disjunction disj1 = new Disjunction(conj1, Bneg, Dneg, E);
            Conjunction conj2 = new Conjunction(A, Dneg, disj1);

            BooleanExpression actual = conj2.ToDNF().ToDNF();
            BooleanExpression expected = conj2.ToDNF();

            Assert.Equal(expected, actual);

            Assert.True(actual.Is_DNF());
            Assert.True(expected.Is_DNF());
        }

        [Fact]
        public void TestSimpleExample()
        {
            Disjunction disj1 = new Disjunction(A, Bneg);
            Conjunction conj = new Conjunction(disj1, C);

            BooleanExpression actual = conj.ToDNF();

            BooleanExpression expected = new Disjunction(new Conjunction(A, C), new Conjunction(Bneg, C));

            Assert.True(actual.Is_DNF());
            Assert.True(expected.Is_DNF());
            Assert.Equal(expected, actual);
        }

        // example from https://www.cs.sfu.ca/~ggbaker/zju/math/normalform.html
        [Fact]
        public void TestMultiOperatorFormula1()
        {
            Conjunction conj = new Conjunction(new Negation(C),
                                               B);
            BooleanExpression formula = new Disjunction(new Negation(new Disjunction(new Negation(A), B)),
                                                        conj);
            BooleanExpression actual = formula.ToDNF();

            BooleanExpression expected = new Disjunction(new Conjunction(A, new Negation(B)),
                                                         new Conjunction(new Negation(C), B));

            Assert.True(actual.Is_DNF());
            Assert.True(expected.Is_DNF());
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void TestEmptyDisjunction()
        {
            BooleanExpression testExpression = ((A | !(B | C)) & ((!D & (A & E)) & !(E | F)));
            testExpression = testExpression.ToDNF();

            BooleanExpression expected = FALSE;

            Assert.Equal(expected, testExpression);
        }
    }

    public class TestOverloads
    {
        Literal A = new Literal("A");
        Literal B = new Literal("B");
        Literal C = new Literal("C");

        [Fact]
        public void TestNegationOverload()
        {
            BooleanExpression actual = !FALSE;
            BooleanExpression expected = new Negation(FALSE);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestConjunctionDisjunctionOverload()
        {
            BooleanExpression actual = A & B | !C;
            BooleanExpression expected = new Disjunction(new Conjunction(A, B), new Negation(C));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestOperatorPrecedence()
        {
            BooleanExpression actual = (A & (B & (C & (!A & (!B & (!C))))));
            BooleanExpression expected = new Conjunction(A,
                                                new Conjunction(B,
                                                    new Conjunction(C,
                                                        new Conjunction(!A,
                                                            new Conjunction(!B, !C)))));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestNegationPrecedence()
        {
            BooleanExpression actual1 = !A & B;
            BooleanExpression actual2 = !(A & B);

            Assert.NotEqual(actual1, actual2);

            BooleanExpression expected1 = new Conjunction(new Negation(A), B);
            BooleanExpression expected2 = new Negation(new Conjunction(A, B));
        }

        [Fact]
        public void TestOperatorOverloadChainedOperation()
        {
            BooleanExpression expr = A & (B | C);

            BooleanExpression exprInDNF = (A & B) | (A & C);

            BooleanExpression actual = !B | expr.ToDNF();
            BooleanExpression expected = !B | exprInDNF;
            Assert.Equal(expected, actual);
        }
    }
}