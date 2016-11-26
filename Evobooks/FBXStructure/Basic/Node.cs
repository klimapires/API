using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBXStructure.Basic
{
    // Representa qualquer Node num arquivo. (Exemplo: O node de objetos, ou de conexões).
    [Serializable]
    public class Node 
    {
        
        public bool Active = true;
        public string Name;
        public List<string> EventualProperties;
        public List<string> Comments = new List<string>();
        public List<Property> Properties = new List<Property>();
      
        public Node(string name, List<string> eventualProperties)
        {
            this.Name = name;
            this.EventualProperties = eventualProperties;
        }

        public Node()
        {

        }

        
    }
}
