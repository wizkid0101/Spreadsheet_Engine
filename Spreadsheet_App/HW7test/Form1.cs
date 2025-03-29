



using Microsoft.VisualBasic;
using SpreadsheetEngine;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

// Benjamin Bordon ID: 011843215
namespace Spreadsheet_Benjamin_Bordon
{
    public partial class Form1 : Form
    {
        private Spreadsheet spreadsheet;
        private int rows = 50;
        private int cols = 26;

        private UndoRedoManager undoRedoManager;

        public Form1() //default form 1 constructor
        {
            InitializeComponent();

            spreadsheet = new Spreadsheet(rows, cols);// intialize new spreadsheet
          
            InitializeDataGridView();

        }

        private void InitializeDataGridView()
        {
            // Add columns A-Z
            for (char c = 'A'; c < 'A' + cols; c++)
            {
                dataGridView1.Columns.Add(c.ToString(), c.ToString());
            }

            // Add rows
            dataGridView1.Rows.Add(rows);

            // Bind cell properties to DataGridView
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var cell = spreadsheet.GetCell(i, j);

                    dataGridView1.Rows[i].Cells[j].Value = cell.Value;

                    cell.PropertyChanged += Cell_PropertyChanged;//subsribes the cells to the spreadsheet
                }

            }

            dataGridView1.CellBeginEdit += DataGridView1_CellBeginEdit;// handles editing before

            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;// and after


        }

        private void DataGridView1_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {

            // Update the spreadsheet cell's Text with the new value from the DataGridView
            var cell = spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
           
           string newText = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
           
            cell.Text = newText; // This should trigger re-evaluation in the cell

            // Update the DataGridView cell to show the evaluated Value
            dataGridView1[e.ColumnIndex, e.RowIndex].Value = cell.Value;
        }





    
        private void DataGridView1_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {

            // Show the Text property (which may be a formula) for editing
            var cell = spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);

            dataGridView1[e.ColumnIndex, e.RowIndex].Value = cell.Text;





        }

       





        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }




        private void Cell_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {


            if (sender is InstanceCell cell)
            {
                var gridCell = dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex];
                if (e.PropertyName == "Value")
                {
                    // Update DataGridView with the new value
                    gridCell.Value = cell.Value;

                    // Notify dependent cells to update their values
                    spreadsheet.NotifyDependentCells(cell);
                }
                else if (e.PropertyName == "BGColor")
                {
                    gridCell.Style.BackColor = Color.FromArgb((int)cell.BGColor);
                }


            }
         
        
        
        
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }

}



