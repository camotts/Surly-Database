using SURLY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDBMS.Models
{
    public class Tables
    {
        public IEnumerable<Tuple<string, Relation>> myTables { get; set; }
    }
}
