using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace SURLY
{
    public class Database
    {
        //The Tuple holds the Table Name and the actual table (Relation)
        public List<Tuple<string, Relation>> Tables { get; set; }
        //Speach synth to help with the demo
        private SpeechSynthesizer synth = new SpeechSynthesizer();

        public Database()
        {
            Tables = new List<Tuple<string, Relation>>();
        }

        public void createTable(string tableName, string[] attributeNames, string[] attributeTypes, int[] attributeLengths, bool[] nullable)
        {
            List<ColumnProperties> columns = new List<ColumnProperties>();

            //Add the desired amount of columns
            for (int i = 0; i < attributeTypes.Length; i++)
                columns.Add(new ColumnProperties(attributeNames[i], attributeTypes[i], attributeLengths[i], nullable[i]));

            //Grab a new relation with no rows and add it to "Tables"
            Relation table = new Relation(columns);
            Tables.Add(Tuple.Create<string, Relation>(tableName, table));
        }

        public void destroy(string tableName)
        {
            //grab the table that they specified
            var destroyed = Tables.FirstOrDefault(x => x.Item1 == tableName);
            if (destroyed != null)
            {
                //remove it from the list if it is found
                Tables.Remove(destroyed);
            }
        }

        public void insert(string tableName, object[] item)
        {
            //find the table that they want to insert into
            var table = Tables.FirstOrDefault(x => x.Item1.ToLower() == tableName.ToLower());

            if (table != null)
            {
                //put it into a temp variable until we know that all the columns are in the correct format
                var tempTable = table.Item2.Columns.ToArray();
                Row temp = new Row(tempTable.Length);

                //Make sure they gave us the right amount of columns
                if (tempTable.Length != item.Length)
                {
                    //Some fun speech synth stuff
                    synth.Speak("Hey! Why you do dat? Put number of thing when supposed to.");
                    return;
                }

                for (int i = 0; i < item.Length; i++)
                {
                    //Make sure that it isn't null if it wasn't suppose too
                    if (!tempTable[i].Nullable && (item[i] == null || item[i].ToString() == ""))
                    {
                        synth.Speak("Hey! Why you do dat? Put things there when supposed to.");
                        return;
                    }//Make sure that if it is null that we don't go through the other checks. 
                    //the array at the end will get incremented and the one that was missed will be put to null anyway
                    else if (!(item[i] == null || item[i].ToString() == ""))
                    {
                        //Make sure the data is in the correct length
                        if (item[i].ToString().Length > tempTable[i].Length)
                        {
                            synth.Speak("Hey! Why you do dat? Make things correct size when supposed to.");
                            return;
                        }
                        //Make sure the Item is the correct type that they specified
                        if (checkType(item[i].GetType().ToString(), tempTable[i].Type))
                        {
                            synth.Speak("Hey! Why you do dat? Make things correct type when supposed to.");
                            return;
                        }
                        //add it to the array if passed all those checks
                        temp.Cellsssss[i] = item[i];
                    }
                }
                //put it to the table if every column for that row passed inspection
                table.Item2.Rows.Add(temp);
            }
        }

        public void delete(string tableName)
        {
            //find the table
            var deleted = Tables.FirstOrDefault(x => x.Item1 == tableName);

            if (deleted != null)
            {
                //delete everything inside the table
                deleted.Item2.Rows.RemoveAll(x => true);
            }
        }

        public void delete(string tableName, string whereColumn, string whereValue)
        {
            //find the table
            var deleted = Tables.FirstOrDefault(x => x.Item1.ToLower() == tableName.ToLower());

            if (deleted != null)
            {
                int tempColumnIndexes = 0;

                foreach (var column in deleted.Item2.Columns)
                {
                    //This for loop gets the index of the WHERE column
                    if (column.Name.ToLower().Equals(whereColumn.ToString().ToLower()))
                    {
                        tempColumnIndexes = deleted.Item2.Columns.IndexOf(column);
                    }
                }

                deleted.Item2.Rows.RemoveAll(x => x.Cellsssss[tempColumnIndexes].ToString().ToLower() == whereValue.ToLower());
            }
        }

        public Relation union(string table1, string table2, string column1, string column2)
        {
            var tempColumns = new List<ColumnProperties>();
            var tempColumnIndex1 = 0;
            var tempColumnIndex2 = 0;

            var tempTable1 = Tables.FirstOrDefault(x => x.Item1 == table1);
            foreach (var column in tempTable1.Item2.Columns)
            {
                tempColumns.Add(column);
                if (column.Name.Equals(column1))
                {
                    tempColumnIndex1 = tempTable1.Item2.Columns.IndexOf(column);
                }
            }
            var tempTable2 = Tables.FirstOrDefault(x => x.Item1 == table2);
            foreach (var column in tempTable2.Item2.Columns)
            {
                tempColumns.Add(column);
                if (column.Name.Equals(column2))
                {
                    tempColumnIndex2 = tempTable2.Item2.Columns.IndexOf(column);
                }
            }

            Relation tempRelation = new Relation(tempColumns);

            foreach (var row1 in tempTable1.Item2.Rows)
            {
                var tempRowsFromTable = new List<Row>();
                for (int i = 0; i < tempTable2.Item2.Rows.Count; i++)
                {
                    if (row1.Cellsssss[tempColumnIndex1].ToString().Equals(tempTable2.Item2.Rows[i].Cellsssss[tempColumnIndex2].ToString()))
                    {
                        tempRowsFromTable.Add(tempTable2.Item2.Rows[i]);
                    }
                }
                //var temp = tempTable2.Item2.Rows.Where(x => x.Cellsssss[tempColumnIndex2] == row1.Cellsssss[tempColumnIndex1]);
                foreach (var table in tempRowsFromTable)
                {
                    var tempRow = new Row(tempRelation.Columns.Count);
                    for (int i = 0; i < row1.Cellsssss.Length; i++)
                    {
                        tempRow.Cellsssss[i] = row1.Cellsssss[i];
                    }
                    for (int i = 0, j = row1.Cellsssss.Length; i < table.Cellsssss.Length; i++, j++)
                    {
                        tempRow.Cellsssss[j] = table.Cellsssss[i];
                    }
                    tempRelation.Rows.Add(tempRow);
                }
            }
            return tempRelation;

        }

        public void update(string tableName, string[] columnUpdate, string[] valueUpdate, string whereColumn, string whereValue)
        {
            var table = Tables.FirstOrDefault(x => x.Item1.ToLower() == tableName.ToLower());
            int tempColumnIndexes = 0;
            List<int> tempUpdateIndexes = new List<int>();
            
            foreach (var column in table.Item2.Columns)
            {
                //This for loop gets the index of the WHERE column
                if (column.Name.ToLower().Equals(whereColumn.ToString().ToLower()))
                {
                    tempColumnIndexes = table.Item2.Columns.IndexOf(column);
                }
                //This loop gets the index of th UPDATE columns
                foreach (var update in columnUpdate)
                {
                    if (column.Name.ToLower().Equals(update.ToString().ToLower()))
                    {
                        tempUpdateIndexes.Add(table.Item2.Columns.IndexOf(column));
                    }
                }
            }

            var things = table.Item2.Rows.Where(x => x.Cellsssss[tempColumnIndexes].ToString().ToLower() == whereValue.ToString().ToLower());
            foreach (var row in things)
            {
                foreach (var item in tempUpdateIndexes)
                {
                    row.Cellsssss[item] = valueUpdate[tempUpdateIndexes.IndexOf(item)];
                }
            }

        }
        public void update(string tableName, string[] columnUpdate, string[] valueUpdate)
        {
            var table = Tables.FirstOrDefault(x => x.Item1.ToLower() == tableName.ToLower());
            List<int> tempUpdateIndexes = new List<int>();
            
            foreach (var column in table.Item2.Columns)
            {
                //This loop gets the index of th UPDATE columns
                foreach (var update in columnUpdate)
                {
                    if (column.Name.ToLower().Equals(update.ToString().ToLower()))
                    {
                        tempUpdateIndexes.Add(table.Item2.Columns.IndexOf(column));
                    }
                }
            }

            foreach (var row in table.Item2.Rows)
            {
                foreach (var item in tempUpdateIndexes)
                {
                    row.Cellsssss[item] = valueUpdate[tempUpdateIndexes.IndexOf(item)];
                }
            }



        }
        public Relation select(string tableName, string column, string value)
        {
            var tempColumns = new List<ColumnProperties>();
            var tempColumnIndex = 0;
            var table = Tables.FirstOrDefault(x => x.Item1.ToLower() == tableName);
            if (table == null) return null;

            foreach (var columnz in table.Item2.Columns)
            {
                tempColumns.Add(columnz);
                if (columnz.Name.Equals(column))
                {
                    tempColumnIndex = table.Item2.Columns.IndexOf(columnz);
                }
            }
            Relation tempRelation = new Relation(tempColumns);
            var selectedRelation =
                table.Item2.Rows.Where(x => x.Cellsssss[tempColumnIndex].ToString().ToLower() == value);
            foreach (var row in selectedRelation)
            {
                tempRelation.Rows.Add(row);
            }
            return tempRelation;
        }



        public void print(string tableName)
        {
            //find the table
            var table = Tables.FirstOrDefault(x => x.Item1.ToLower() == tableName.ToLower());
            if (table != null)
            {
                print(table.Item2);
            }
        }

        public void print(Relation rel)
        {
            //For nice formatting
            Console.WriteLine("\n____________________________________________________________________");
            if (rel == null) return;
            //Go through all the columns and format it for the console
            for (int y = 0; y < rel.Columns.Count(); y++)
                Console.Write("{0, " + (2 * y + 7) + "} | ", rel.Columns[y].Name);

            Console.WriteLine("\n____________________________________________________________________");

            foreach (var row in rel.Rows)
            {
                //do the same for all the cells in the row
                for (int j = 0; j < row.Cellsssss.Count(); j++)
                {
                    try
                    {
                        Console.Write("{0, " + (2 * j + 7) + "} | ", row.Cellsssss[j].ToString());
                    }
                    catch //This only happens when there is a null value in the table
                    {
                        Console.Write("{0, " + (2 * j + 7) + "} |", "Null");
                    }
                }
                //Write a line for nicer formatting on the console
                Console.WriteLine();
            }
        }

        private Boolean checkType(string typeOfInput, string typeOfNeeded)
        {
            //Checks the type of the two strings. 
            //The "Type of input" should be the GetType().toString() of the object that is needs to be checked.
            //The "Type of needed" is the one that was specified in the columns when they were first made
            switch (typeOfInput)
            {
                case "System.Int32":
                    return ((typeOfNeeded.ToLower().Equals("int") |
                                typeOfNeeded.ToLower().Equals("integer")) ? false : true);
                case "System.String":
                    return ((typeOfNeeded.ToLower().Equals("string")) ? false : true);
                case "System.Boolean":
                    return ((typeOfNeeded.ToLower().Equals("boolean") |
                               typeOfNeeded.ToLower().Equals("bool")) ? false : true);
                default:
                    return false;
            }
        }


        #region parse
        public void parse(string statementFull)
        {
            var splitStatements = statementFull.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string statement in splitStatements)
            {
                var command = statement.Substring(0, 11);
                if (command.ToLower().Contains("insert"))
                {
                    parseInsert(statement);
                }
                else if (command.ToLower().Contains("select"))
                {
                    if (statement.ToLower().Contains("where"))
                    {
                        parseSelect(statement);
                    }
                    else
                    {
                        parsePrint(statement);
                    }
                }
                else if (command.ToLower().Contains("create"))
                {
                    parseCreate(statement);
                }
                else if (command.ToLower().Contains("update"))
                {
                    parseUpdate(statement);
                }
                else if (command.ToLower().Contains("delete"))
                {

                    parseDelete(statement);

                }
            }
        }

        private void parseInsert(string statement)
        {
            statement = statement.Remove(0, 11);
            var split = Regex.Split(statement, " values ",RegexOptions.IgnoreCase);

            var tableName = split[0].Trim();

            var values = split[1].Split(',');
            List<Object> tempObjects = new List<Object>();
            foreach (var item in values)
            {
                var addedItem = item.Trim();
                tempObjects.Add(tryCast(addedItem));
            }
            insert(tableName, tempObjects.ToArray());
        }

        private void parseSelect(string statement)
        {

            var split = statement.ToLower().Split(new string[] { " from " }, StringSplitOptions.None);

            if (split.Length > 1)
            {
                var tableWhere = split[1].ToLower().Split(new string[] { "where" }, StringSplitOptions.None);

                if (tableWhere.Length > 1)
                {
                    var tableName = tableWhere[0].Trim();

                    var whereClause = tableWhere[1].ToLower().Split(new string[] { "=" }, StringSplitOptions.None);

                    if (whereClause.Length > 1)
                    {
                        print(select(tableName, whereClause[0].Trim(), whereClause[1].Trim()));
                    }
                }
            }
        }

        private void parsePrint(string statement)
        {
            var split = statement.ToLower().Split(new string[] { " from " }, StringSplitOptions.None);
            if (split.Length > 1)
            {
                print(split[1].Trim());
            }
        }

        private void parseCreate(string statement)
        {
            var split = Regex.Split(statement, " table ", RegexOptions.IgnoreCase);

            if (split.Length > 1)
            {
                var tableInfo = split[1].Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                var tableName = tableInfo[0].Trim();

                if (tableInfo.Length > 1)
                {
                    var createColumns = Regex.Split(tableInfo[1], ",", RegexOptions.IgnoreCase);

                    //Lists for create method
                    List<string> attributeNames = new List<string>();
                    List<string> attributeTypes = new List<string>();
                    List<int> attributeLength = new List<int>();
                    List<bool> nullable = new List<bool>();
                    foreach (var column in createColumns)
                    {
                        try
                        {


                            var columnSplit = column.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            int length;
                            bool isNull;
                            attributeNames.Add(columnSplit[0]);
                            attributeTypes.Add(columnSplit[1]);
                            try
                            {
                                length = Convert.ToInt32(columnSplit[2]);
                            }
                            catch
                            {
                                throw new Exception("Couldn't convert the length field to number");
                            }
                            try
                            {
                                isNull = Convert.ToBoolean(columnSplit[3]);

                            }
                            catch
                            {
                                throw new Exception("Couldn't convert the null field to boolean");
                            }

                            attributeLength.Add(length);
                            nullable.Add(isNull);
                        }
                        catch
                        {
                            throw new Exception("Couldn't parse Create input");
                        }
                    }
                    createTable(tableName, attributeNames.ToArray(),
                                attributeTypes.ToArray(), attributeLength.ToArray(),
                                nullable.ToArray());
                }
            }
        }

        private void parseDelete(string statement)
        {
            var split = statement.ToLower().Split(new string[] { "from" }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 1)
            {
                var whereSplit = split[1].ToLower().Split(new string[] { "where" }, StringSplitOptions.RemoveEmptyEntries);
                if (whereSplit.Length > 1)
                {
                    var whereValue = whereSplit[1].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    delete(whereSplit[0].Trim(), whereValue[0].Trim(), whereValue[1].Trim());
                }
                else
                {
                    delete(split[1].Trim());
                }
            }

        }

        private void parseUpdate(string statement)
        {
            var split = statement.Trim().Remove(0, 6);
            var setWhereSplit = Regex.Split(split, " set ", RegexOptions.IgnoreCase);

            if (setWhereSplit.Length > 1)
            {
                var tableName = setWhereSplit[0].Trim();
                var setSplit = Regex.Split(setWhereSplit[1], "where", RegexOptions.IgnoreCase);

                var valuesColumns = setSplit[0].Split(',');

                List<string> updateColumns = new List<string>();
                List<string> updateValues = new List<string>();

                foreach (var update in valuesColumns)
                {
                    var valueSplit = update.Split(new string[] {"="}, StringSplitOptions.RemoveEmptyEntries);

                    updateColumns.Add(valueSplit[0].Trim());
                    updateValues.Add(valueSplit[1].Trim());
                }

                if (setSplit.Length > 1)
                {
                    var whereSplit = setSplit[1].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    update(tableName, updateColumns.ToArray(), updateValues.ToArray(), whereSplit[0].Trim(), whereSplit[1].Trim());
                }
                else
                {
                    update(tableName, updateColumns.ToArray(), updateValues.ToArray());

                }
            }
        }
        #endregion

        private Object tryCast(string input)
        {
            try
            {
                return Convert.ToInt32(input);
            }
            catch
            {
                try
                {
                    return Convert.ToBoolean(input);
                }
                catch
                {
                    return input;
                }
            }
        }
    }
}
