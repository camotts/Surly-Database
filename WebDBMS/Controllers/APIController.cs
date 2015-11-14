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
            var ret = WebDataBase.db.parse("Select from Teacher Where username = '" + userName + "' AND password = " + password);
            if (ret != null)
            {
                var whtat = ret.First(x => true).Item2.Rows;
                return JsonConvert.SerializeObject(whtat);
            }
            return null;
        }

        public string Classes()
        {
            var TeachID = 0;
            var ret = WebDataBase.db.parse("Select from Classes WHERE TeacherID = " + TeachID);
            //var dict = new Dictionary<Int64, String>();
            //dict.Add(1, "key");
            //dict.Add(2, "second");
            //var y = JsonConvert.SerializeObject(dict);
            //var z = JsonConvert.DeserializeObject(y);
            if (ret != null)
            {
                var jRet = new List<Dictionary<String, String>>();
                foreach (var row in ret[0].Item2.Rows)
                {
                    var item = new Dictionary<String, String>();
                    for (int i = 0; i < row.Cellsssss.Length; i++)
                    {
                        item = new Dictionary<String, String>();
                        var kjd = ret[0].Item2.Columns[i].ToString();
                        item.Add(ret[0].Item2.Columns[i].ToString(), row.Cellsssss[i].ToString());
                    }
                    jRet.Add(item);
                }
                var test = JsonConvert.SerializeObject(jRet);
                return test;
            }
            return null;
        }
    }
}