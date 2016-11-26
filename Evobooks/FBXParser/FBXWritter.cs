using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBXStructure.Basic;
using FBXStructure.Specific;
using System.IO;

namespace FBXParser
{
    /// <summary>
    /// Pode receber uma estrutura e gerar arquivos no formato .FBX ASCII
    /// </summary>
    public class FBXWritter
    {
        /// <summary>
        /// Stream onde os dados do arquivo serão escritos.
        /// </summary>
        public StreamWriter stWritter;

        /// <summary>
        /// Escreve informações sobre um FBX num Stream.
        /// </summary>
        /// <param name="file">A estrutura que representa todo o arquivo a ser escrito.</param>
        /// <param name="stream">O Stream onde os dados devem ser escritos</param>
        public void WriteFBX(Node file, Stream stream)
        {
            stWritter = new StreamWriter(stream);

            //Nesse momento essas properties são os nodes maiores, como por exemplo Objects, Connections.
            foreach (var property in file.Properties)
            {
                var node = property.Value as Node;
                if (node.Active)
                {
                    WriteNode(node, stWritter);
                    stWritter.WriteLine("}");
                }
            }

        }
        /// <summary>
        /// Faz as análises de estrutura, monta as informações e as escreve utilizando um StreamWriter a partir de uma estrutura de nó.
        /// </summary>
        /// <param name="node">O nó a ser analisádo e escrito.</param>
        /// <param name="writer">O escritor utilizado para gravar as informações.</param>
        void WriteNode(Node node, StreamWriter writer)
        {
            foreach (string comment in node.Comments) //Todos os comentários vem primeiro e sempre são iniciados com ';'.
            {
                writer.WriteLine(";" + comment);
            }

            writer.WriteLine(node.Name + ": " + node.EventualProperties.GetStringContent() + " {"); //Todo nó começa com: "Nome: prop, prop2, prop3 {"

            foreach (var property in node.Properties)
            {
                foreach (var comment in property.Comments)
                    writer.WriteLine(";" + comment);

                if (property.Value.GetType() == typeof(Node)) //Se o valor da propriedade for outro nó, começamos o processo de escrita de nós novamente .
                {
                    var nodeInterno = property.Value as Node;
                    if (node.Active)
                    {
                        WriteNode(nodeInterno, writer);
                        writer.WriteLine("}");
                    }
                }
                else//Se o valor da propriedade não é um nó, sempre é uma lista de valores em texto separados pro virgula.
                    if (property.Active)
                    writer.WriteLine(property.Name + ": " + (property.Value as List<string>).GetStringContent());
            }
        }
    }
   
}
