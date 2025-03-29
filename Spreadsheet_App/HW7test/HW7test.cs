using SpreadsheetEngine;
using System.Linq.Expressions;

namespace HW7test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]


        public void GetVariableNames_ShouldReturnAllVariableNames()
        {
            string expression = "A1+B2-C3";
            ExpTree tree = new ExpTree(expression); // Your ExpressionTree constructor takes the full expression

            // Expected set of unique variable names parsed from the expression
            var expectedVariableNames = new HashSet<string> { "A1", "B2", "C3" };

            // Act: Get the variable names from the expression tree
            var actualVariableNames = tree.GetVariableNames();

            // Assert: The actual variable names should match the expected ones
            Assert.That(actualVariableNames, Is.EquivalentTo(expectedVariableNames), "The variable names extracted from the expression do not match the expected ones.");
        }

        [Test]
        public void Evaluate_SimpleAddition_ReturnsCorrectResult()
        {
            // Arrange
            var expTree = new ExpTree("2+3");

            // Act
            var result = expTree.Evaluate();

            // Assert
            Assert.AreEqual(5, result, "The evaluation of 2+3 should be 5.");
        }

        [Test]
        public void Evaluate_ComplexExpression_ReturnsCorrectResult()
        {
            // Arrange
            var expTree = new ExpTree("2+3*4");

            // Act
            var result = expTree.Evaluate();

            // Assert
            Assert.AreEqual(14, result, "The evaluation of 2+3*4 should be 14.");
        }

        [Test]
        public void Evaluate_ExpressionWithVariables_ReturnsCorrectResult()
        {
            // Arrange
            var expTree = new ExpTree("a+2*b");
            expTree.SetVariable("a", 3);
          
            expTree.SetVariable("b", 4);

            // Act
            var result = expTree.Evaluate();

            // Assert
            Assert.AreEqual(11, result, "The evaluation of a+2*b with a=3, b=4 should be 11.");



        }






    }


}
