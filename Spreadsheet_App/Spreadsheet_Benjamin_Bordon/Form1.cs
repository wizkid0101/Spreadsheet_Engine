



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

            undoRedoManager = new UndoRedoManager();

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

                    dataGridView1.Rows[i].Cells[j].Value = cell.Value;//sets the datatgridviews values to the cell Values

                    cell.PropertyChanged += Cell_PropertyChanged;//subsribes the cells to the spreadsheet
                }

            }


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.HeaderCell.Value = (row.Index + 1).ToString();

            }



            dataGridView1.CellBeginEdit += DataGridView1_CellBeginEdit;// handles editing before

            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;// and after


        }

        private void DataGridView1_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            var cell = spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
            string oldText = cell.Text; // Store the old text for undo
            string newText = dataGridView1[e.ColumnIndex, e.RowIndex].Value?.ToString() ?? string.Empty;

            // Check if the text actually changed to avoid unnecessary commands
            if (oldText != newText)
            {
                // Create and execute the ChangeTextCommand
                ICommand command = new CommandTextChanges(cell, oldText, newText);
                undoRedoManager.ExecuteCommand(command);


            }
            //Update the cell's text in the spreadsheet logic

            cell.Text = newText;


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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void changeBGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Color selectedColor = colorDialog.Color;
                    foreach (DataGridViewCell gridCell in dataGridView1.SelectedCells)
                    {
                        // Assuming you have a way to map DataGridView cells to your logic layer cells
                        var cell = spreadsheet.GetCell(gridCell.RowIndex, gridCell.ColumnIndex);

                        // Store the old color for undo functionality
                        uint oldColor = cell.BGColor;

                        // Set the new background color
                        cell.BGColor = (uint)selectedColor.ToArgb();

                        // Change the DataGridView cell's background color
                        gridCell.Style.BackColor = selectedColor;

                        undoRedoManager.ExecuteCommand(new ChangeBGColorCommand(cell, oldColor, cell.BGColor));
                    }
                }




            }

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)// redo menu item
        {
            if (undoRedoManager.CanRedo)
            {
                undoRedoManager.Redo();

                UpdateUndoRedoMenuItems();// updates the ui text in real time

            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoRedoManager.CanUndo)// checks if the undo action can be performed
            {
                undoRedoManager.Undo();

                UpdateUndoRedoMenuItems();// updates the menuitems in real time
            }
        }

        private void UpdateUndoRedoMenuItems()
        {
            undoToolStripMenuItem.Text = undoRedoManager.UndoStackSize > 0//checks if the undo stack is not empty

           ? $"Undo {undoRedoManager.UndoDescription}"//updates the description of the undo menu item

           : "Undo";

            RedoTool.Text = undoRedoManager.UndoStackSize > 0// checks if the redo stack is empty 

             ? $"Redo {undoRedoManager.RedoDescription}"//updates the redo text to match the item that need to be redone

             : "Redo";

        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)//Save grid cells
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        spreadsheet.SaveFile(stream);
                    }
                }
            }



        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)// Load Grid cells
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = openFileDialog.OpenFile())
                    {
                        spreadsheet.loadFile(stream);

                    }
                       
                }


            }
        }

    }

}


