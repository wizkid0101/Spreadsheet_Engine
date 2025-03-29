using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    public class CommandTextChanges:ICommand
    {
        private Cell cell;
        private string oldValue;
        private string newValue;

        public CommandTextChanges(Cell cell, string oldValue, string newValue)
        {
            this.cell = cell;
            
            this.oldValue = oldValue;
            
            this.newValue = newValue;
        }

        public void Execute()
        {
            cell.Text = newValue;// sets text property to the new text value
        }

        public void Undo()
        {
            cell.Text = oldValue;// sets text property to the old text 
        }

        public string Description => "TextChange";//updates the textcomamand descrption




    }

    public class ChangeBGColorCommand : ICommand
    {
        private Cell cell;// cell object
       
        private uint oldColor;// old color property
      
        private uint newColor;//new color property

        public ChangeBGColorCommand(Cell cell, uint oldColor, uint newColor)
        {
            this.cell = cell;
            this.oldColor = oldColor;
            this.newColor = newColor;
        }

        public void Execute()
        {
            cell.BGColor = newColor;// sets color property to the newcolor
        }

        public void Undo()
        {
            cell.BGColor = oldColor;// sets color property to the old color
        }
        
        
        public string Description => "ColorChange";// chnages command text in the UI


    }













}



