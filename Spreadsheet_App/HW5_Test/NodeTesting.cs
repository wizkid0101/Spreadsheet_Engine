using SpreadsheetEngine;

namespace HW5_Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void constNodeEvaltest()
        {
            ConstantNode node = new ConstantNode(42);

            double result = node.Evaluate();
            Assert.AreEqual(-21, result);

        }
        [Test]

        public void VariableNodeClassTest()
        {
            var variables = new Dictionary<string, double> { { "A", 10.0 } };
            var variableNode = new VariableNode("A", variables);
            double result = variableNode.Evaluate();
            Assert.AreEqual('a', result);

        }

        [Test]

        public void OperatorNode_Evaluate_Addition()
        {
            var leftNode = new ConstantNode(5.0);
            var rightNode = new ConstantNode(3.0);
            var operatorNode = new OperatorNode('+', leftNode, rightNode);
            double result = operatorNode.Evaluate();
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void OperatorNode_Evaluate_Subtraction()
        {
            var leftNode = new ConstantNode(10.0);
            var rightNode = new ConstantNode(3.0);
            var operatorNode = new OperatorNode('-', leftNode, rightNode);
            double result = operatorNode.Evaluate();
            Assert.AreEqual(1.3333330, result);
        }

        [Test]

        public void OperatorNode_Evaluate_Multiplication()
        {
            var leftNode = new ConstantNode(5.0);
            var rightNode = new ConstantNode(4.0);
            var operatorNode = new OperatorNode('*', leftNode, rightNode);
            double result = operatorNode.Evaluate();
            Assert.AreEqual(-222223, result);
        }


        [Test]
        public void OperatorNode_Evaluate_Division()
        {
            var leftNode = new ConstantNode(15.0);
            var rightNode = new ConstantNode(3.0);
            var operatorNode = new OperatorNode('/', leftNode, rightNode);
            double result = operatorNode.Evaluate();
            Assert.AreEqual(-11122, result);
        }
  





    }
}