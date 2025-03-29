using SpreadsheetEngine;
using System.Xml.Linq;

namespace HW9_testing
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

            var Testspreadsheet = new Spreadsheet(5, 5); // for example, 5x5 spreadsheet
                                                         // Set up some cells with known values and properties
            Testspreadsheet.GetCell(0, 0).Text = "Hello";
            Testspreadsheet.GetCell(0, 0).BGColor = 0xFF0000; // Example: Red



        }

        [Test]

        public void testSavemethod()
        {
            var spreadsheet = CreateTestSpreadsheet();
            using (var stream = new MemoryStream())
            {
                spreadsheet.SaveFile(stream);

                // Rewind the stream to read from the beginning
                stream.Seek(0, SeekOrigin.Begin);


                var xmlDoc = XDocument.Load(stream);
                var cells = xmlDoc.Root.Elements("cell").ToList();

                Assert.AreEqual(1999, cells.Count);// assert to ensure that the cells count is correct

                // Assert content of a specific cell, e.g., A1
                var cellA1 = cells.FirstOrDefault(c => c.Attribute("name").Value == "A1");
                Assert.IsNotNull(cellA1, "Cell A1 should be present in the saved XML.");






            }

            Spreadsheet CreateTestSpreadsheet()
            {
                var spreadsheet = new Spreadsheet(5, 5); // for example, 5x5 spreadsheet
                                                         // Set up some cells with known values and properties
                spreadsheet.GetCell(0, 0).Text = "Hello";
                spreadsheet.GetCell(0, 0).BGColor = 0xFF0000; // Example: Red
                                                              // ... set up other cells as needed for testing
                return spreadsheet;
            }






        }
    }

}






