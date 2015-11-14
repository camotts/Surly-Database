using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;

namespace SURLY
{
    public class Database
    {
        private bool isBackup = false;
        //Speach synth to help with the demo
        private SpeechSynthesizer synth = new SpeechSynthesizer();

        public Database()
        {
            Tables = new List<Tuple<string, Relation>>();
            LoadDatabase(@"Database.txt");
        }

        //The Tuple holds the Table Name and the actual table (Relation)
        public List<Tuple<string, Relation>> Tables { get; set; }

        public void createTable(string tableName, string[] attributeNames, string[] attributeTypes,
            int[] attributeLengths, bool[] nullable)
        {
            var columns =
                attributeTypes.Select(
                    (t, i) => new ColumnProperties(attributeNames[i], t, attributeLengths[i], nullable[i])).ToList();

            //Add the desired amount of columns

            //Grab a new relation with no rows and add it to "Tables"
            var table = new Relation(columns);
            Tables.Add(Tuple.Create(tableName, table));
        }

        public void destroy(string tableName)
        {
            //grab the table that they specified
            var destroyed = Tables.FirstOrDefault(x => x.Item1.ToLower() == tableName.ToLower().Trim());
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
                var temp = new Row(tempTable.Length);

                //Make sure they gave us the right amount of columns
                if (tempTable.Length != item.Length)
                {
                    //Some fun speech synth stuff
                    return;
                }

                for (var i = 0; i < item.Length; i++)
                {
                    //Make sure that it isn't null if it wasn't suppose too
                    if (!tempTable[i].Nullable && (item[i] == null || item[i].ToString() == ""))
                    {
                        return;
                    } //Make sure that if it is null that we don't go through the other checks. 
                        //the array at the end will get incremented and the one that was missed will be put to null anyway
                    if (!(item[i] == null || item[i].ToString() == ""))
                    {
                        //Make sure the data is in the correct length
                        if (item[i].ToString().Length > tempTable[i].Length)
                        {
                            return;
                        }
                        //Make sure the Item is the correct type that they specified
                        if (checkType(item[i].GetType().ToString(), tempTable[i].Type))
                        {
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
                var tempColumnIndexes = 0;

                foreach (var column in deleted.Item2.Columns)
                {
                    //This for loop gets the index of the WHERE column
                    if (column.Name.ToLower().Equals(whereColumn.ToLower()))
                    {
                        tempColumnIndexes = deleted.Item2.Columns.IndexOf(column);
                    }
                }

                deleted.Item2.Rows.RemoveAll(
                    x => x.Cellsssss[tempColumnIndexes].ToString().ToLower() == whereValue.ToLower());
            }
        }

        public Relation union(string table1, string table2, string column1, string column2)
        {
            var tempColumns = new List<ColumnProperties>();
            var tempColumnIndex1 = 0;
            var tempColumnIndex2 = 0;

            var tempTable1 = Tables.FirstOrDefault(x => x.Item1.ToLower() == table1.ToLower());
            if (tempTable1 != null)
            {
                //Get the index of the columns and add it to the tempColumns list
                foreach (var column in tempTable1.Item2.Columns)
                {
                    tempColumns.Add(column);
                    if (column.Name.Equals(column1))
                    {
                        tempColumnIndex1 = tempTable1.Item2.Columns.IndexOf(column);
                    }
                }
                var tempTable2 = Tables.FirstOrDefault(x => x.Item1.ToLower() == table2.ToLower());
                if (tempTable2 != null)
                {
                    //get the index of the second table and put it to the list also
                    foreach (var column in tempTable2.Item2.Columns)
                    {
                        tempColumns.Add(column);
                        if (column.Name.Equals(column2))
                        {
                            tempColumnIndex2 = tempTable2.Item2.Columns.IndexOf(column);
                        }
                    }

                    //Make a new relation from the list
                    var tempRelation = new Relation(tempColumns);

                    //Grab all the rows that match and put it to the relation 
                    foreach (var row1 in tempTable1.Item2.Rows)
                    {
                        //Grab all the rows from table 2
                        var tempRowsFromTable = new List<Row>();
                        for (var i = 0; i < tempTable2.Item2.Rows.Count; i++)
                        {
                            if (
                                row1.Cellsssss[tempColumnIndex1].ToString()
                                    .Equals(tempTable2.Item2.Rows[i].Cellsssss[tempColumnIndex2].ToString()))
                            {
                                tempRowsFromTable.Add(tempTable2.Item2.Rows[i]);
                            }
                        }

                        //Find all the values that match and put it to the relation
                        foreach (var table in tempRowsFromTable)
                        {
                            var tempRow = new Row(tempRelation.Columns.Count);
                            for (var i = 0; i < row1.Cellsssss.Length; i++)
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
            }
            return null;
        }

        public void update(string tableName, string[] columnUpdate, string[] valueUpdate, string whereColumn,
            string whereValue)
        {
            var table = Tables.FirstOrDefault(x => x.Item1.ToLower() == tableName.ToLower());
            var tempColumnIndexes = 0;
            var tempUpdateIndexes = new List<int>();

            foreach (var column in table.Item2.Columns)
            {
                //This for loop gets the index of the WHERE column
                if (column.Name.ToLower().Equals(whereColumn.ToLower()))
                {
                    tempColumnIndexes = table.Item2.Columns.IndexOf(column);
                }
                //This loop gets the index of th UPDATE columns
                foreach (var update in columnUpdate)
                {
                    if (column.Name.ToLower().Equals(update.ToLower()))
                    {
                        tempUpdateIndexes.Add(table.Item2.Columns.IndexOf(column));
                    }
                }
            }


            //Put the values into the cells of the specified column with conditions
            var whereFields =
                table.Item2.Rows.Where(
                    x => x.Cellsssss[tempColumnIndexes].ToString().ToLower() == whereValue.ToString().ToLower());
            foreach (var row in whereFields)
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
            var tempUpdateIndexes = new List<int>();

            foreach (var column in table.Item2.Columns)
            {
                //This loop gets the index of th UPDATE columns
                foreach (var update in columnUpdate)
                {
                    if (column.Name.ToLower().Equals(update.ToLower()))
                    {
                        tempUpdateIndexes.Add(table.Item2.Columns.IndexOf(column));
                    }
                }
            }

            foreach (var row in table.Item2.Rows)
            {
                //Put the values into the cells of the specified column
                foreach (var item in tempUpdateIndexes)
                {
                    row.Cellsssss[item] = valueUpdate[tempUpdateIndexes.IndexOf(item)];
                }
            }
        }

        public Tuple<string, Relation> select(string tableName, string column, string value)
        {
            var tempColumns = new List<ColumnProperties>();
            var tempColumnIndex = 0;
            var table = Tables.FirstOrDefault(x => x.Item1.ToLower() == tableName);

            //Grab the index of the columns for new relation
            foreach (var columnz in table.Item2.Columns)
            {
                tempColumns.Add(columnz);
                if (columnz.Name.ToUpper().Equals(column.ToUpper()))
                {
                    tempColumnIndex = table.Item2.Columns.IndexOf(columnz);
                }
            }
            var tempRelation = new Relation(tempColumns);

            //find the condition from the index and get the list of rows that will need to be selected
            var selectedRelation =
                table.Item2.Rows.Where(x => x.Cellsssss[tempColumnIndex].ToString().ToLower().Equals(value.ToLower()));
            foreach (var row in selectedRelation)
            {
                tempRelation.Rows.Add(row);
            }
            return new Tuple<string, Relation>(table.Item1, tempRelation);
        }


        public Tuple<string, Relation> select(string tableName)
        {
            //find the table
            var table = Tables.FirstOrDefault(x => x.Item1.ToLower() == tableName.ToLower());
            return table;
        }

        public void print(Relation rel)
        {
            //For nice formatting
            Console.WriteLine("\n____________________________________________________________________");

            //Go through all the columns and format it for the console
            for (var y = 0; y < rel.Columns.Count(); y++)
                Console.Write("{0, " + (2*y + 7) + "} | ", rel.Columns[y].Name);

            Console.WriteLine("\n____________________________________________________________________");

            foreach (var row in rel.Rows)
            {
                //do the same for all the cells in the row
                for (var j = 0; j < row.Cellsssss.Count(); j++)
                {
                    try
                    {
                        Console.Write("{0, " + (2*j + 7) + "} | ", row.Cellsssss[j]);
                    }
                    catch //This only happens when there is a null value in the table
                    {
                        Console.Write("{0, " + (2*j + 7) + "} |", "Null");
                    }
                }
                //Write a line for nicer formatting on the console
                Console.WriteLine();
            }
        }

        private bool checkType(string typeOfInput, string typeOfNeeded)
        {
            //Checks the type of the two strings. 
            //The "Type of input" should be the GetType().toString() of the object that is needs to be checked.
            //The "Type of needed" is the one that was specified in the columns when they were first made
            if (typeOfNeeded.ToLower().Equals("string"))
            {
                return false;
            }
            switch (typeOfInput)
            {
                case "System.Int32":
                    return ((!(typeOfNeeded.ToLower().Equals("int") |
                               typeOfNeeded.ToLower().Equals("integer"))));
                case "System.String":
                    return (!typeOfNeeded.ToLower().Equals("string"));
                case "System.Boolean":
                    return ((!(typeOfNeeded.ToLower().Equals("boolean") |
                               typeOfNeeded.ToLower().Equals("bool"))));
                default:
                    return false;
            }
        }

        private object tryCast(string input)
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

        private void LoadDatabase(string fileName)
        {
            var uri = AppDomain.CurrentDomain.BaseDirectory;
            using (var sr = new StreamReader(uri +fileName))
            {
                string line;
                string tableName = "";
                var columnNames = new List<string>();
                var columnAttributes = new List<string>();
                var columnLength = new List<int>();
                var nullable = new List<bool>();
                bool newTable = false;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line[0] == 'µ')
                    {
                        var split = line.Split(new[] {"µ"}, StringSplitOptions.RemoveEmptyEntries);
                        tableName = split[0];
                        columnNames = new List<string>();
                        columnAttributes = new List<string>();
                        columnLength = new List<int>();
                        nullable = new List<bool>();
                        newTable = true;
                    }
                    else if (line[0] == '‰')
                    {
                        var split = line.Split(new[] {"‰", "\t"}, StringSplitOptions.RemoveEmptyEntries);
                        columnNames.AddRange(split);
                    }
                    else if (line[0] == 'ß')
                    {
                        var split = line.Split(new[] { "ß", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        columnAttributes.AddRange(split);
                    }
                    else if (line[0] == 'Σ')
                    {
                        var split = line.Split(new[] { "Σ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        columnLength.AddRange(split.Select(length => Convert.ToInt32(length)));
                    }
                    else if (line[0] == 'Φ')
                    {
                        var split = line.Split(new[] { "Φ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        nullable.AddRange(split.Select(Convert.ToBoolean));
                    }
                    else if(line[0] == 'φ')
                    {
                        if (newTable)
                        {
                            createTable(tableName, columnNames.ToArray(), columnAttributes.ToArray(), columnLength.ToArray(), nullable.ToArray());
                            newTable = false;
                        }
                        var split = line.Split(new[] { "φ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        Object[] send = new object[split.Length];
                        for (int i = 0; i < split.Length; i++)
                        {
                            send[i] = tryCast(split[i]);
                        }
                        insert(tableName, send);
                    }
                }
            }
        }

        private async Task DbSave(string filePath)
        {
            var uri = AppDomain.CurrentDomain.BaseDirectory;
            using (var file = new StreamWriter( uri +filePath))
            {
                foreach (var table in Tables)
                {
                    file.WriteLine("µ" + table.Item1);
                    file.Write("‰");
                    foreach (var item in table.Item2.Columns)
                    {
                        file.Write(item.Name + "\t");
                    }
                    file.WriteLine("");
                    file.Write("ß");
                    foreach (var data in table.Item2.Columns)
                    {
                        file.Write(data.Type + "\t");
                    }
                    file.WriteLine("");
                    file.Write("Σ");
                    foreach (var data in table.Item2.Columns)
                    {
                        file.Write(data.Length + "\t");
                    }
                    file.WriteLine("");
                    file.Write("Φ");
                    foreach (var data in table.Item2.Columns)
                    {
                        file.Write(data.Nullable + "\t");
                    }
                    file.WriteLine("");
                    if (table.Item2.Rows.Count > 0)
                    {
                        foreach (var row in table.Item2.Rows)
                        {
                            for (var i = 0; i < table.Item2.Columns.Count; i++)
                            {
                                file.Write("φ"+row.Cellsssss[i] + "\t");
                            }
                            file.WriteLine("");
                        }
                    }
                    else
                    {
                        file.WriteLine("φ");
                    }
                }
            }
        }

        #region parse

        public List<Tuple<string, Relation>> parse(string statementFull)
        {
            //split at the ';' for each new command
            var splitStatements = statementFull.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
            var retList = new List<Tuple<string, Relation>>();

            //Parse through the commands and send it to a function to do the rest of the parsing
            foreach (var statement in splitStatements)
            {
                var command = statement.Substring(0, 11);
                if (command.ToLower().Contains("insert"))
                {
                    parseInsert(statement.Trim());
                }
                else if (command.ToLower().Contains("select"))
                {
                    var temp = parseSelect(statement.Trim());
                    if (temp != null)
                    {
                        retList.Add(temp);
                    }
                }
                else if (command.ToLower().Contains("create"))
                {
                    parseCreate(statement.Trim());
                }
                else if (command.ToLower().Contains("update"))
                {
                    parseUpdate(statement.Trim());
                }
                else if (command.ToLower().Contains("delete"))
                {
                    parseDelete(statement.Trim());
                }
                else if (command.ToLower().Contains("destroy"))
                {
                    parseDestroy(statement.Trim());
                }
                else if (command.ToLower().Contains("union"))
                {
                    var temp = parseUnion(statement.Trim());
                    if (temp != null)
                    {
                        retList.Add(temp);
                    }
                }
            }
            var task = DbSave(@"Database.txt");
            task.WaitAndUnwrapException();
            return retList;
        }

        private void parseDestroy(string statement)
        {
            var split = statement.ToLower().Split(new[] {" table "}, StringSplitOptions.None);
            if (split.Length > 1)
            {
                destroy(split[1].Trim());
            }
        }

        private void parseInsert(string statement)
        {
            statement = statement.Trim().Remove(0, 11);
            var split = Regex.Split(statement, " values", RegexOptions.IgnoreCase);

            var tableName = split[0].Trim();
            if (split.Length > 1)
            {
                var values = split[1].Replace('(', ' ').Replace(')', ' ').Split(',');
                var tempObjects = new List<object>();
                foreach (var item in values)
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        tempObjects.Add(null);
                    }
                    else
                    {
                        var addedItem = item.Trim();
                        tempObjects.Add(tryCast(addedItem));
                    }
                }
                insert(tableName, tempObjects.ToArray());
            }
        }

        private Tuple<string, Relation> parseUnion(string statement)
        {
            statement = statement.Trim().Remove(0, 5);
            var tableNames = Regex.Split(statement, "and", RegexOptions.IgnoreCase);

            if (tableNames.Length > 1)
            {
                var whereSplit = Regex.Split(tableNames[1], "where", RegexOptions.IgnoreCase);

                if (tableNames.Length > 1)
                {
                    var valueSplit = Regex.Split(whereSplit[1], "=");

                    if (valueSplit.Length > 1)
                    {
                        return new Tuple<string, Relation>(tableNames[0].Trim() + whereSplit[0].Trim(),
                            union(tableNames[0].Trim(), whereSplit[0].Trim(), valueSplit[0].Trim(), valueSplit[1].Trim()));
                    }
                }
            }
            return null;
        }

        private Tuple<string, Relation> parseSelect(string statement)

        {
            var split = statement.ToLower().Split(new[] {" from "}, StringSplitOptions.None);

            if (split.Length > 1)
            {
                var tableWhere = split[1].ToLower().Split(new[] {"where"}, StringSplitOptions.None);
                var tableName = tableWhere[0].Trim();
                if (tableWhere.Length > 1)
                {
                    var whereClause = tableWhere[1].ToLower().Split(new[] {"="}, StringSplitOptions.None);

                    if (whereClause.Length > 1)
                    {
                        return select(tableName, whereClause[0].Trim(), whereClause[1].Trim());
                    }
                }
                else
                {
                    return select(tableName);
                }
            }
            return null;
        }

        private void parsePrint(string statement)
        {
            var split = statement.ToLower().Split(new[] {" from "}, StringSplitOptions.None);
            if (split.Length > 1)
            {
                //print(split[1].Trim());
            }
        }

        private void parseCreate(string statement)
        {
            var split = Regex.Split(statement, " table ", RegexOptions.IgnoreCase);

            if (split.Length > 1)
            {
                var tableInfo = split[1].Split(new[] {"(", ")"}, StringSplitOptions.RemoveEmptyEntries);
                var tableName = tableInfo[0].Trim();

                if (tableInfo.Length > 1)
                {
                    var createColumns = Regex.Split(tableInfo[1], ",", RegexOptions.IgnoreCase);

                    //Lists for create method
                    var attributeNames = new List<string>();
                    var attributeTypes = new List<string>();
                    var attributeLength = new List<int>();
                    var nullable = new List<bool>();
                    foreach (var column in createColumns)
                    {
                        try
                        {
                            var columnSplit = column.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
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
            var split = statement.ToLower().Split(new[] {"from"}, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 1)
            {
                var whereSplit = split[1].ToLower().Split(new[] {"where"}, StringSplitOptions.RemoveEmptyEntries);
                if (whereSplit.Length > 1)
                {
                    var whereValue = whereSplit[1].Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);
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

                var updateColumns = new List<string>();
                var updateValues = new List<string>();

                foreach (var valueSplit in valuesColumns.Select(update => update.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries)))
                {
                    updateColumns.Add(valueSplit[0].Trim());
                    updateValues.Add(valueSplit[1].Trim());
                }

                if (setSplit.Length > 1)
                {
                    var whereSplit = setSplit[1].Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);
                    update(tableName, updateColumns.ToArray(), updateValues.ToArray(), whereSplit[0].Trim(),
                        whereSplit[1].Trim());
                }
                else
                {
                    update(tableName, updateColumns.ToArray(), updateValues.ToArray());
                }
            }
        }

        #endregion
        private string PathToAppDir(string localPath)
        {
            var currentDir = Environment.CurrentDirectory;
            var directory = new DirectoryInfo(
                Path.GetFullPath(Path.Combine(currentDir, @"..\..\" + localPath)));
            return directory.ToString();
        }
    }
}