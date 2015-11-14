using System;
using System.Linq;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using WebDBMS.Models;
using System.Collections.Generic;

namespace WebDBMS.Controllers
{
    public class APIController : Base
    {
        public string Login(string userName, string password)
        {
            return JsonQuery("Select * from Teacher Where username = " + userName + " AND password = " + password);
        }

        public string Classes(int TeachID)
        {
            return JsonQuery("Select * from Classes WHERE TeacherID = " + TeachID);
        }

        private string JsonQuery(string query)
        {
            var db = WebDataBase.db.parse(query);
            if (db != null)
            {
                var ret = ObjectFromDB(db);
                if (ret.Count > 1)
                {
                    return JsonConvert.SerializeObject(ret);
                }
                else
                {
                    return JsonConvert.SerializeObject(ret[0]);
                }
            }
            return null;
        }

        private List<Dictionary<String, String>> ObjectFromDB(List<Tuple<string, SURLY.Relation>> input)
        {
            var ret = new List<Dictionary<String, String>>();
            foreach (var row in input[0].Item2.Rows)
            {
                var item = new Dictionary<String, String>();
                for (int i = 0; i < row.Cellsssss.Length; i++)
                {
                    item.Add(input[0].Item2.Columns[i].Name.ToString(), row.Cellsssss[i].ToString());
                }
                ret.Add(item);
            }
            return ret;
        }
    }
}