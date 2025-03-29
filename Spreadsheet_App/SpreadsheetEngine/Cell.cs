using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using static System.Net.Mime.MediaTypeNames;
//Benjmain Bordon,ID:011843215
//https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged?view=net-7.0

namespace SpreadsheetEngine
{

    public abstract class Cell : INotifyPropertyChanged

    {

        // public fields for row and column indices
        public int ColumnIndex { get; }
        public int RowIndex { get; }

        // Protected fields for the Text and Value properties
        protected string value;

        protected string text;

        protected string cellName;


        private uint bgColor = 0xFFFFFFFF; // Default to white

        public uint BGColor
        {
            get => bgColor;
            set
            {
                if (bgColor != value)
                {
                    bgColor = value;
                    OnPropertyChanged(nameof(BGColor));
                }
            }
        }




        protected Cell(int rowIndex, int columnIndex)// protocted cell constructor 
        {
            RowIndex = rowIndex;
            
            ColumnIndex = columnIndex;


            text = string.Empty; // Initialize text as empty string
            value = string.Empty; // Initialize value as empty string
            bgColor = 0xFFFFFFFF; // Default background color (white)



        }
        public string Text// text method to inkoke a property change if the text were to change
        {
            get => text;

            set// if text is empty
            {
                if (text != value)
                {

                    text = value;

                    OnPropertyChanged(nameof(Text));

                }
            }

        }

        // Property to make it so that value cannot be modified outside the class itself
        public string Value
        {
            get => value;
           
            internal set
            {
                if (value != this.value)
                {
                   
                    
                    this.value = value;
                    
                    
                    OnPropertyChanged(nameof(Value));
                }
            }

        }


        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsDefault()// method to check if a cell is empty or default
        {
            return BGColor == 0xFFFFFFFF && string.IsNullOrEmpty(Text);
        }


        public void ClearCells()
        {
            
            Text = string.Empty;
            BGColor = 0xFFFFFFFF; 
        }


    }

    public class InstanceCell : Cell// public class to create a instance of the cell declared outside of the cell class
    {
        private Spreadsheet spread;
       
        private ExpTree expressionTree;
        public InstanceCell(int rowIndex, int columnIndex) : base(rowIndex, columnIndex)// inherits the base constructor of the cell class
        {

            

        }


        public double GetCellValue(string Cellname)// returns cell value
        {
            return spread.GetCellValue(Cellname);
            
        }

     

        public string Text// handles the text if the text were to change 
        {
            get => base.text;

            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    OnPropertyChanged(nameof(Text));

                }
                else if(base.text.StartsWith("="))
                {
                    EvaluateExpression();

                }





            }
        }
        public void EvaluateExpression()
        {
            if (text.StartsWith("="))
            {
                // Remove the '=' before parsing the expression.
                string expression = text.Substring(1);
                
                expressionTree = new ExpTree(expression);

                // Get the variable names from the expression.
                var variableNames = expressionTree.GetVariableNames();

                // Now set the values of these variables in the expression tree.
                foreach (var varName in variableNames)
                {
                    // You would need a method to get the current value of the cell corresponding to varName.
                    double value = GetCellValue(varName);
                   
                    expressionTree.SetVariable(varName, value);
                }

                // Evaluate the expression and set the cell's value.
                this.Value = expressionTree.Evaluate().ToString();
               
                OnPropertyChanged(nameof(Value));
            }


        }







    }
}






   


    




