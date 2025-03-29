using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Benjmamin Bordon
//ID :011843215
namespace SpreadsheetEngine
{
    public class UndoRedoManager
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();// stack undo actions
       
        private Stack<ICommand> redoStack = new Stack<ICommand>();//stack redo actions

        public string UndoDescription => undoStack.Count > 0 ? undoStack.Peek().Description : string.Empty;// this checks the type in the undostack
        public string RedoDescription => redoStack.Count > 0 ? redoStack.Peek().Description : string.Empty;//This checks the type in the redostack

        public int UndoStackSize => undoStack.Count;//returns the stack size of the undo action
        public int RedoStackSize => redoStack.Count;// return the stack size of the redo action
        public bool CanUndo => undoStack.Count > 0;// checks if the count is greater than zero
        public bool CanRedo => redoStack.Count > 0;//checks if the redo  stack is greater than zero

        public void ExecuteCommand(ICommand command)// excutes the commmand it is given
        {
            command.Execute();
            
            undoStack.Push(command);
            
            redoStack.Clear(); // Clear redo stack on new command execution
        }

        public void Undo()// Undo method dervied from the interface which pops a element of its stack and repushes into theredostack
        {
            if (undoStack.Count > 0)
            {
                var command = undoStack.Pop();
                
                command.Undo();
               
                redoStack.Push(command);
            }
        }

        public void Redo()// Redo method which is pops of the redo stack and pushes into the undostack
        {
            if (redoStack.Count > 0)
            {
                var command = redoStack.Pop();
                
                command.Execute();
                
                undoStack.Push(command);
            }
        }



        public void clearStacks()
        {  if(undoStack.Count>0&& redoStack.Count>0)
            {

                undoStack.Clear();

                redoStack.Clear();
            }


        }


    }

    public interface ICommand
    {
        void Execute();// execute or redo command
        void Undo();// undo command

        string Description { get; }// propety descriptioon
    }

}
