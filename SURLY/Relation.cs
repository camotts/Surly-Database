using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURLY
{
    public class ColumnProperties
    {
        public ColumnProperties(string Name, string Type, int Length, bool Nullable)
        {
            //These are the 'Tuples' for the columns 
            //Origanally we had it use the class Tuple, but 
            //that got a bit hard to read and understand
            this.Name = Name;
            this.Type = Type;
            this.Length = Length;
            this.Nullable = Nullable;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public int Length { get; set; }
        public bool Nullable { get; set; }
    }
    public class Relation 
    {
        //The main table definition that everything will use
        public List<Row> Rows { get; set; }
        public List<ColumnProperties> Columns { get; private set; }

        public Relation(List<ColumnProperties> newColumns){
            Columns = newColumns;
            Rows = new List<Row>();
        }
    }
}
