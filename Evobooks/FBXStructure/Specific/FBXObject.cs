using FBXStructure.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBXStructure.Specific
{
    //Representa um objeto do arquivo FBX, uma especificação de um dos membros do Node Objects.
    [Serializable]
    public class FBXObject
    {
        
        public string MainClass;
        public string UID;
        public string Name;
        public string Class;
        public string SubClass;

        /// <summary>
        /// O objeto Node ao qual esse objeto é associado.
        /// </summary>
        public Node RelatedNode;
        public FBXObject() { }

        public FBXObject(string MainClass, string UID, string Name, string Class, string SubClass, Node RelatedNode)
        {
            this.MainClass = MainClass;
            this.UID = UID;
            this.Name = Name;
            this.Class = Class;
            this.SubClass = SubClass;
            this.RelatedNode = RelatedNode;        }

        /// <summary>
        /// Retorna um FBXObject a partir de um tipo Node.
        /// </summary>
        /// <param name="node">Node membro do nó de objetos</param>
        /// <returns></returns>
        public static FBXObject GetFBXObjectFromObjectNode(Node node)
        {
            //Pega as propriedades a partir de padrões de escrita dos arquivos FBX.

            var nameAndClass = node.EventualProperties[1].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            string name = nameAndClass[0].Length > 0 ? nameAndClass[0].Split('\"')[1] : nameAndClass[0];
            string classe = nameAndClass[1].Length > 0 ? nameAndClass[1].Split('\"')[0] : nameAndClass[1];

            string subClass = node.EventualProperties[2].Remove(0, 2);

            subClass = subClass.Substring(0, subClass.Length - 1);

            return new FBXObject(node.Name, node.EventualProperties[0], name , classe, subClass, node);
        }
    }
}
