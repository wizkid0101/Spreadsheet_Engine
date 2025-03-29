using SpreadsheetEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpreadsheetEngine

{   //Benjamin Bordon ID: 011843215
    /* this custom event class helps us maintain better encapsulation by passing information if a cells property information were to change
    in the case of the cellPropetyChange event this is especially useful when dealing with the helper method
    */
    public class CellPropertyChangedEventArgs : EventArgs 
    {
        public Cell Cell { get; }// gets cell object
        public string PropertyName { get; }// gets property name

        public CellPropertyChangedEventArgs(Cell cell, string propertyName)
        {
            Cell = cell;
            PropertyName = propertyName;
        }


    }
    
    public class Spreadsheet
    {

        public event EventHandler<CellPropertyChangedEventArgs> CellPropertyChanged;//eventhandler delegate

        private int rowCount;// keeps track of the rows

        private int colCount;//keeps track of the columns

       private  InstanceCell[,] spreadsheetCell;// Instance cell object to keep track of cell data

        private Dictionary<string, List<InstanceCell>> dependencies;

        UndoRedoManager undoRedo;

        public int RowCount// returns row count 
        {
            get { return this.rowCount; }
        }


        public int ColumnCount// returns column count
        {
            get { return this.colCount; }
        }


        // return the cell at the specified location
        public Cell GetCell(int row, int col)
        {

            // Check bounds and return the cell at the specified indices
            if (row >= 0 && row < RowCount && col >= 0 && col < ColumnCount)
            {


                return spreadsheetCell[row, col];


            }
            else
            {

                return null;

            }

        }
        
        
        public Spreadsheet(int newRow, int newCol)// public spreadsheet constructor
        {
            rowCount = newRow;
            
            colCount = newCol;

            spreadsheetCell = new InstanceCell[rowCount, colCount];

            dependencies = new Dictionary<string, List<InstanceCell>>();

            cellInit();// intializes all the cells in the spreadsheet


        }


        private void cellInit()// intializes all the cells in spreadsheet
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    var cell = new InstanceCell(i, j);
                    cell.PropertyChanged += Cell_PropertyChanged;// subsribes to all the cells 

                    spreadsheetCell[i, j] = cell;
                }
            }
        }

        private void Cell_PropertyChanged(object? sender, PropertyChangedEventArgs e)// cell 
        {

            if (sender is InstanceCell cell)
            {
                switch (e.PropertyName)
                {
                    case "Text":
                        // Evaluate the cell's formula if it starts with "="
                        cellEval(cell);


                        // Update the dependencies based on the new text
                        UpdateCellDependencies(cell);


                        // CellPropertyChanged event to notify the UI 

                        CellPropertyChanged?.Invoke(this, new CellPropertyChangedEventArgs(cell, e.PropertyName));

                        // notify dependent cells that they need to update their values

                        NotifyDependentCells(cell);

                        break;

                    case "Value":
                        // If a cell's value changes, notify any cells that depend on this cell
                        NotifyDependentCells(cell);

                        // No need to invoke CellPropertyChanged here since the UI
                        // is already updated in the NotifyDependentCells method.
                        break;
                }

            }

        }


        private void cellEval(InstanceCell cell)
        {
            if (cell.Text.StartsWith("="))
            {
                // Create a new ExpressionTree with the formula (without the '=' prefix)
                ExpTree expTree = new ExpTree(cell.Text.Substring(1));

                // Assuming we have a method to update variable values in the expression tree
                UpdateVariableValues(expTree);

                // Now evaluate the expression
                try
                {
                    double result = expTree.Evaluate();
                   
                    cell.Value = result.ToString(); // Update the cell's Value property with the result
                }
                catch (Exception ex)
                {
                    // If evaluation fails (e.g., due to undefined variables), handle the error
                    cell.Value = "Error";
                }
            }
            else
            {
                // If the cell's text is not a formula, simply copy the Text to the Value
                cell.Value = cell.Text;
                
            }


        }
        
        
        
        
        
        
        private void UpdateVariableValues(ExpTree expTree)// this method updates value in expression tree
        {
            // Go through all variables used in the expression and update their values
            foreach (var varName in expTree.GetVariableNames())
            {
                // Convert the variable name to row and column indices
                
                var (row, col) = ConvertNameToCellCoordinates(varName);
                
                // Retrieve the value from the corresponding cell
                
                var valueCell = GetCell(row, col);
                
                // If valueCell is not null and its value can be parsed as a double update vairable value
                if (valueCell != null && double.TryParse(valueCell.Value, out double value))
                {
                   
                    expTree.SetVariable(varName, value);
                }
                else
                {
                    // throw ex when nessaryy
                    throw new InvalidOperationException($"Cannot find or parse the value of the cell {varName}.");
                }
            }


        }



        private string GetCellName(int rowIndex, int colIndex)
        {
            // Assuming column index 0 corresponds to "A", 1 to "B", etc.
            char colLetter = (char)('A' + colIndex);
          
            int rowNumber = rowIndex + 1; // Assuming row indices are 0-based internally but 1-based in cell names.
            
            return $"{colLetter}{rowNumber}";



        }

     
        
        
        
        private void UpdateCellDependencies(InstanceCell ChangedCell)
        {
            // Clear old dependencies for this cell.
            string cellName = GetCellName(ChangedCell.RowIndex, ChangedCell.ColumnIndex);//gets cell cellname from the row and column index

            if(dependencies.ContainsKey(cellName))
            {
                foreach(InstanceCell dependentCell in dependencies[cellName])// conditional to handle cells that require no dependancies
                {

                   // Remove the changed cell from other cell's dependency list.
                 
                    removeDependency(dependentCell, ChangedCell);
                
                }

            }

           
            dependencies[cellName] = new List<InstanceCell>();//creates a new list of dependancies

            if(ChangedCell.Text.StartsWith("="))//conditional to handle expressions starting with =
            {
                ExpTree expTree = new ExpTree(ChangedCell.Text.Substring(1));

                var variableNames = expTree.GetVariableNames();

                foreach(var variableCell in variableNames)
                {
                    
                    (int dependRow, int dependCol) = ConvertNameToCellCoordinates(variableCell);//converts cell name into spreadsheet cordinates

                    InstanceCell depenadant = (InstanceCell)GetCell(dependRow, dependCol);// gets the cell at those cordinates 

                    AddDependancy(cellName, depenadant);// adds depenedancy to the cells 

                }
            }
        }

        public void NotifyDependentCells(InstanceCell ChangedCell)// notifys the cells to reevaluate there expressions
        {
            string cellName = GetCellName(ChangedCell.RowIndex, ChangedCell.ColumnIndex);// grabs the cellname at those cords

            if(dependencies.ContainsKey(cellName))// if a cell is dependant
            {
                foreach(InstanceCell dep in dependencies[cellName])
                {

                   dep.EvaluateExpression();// force dependent cells to reevaluate there expresses
                }


            }


        }


        // Converts a cell name like "A1" to its corresponding row and column indices (0-based).
        public (int row, int col) ConvertNameToCellCoordinates(string cellName)
        {

            if (string.IsNullOrEmpty(cellName))
            {
                
                
                throw new ArgumentException("Cell name cannot be null.", nameof(cellName));
            
            }
           
            var cellMatch = Regex.Match(cellName.ToUpper(), @"^([A-Z]+)(\d+)$");//extracts row and col elements from cell name

            if (!cellMatch.Success)// if the cellName is not correctly formatted
            {
               
                throw new ArgumentException("Invalid cell name format.", nameof(cellName));
            
            
            }

            int column = 0;
            
            string columnSegment = cellMatch.Groups[1].Value;
            
            string rowSegment = cellMatch.Groups[2].Value;

            // Convert the row part to a zero-based row index by subtracting 1.
            int row = int.Parse(rowSegment) - 1;
           
            for (int i =0; i < columnSegment.Length; i++)
            {
                column *= 26;
                column += (columnSegment[i] - 'A');

            }

            return (row, column);




        }


        private void AddDependancy(string Cellname , InstanceCell instCell)// adds dependants to the list
        {

            if (!dependencies.ContainsKey(Cellname))//  checks if the cell is not already on the list
            {
                dependencies[Cellname] = new List<InstanceCell>();
            }
            if (!dependencies[Cellname].Contains(instCell))//checks if the cell itself is in the list
            {
                dependencies[Cellname].Add(instCell);// adds the cell to dependancy list
            }



        }


        private void removeDependency(InstanceCell cell, InstanceCell dependantCell)// removes cells from the dependancy list
        {
            string cellName = GetCellName(cell.RowIndex, cell.ColumnIndex);
            
            dependencies[cellName].Remove(dependantCell);// removes ghe cell from the list of dependancies 
        }


        public double GetCellValue(string cellName)// gets the value from a cell using its cellname
        {
            var (row, col) = ConvertNameToCellCoordinates(cellName);

            var cell = GetCell(row, col);

            // Assume that the Value is stored as a double in the cell.
           
            // You need to handle cases where the Value might not be a valid double.
            if (double.TryParse(cell.Value, out double value))
            {
                return value;
            }
            else
            {
                // throw an exception or decide on a default return value.
                throw new InvalidOperationException($"The cell value '{cell.Value}' is not a number.");
            }
        }


        public void loadFile(Stream stream)
        {
            var doc = XDocument.Load(stream);//creates a new xml documment 

            ClearSpreadsheet(); // Clear existing data
            
            foreach (var cellElement in doc.Root.Elements("cell"))
            {
                var Cellname = cellElement.Attribute("name")?.Value;// grabs the name of the cell
              
                var (row, col) = ConvertNameToCellCoordinates(Cellname);// using the name gets the cordinates of the cell


                var cell = GetCell(row, col);//gets the cell at those row and column cordinaates 

                var colorElement = cellElement.Element("bgcolor");

                if(colorElement!=null)// checks if the color property is not null
                {
                    cell.BGColor = uint.Parse(colorElement.Value, System.Globalization.NumberStyles.HexNumber);
                }


                var textElement = cellElement.Element("text");

                if (textElement != null)//if the text element is not null then set the cell text the elemnents value
                {
                    cell.Text = textElement.Value;
                }

           
            }
           
            //undoRedo.clearStacks();
        
        }



        public void SaveFile(Stream stream)
        {
            var doc = new XDocument(new XElement("Spreadsheet"));// creates a new Xdocument for spreadsheet

            foreach (var cell in GetAllModifiedCells())// iterates through all the cells that have been modified
            {
                
                var cellElement = new XElement("cell", new XAttribute("name", GetCellName(cell.RowIndex, cell.ColumnIndex)));
                // creates a new  cell Element along with the atributes of the cellName

                if (cell.BGColor != 0xFFFFFFFF)
                   
                    cellElement.Add(new XElement("bgcolor", cell.BGColor.ToString("X8")));

                if (!string.IsNullOrEmpty(cell.Text))// checks if the text is not empty
                   
                   
                    cellElement.Add(new XElement("text", cell.Text));

                
                doc.Root.Add(cellElement);
            }

            doc.Save(stream);

        }

        public IEnumerable<Cell> GetAllModifiedCells()// helper method returns all the cells in the spreadhsheet that have been modified
        {
            List<Cell> nonDefaultCells = new List<Cell>();

            for (int row = 0; row < RowCount; row++)//iterates through the entire spreadsheet
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    var cell = spreadsheetCell[row, col];
                    
                    if (!cell.IsDefault())// checks if a cell is not default
                    {
                       
                        nonDefaultCells.Add(cell);// adds the cells to the list
                    }
                }
            }

            return nonDefaultCells;// return all the cells that are not default
        }

        public void ClearSpreadsheet()// helper method to clear all cells currently on the spreasheet to the default
        {
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    spreadsheetCell[row, col].ClearCells();
                }
            }
        }





    }


}







    
    





