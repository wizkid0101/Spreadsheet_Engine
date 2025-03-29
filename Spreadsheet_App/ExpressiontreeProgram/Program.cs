

using SpreadsheetEngine;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        ExpTree exprTree = new ExpTree("A1+B1+C1"); // default

        while (true)
        {
            Console.WriteLine("Options:");
            Console.WriteLine("1. Enter expression");
            Console.WriteLine("2. Set variable value");
            Console.WriteLine("3. Evaluate expression");
            Console.WriteLine("4. Quit");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter expression: ");
                    string expr = Console.ReadLine();
                    exprTree = new ExpTree(expr);
                    break;

                case "2":
                    Console.Write("Enter variable name: ");
                    string varName = Console.ReadLine();
                    Console.Write($"Enter value for {varName}: ");
                    if (double.TryParse(Console.ReadLine(), out double value))
                    {
                        exprTree.SetVariable(varName, value);
                    }
                    else
                    {
                        Console.WriteLine("Invalid value. Try again.");
                    }
                    break;

                case "3":
                    try
                    {
                        Console.WriteLine($"Value: {exprTree.Evaluate()}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                    break;

                case "4":
                    return;

                default:
                    // Ignoring invalid options
                    break;
            }
        }
    }
}





















































