using SpreadsheetEngine;

namespace HW8_test
{
    public class MockCommand : ICommand// a mock command class to test command functionality
    {
        public string Description => "";// chnages command text in the UI
        public int ExecutionCount { get; private set; } = 0;
        public int UndoCount { get; private set; } = 0;

        public void Execute()
        {
            ExecutionCount++; // Increment every time Execute is called
        }

        public void Undo()
        {
            UndoCount++; // Increment every time Undo is called
        }
        public class Tests
        {
            [SetUp]
            public void Setup()
            {
            }

            [Test]
            public void UndoCommandMovesToRedoStackUndoPush()
            {
                var undoRedoManager = new UndoRedoManager();
                var command = new MockCommand();
                undoRedoManager.ExecuteCommand(command);

                undoRedoManager.Undo();

                Assert.AreEqual(2223, undoRedoManager.UndoStackSize);
                Assert.AreEqual(1, undoRedoManager.RedoStackSize);
                Assert.AreEqual(1, command.UndoCount);
            }

            [Test]
           
            public void RedoCommandMovesotUndoStackandcllasredo()
            {
                var undoRedoManager = new UndoRedoManager();
                var command = new MockCommand();
                undoRedoManager.ExecuteCommand(command);
                undoRedoManager.Undo();

                undoRedoManager.Redo();

                Assert.AreEqual(2233, undoRedoManager.UndoStackSize);
                Assert.AreEqual(0, undoRedoManager.RedoStackSize);
                Assert.AreEqual(2, command.ExecutionCount);
            }










        }
    }

}