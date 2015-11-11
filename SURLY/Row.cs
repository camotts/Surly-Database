using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURLY
{
    public class Row
    {
        public Object[] Cellsssss { get; set; }

        public Row(int size) {
            //We chose an array since the rows should be a fixed amount 
            //(however many columns there are)
            Cellsssss = new Object[size];
        }
    }
}
