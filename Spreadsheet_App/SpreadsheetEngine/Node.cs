using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

//Benjamin Bordon
//ID: 011843215

namespace SpreadsheetEngine
{
    // Base Node class
    public abstract class Node
    {
        public abstract double Evaluate(Dictionary<string, double> variableValues);
    }

    // Node for constant values
    public class ConstantNode : Node
    {
        private double _value;

        public ConstantNode(double value)
        {
            _value = value;
        }

        public override double Evaluate(Dictionary<string, double> variableValues)
        {
            return _value;
        }
    }

    // Node for variables
    public class VariableNode : Node
    {
        private string _variableName;
        public string VariableName => _variableName;

        public VariableNode(string variableName)
        {
            _variableName = variableName;
        }

        public override double Evaluate(Dictionary<string, double> variableValues)
        {
            if (variableValues.TryGetValue(_variableName, out double value))
                return value;
            return 0;  // Default value if not set
        }
    }

    // Node for operators
    public class OperatorNode : Node
    {
        private char _operator;
        private Node _left;
        private Node _right;
        public Node Left => _left;// public left get methods for the expression tree
        public Node Right => _right;//public right get methods for the expression tree

        public OperatorNode(char op, Node left, Node right)
        {


            _operator = op;
            _left = left;
            _right = right;
        }

        public override double Evaluate(Dictionary<string, double> variableValues)
        {
            switch (_operator)
            {
                case '+': return _left.Evaluate(variableValues) + _right.Evaluate(variableValues);
                case '-': return _left.Evaluate(variableValues) - _right.Evaluate(variableValues);
                case '*': return _left.Evaluate(variableValues) * _right.Evaluate(variableValues);
                case '/': return _left.Evaluate(variableValues) / _right.Evaluate(variableValues);
                default: throw new Exception("Invalid operator");
            }
        }
    }








}




























