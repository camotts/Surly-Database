using System;
using System.Linq;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using WebDBMS.Models;

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
        }

        public void Register(string name, string userName, string password)
        {
            
        }

        public string 
    }
}