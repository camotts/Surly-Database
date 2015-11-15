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
            var user = JsonQuery("Select * from Teacher Where name = " + userName);
            if (user[0]["Name"].Equals(password))
            {
                return objToJson(user);
            }
            return null;
        }

        public string Classes(int TeachID)
        {
            return objToJson(JsonQuery("Select * from Classes WHERE TeacherID = " + TeachID));
        }

        private string objToJson(List<Dictionary<String, String>> obj)
        {
                if (obj.Count > 1)
                {
                    return JsonConvert.SerializeObject(obj);
                }
                else
                {
                    return JsonConvert.SerializeObject(obj[0]);
                }
            
        }

        private List<Dictionary<String, String>> JsonQuery(string query)
        {
            var db = WebDataBase.db.parse(query);
            if (db != null)
            {
                return ObjectFromDB(db);
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
            if (ret.Count > 0)
            {
                return ret;
            }
            return null;
        }
    }
}