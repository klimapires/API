using FBXStructure.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBXStructure.Specific
{
    /// <summary>
    /// Representa uma conexão entre dois elementos de um arquivo (Por exemplo dois objetos, ou propriedades, ou um objeto com uma propriedade).
    /// É um objeto que trata do Node de conexões dos arquivos FBX ASCII.
    /// </summary>
    [Serializable]
    public class FBXConnection
    {
        /// <summary>
        ///  Indica o tipo de conexão (OO = entre objetos, OP = objeto-propriedade, etc).
        /// </summary>
        public string ConnectionString;

        /// <summary>
        /// Aponta para o FBXObject que representa "o que é conectado à", quando a conexão é do tipo OO.
        /// </summary>
        public FBXObject Child;

        /// <summary>
        /// Aponta para o FBXObject que representa "no que é conectado", quando a conexão é do tipo OO.
        /// </summary>
        public FBXObject Parent;

        /// <summary>
        /// Aponta para a Propriedade que representa essa conexão. (Sempre membro do Node Connections)
        /// </summary>
        public Property RelatedProperty;

        public FBXConnection() { }
        public FBXConnection(string connection, FBXObject child, FBXObject parent, Property RelatedProperty)
        {
            this.ConnectionString = connection;
            this.Child = child;
            this.Parent = parent;
            this.RelatedProperty = RelatedProperty;
        }
        /// <summary>
        /// Cria uma conexão apartir de uma propriedade.
        /// </summary>
        /// <param name="prop">A propriedade que representa a conexão.</param>
        /// <param name="objects">Uma lista com todos os objetos conhecidos do arquivo (membros do nó de objetos)</param>
        /// <param name="rootNode">Um objeto que represente o próprio arquivo como nó. Não precisa ser relacionado a lista de objetos</param>
        /// <returns></returns>
        public static FBXConnection GetConnectionFromConnectionProperty(Property prop, List<FBXObject> objects, FBXObject rootNode)
        {
            if (prop.Name != "C")
            {
                return null;
            }
            try
            {
                var childUID = (prop.Value as List<string>)[1];

                var parentUID = (prop.Value as List<string>)[2];

                FBXObject child = objects.Any(d => d.UID == childUID) ? objects.First(d => d.UID == childUID) : null;
                FBXObject parent = rootNode;

                if (parentUID != "0")
                {
                    parent = objects.Any(d => d.UID == parentUID) ? objects.First(d => d.UID == parentUID) : null;
                }
                return new FBXConnection((prop.Value as List<string>)[0], child, parent, prop);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Cria e retorna todas as conexões relacionadas à um determinado objeto.
        /// </summary>
        /// <param name="connectionsNode">O tipo Node que corresponda, no FBX, ao nó de conexões (Connections)</param>
        /// <param name="objects">Uma lista com todos os membros do node Objects (representando os objetos do FBX ASCII)</param>
        /// <param name="rootNode">Um Objeto que represente o arquivo como um todo, sempre deve ter UID = 0. Não precisa ser relacionado a lista de objetos</param>
        /// <returns></returns>
        public static IEnumerable<FBXConnection> GetConnectionsFromConnectionNode(Node connectionsNode, List<FBXObject> objects, FBXObject rootNode)
        {
            if (connectionsNode.Name != "Connections") throw new ArgumentException();

            List<FBXConnection> connections = new List<FBXConnection>();
            foreach (var connection in connectionsNode.Properties)
            {
                connections.Add(FBXConnection.GetConnectionFromConnectionProperty(connection, objects, rootNode));
            }
            return connections;
        }
    }
}
