using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBXStructure.Basic
{
    //Representa uma propriedade qualquer de um Node.
    [Serializable]
    public class Property 
    {
        public string Name;
        public object Value; 
        public bool Active = true;
        public List<string> Comments = new List<string>();

        public Property(string name, object value, List<string> comments)
        {
            this.Name = name;
            this.Value = value;
            Comments = comments;
        }

        public override string ToString()
        {
            return Name;
        }

        
    }
}
