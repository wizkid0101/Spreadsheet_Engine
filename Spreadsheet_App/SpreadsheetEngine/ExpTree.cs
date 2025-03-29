using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//Benjamin Bordon
//ID 011843215
namespace SpreadsheetEngine
{

    public class ExpTree
    {

        private Node _root;
        
        private Dictionary<string, double> _variables = new Dictionary<string, double>();
      
        private List<string> _variableList;

        public ExpTree(string expression)
        {
            BuildTree(expression);
        }

        private void BuildTree(string expression)// this builds the tree using the shunting yard algorithim
        {
            var postfix = ConvertToPostfix(expression);// helper method to convert expression

            Stack<Node> treeStack = new Stack<Node>();// keeps track of the stack on the tree

            foreach (var token in postfix)
            {
                if (double.TryParse(token, out double n))
                {
                    treeStack.Push(new ConstantNode(n));// pushes constants into the stack
                }
                else if (Regex.IsMatch(token, @"^[a-zA-Z][a-zA-Z0-9]*$"))
                {
                    treeStack.Push(new VariableNode(token));// pushes any variable nodes into the stack
                }
                else
                {
                    Node rightOperand = treeStack.Pop();
                    Node leftOperand = treeStack.Pop();
                    treeStack.Push(new OperatorNode(token[0], leftOperand, rightOperand));//pushes operators into the stack
                }

            }



            _root = treeStack.Pop();


        }


        private Queue<string> ConvertToPostfix(string infix)//helper method ti convert exression into postfix form
        {
            Stack<string> operatorStack = new Stack<string>();// operatorstack to keep track of the operators
           
            Queue<string> outputQueue = new Queue<string>();//returns the ouputexpression as a queue

            var tokens = Regex.Split(infix, @"([+*/\-()])").Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();

            foreach (var token in tokens)
            {
                if (double.TryParse(token, out double n) || Regex.IsMatch(token, @"^[a-zA-Z][a-zA-Z0-9]*$")) // Operand
                {
                    outputQueue.Enqueue(token);
                }
                else if ("+-*/".Contains(token)) // Operator
                {
                    while (operatorStack.Count > 0 && GetPrecedence(operatorStack.Peek()) >= GetPrecedence(token))// checks stack precdence
                    {
                        outputQueue.Enqueue(operatorStack.Pop());// pushes the operator onto the queue
                    }
                    operatorStack.Push(token);
                }
                else if (token == "(")// manages parenthesises
                {
                    operatorStack.Push(token);
                }
                else if (token == ")")
                {
                    string topToken = operatorStack.Pop();
                    while (topToken != "(")
                    {
                        outputQueue.Enqueue(topToken);
                        topToken = operatorStack.Pop();
                    }
                }
            }

            while (operatorStack.Count > 0)//while loop to keep pushing operators into the queue
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }

            return outputQueue;//returns the output in postfix form 
        }

        private int GetPrecedence(string token)// checks the precedence of the oeprator
        {
            switch (token)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                default:
                    return 0;
            }
        }

        public void SetVariable(string variableName, double variableValue)// sets the variable into the dictionary
        {
            _variables[variableName] = variableValue;
        }

        public double Evaluate()// evaluate method in the expression tree
        {
            return _root.Evaluate(_variables);
        }


        public List<string> GetVariableNames()
        {
            // If the variable list is null or the tree has been modified, re-collect variable names.
            if (_variableList == null)
            {
                _variableList = new List<string>();
                CollectVariableNames(_root, _variableList);
            }

            return _variableList;
        }

        private void CollectVariableNames(Node node, List<string> variableNames)
        {
            // Base case: if node is null, return.
            if (node == null)
            {
                return;
            }

            // Check if the current node is a VariableNode.
            if (node is VariableNode variableNode)
            {
                // Avoid adding duplicates.
                if (!variableNames.Contains(variableNode.VariableName))
                {
                    variableNames.Add(variableNode.VariableName);
                }
            }
            // If it's an OperatorNode, we continue the traversal.
            else if (node is OperatorNode operatorNode)
            {
                CollectVariableNames(operatorNode.Left, variableNames);
                
                CollectVariableNames(operatorNode.Right, variableNames);
            }
            // No need to handle ConstantNode as it does not contain variable names.
        }








    }

}

 










    
























