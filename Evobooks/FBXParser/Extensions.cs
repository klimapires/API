using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBXParser
{
    public static class Extensions
    {
        /// <summary>
        /// Gera uma string em CSV apartir de uma lista de valores
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetStringContent(this List<string> list)
        {
            string ret = "";
            foreach (var i in list)
            {
                ret += i + ",";
            }
            if (ret.Length > 0)
                return ret.Substring(0, ret.Length - 1);
            else
                return "";

        }
    }
}
