using FBXStructure.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBXStructure.Specific
{
    /// <summary>
    /// Representam informações finais sobre um arquivo FBX, contendo seus objetos, conexões e sua estrutura.
    /// </summary>
    [Serializable]
    public class Final
    {
        /// <summary>
        /// Conexões do arquivo. (Membros do nó Connections)
        /// </summary>
        public List<FBXConnection> Connections = new List<FBXConnection>();

        /// <summary>
        /// Objetos do arquivo. (Membros do nó Objects)
        /// </summary>
        public List<FBXObject> Objects = new List<FBXObject>();

        /// <summary>
        /// Estrutura do arquivo. 
        /// </summary>
        public Node FBXFile = new Node();

        public Final(List<FBXConnection> connections, List<FBXObject> objects, Node fbxFile)
        {
            this.Connections = connections;
            this.Objects = objects;
            this.FBXFile = fbxFile;
        }
    }
}
