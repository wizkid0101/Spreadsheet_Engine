using SpreadsheetEngine;

namespace HW4_UnitTested
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Cell_Constructor_Test()// test the cell constructor 
        {
            // Arrange
            int expectedRowIndex = 1;
            int expectedColumnIndex = 2;

            // Act
            var cell = new InstanceCell(expectedRowIndex, expectedColumnIndex); 

            // Assert
            Assert.AreEqual(expectedRowIndex, cell.RowIndex);
            Assert.AreEqual(expectedColumnIndex, cell.ColumnIndex);

        }

        [Test]
        public void SpreadsheetConstructorTest()// test spreadsheet constructor
        {
            int expectedRowIndex = 50;
            int expectedColIndex = 26;
            var spreadsheet = new Spreadsheet(expectedRowIndex, expectedColIndex);
            Assert.AreEqual(expectedRowIndex, spreadsheet.RowCount);
            Assert.AreEqual(expectedColIndex, spreadsheet.ColumnCount);

        }
        [Test]
        public void testEventHanlderLogic()// test eventhandlerlogic to ensure that it is functioning as intended
        {
            var cell = new InstanceCell(0, 0);

            
            string expectedText = "Testing";

            string propertyNametest = null;

            cell.PropertyChanged += (sender, args) => { propertyNametest = args.PropertyName; };

            cell.Text = expectedText;
            // Assert
            Assert.AreEqual(expectedText, cell.Text);
            Assert.AreEqual("Text",propertyNametest );

        }
    
    
    
    
    
    
    }

}

    
    
    
    
    
    
    
    




