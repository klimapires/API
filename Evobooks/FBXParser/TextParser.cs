using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBXStructure.Basic;
using FBXStructure.Specific;
using System.IO;
using System.Text.RegularExpressions;

namespace FBXParser
{

    /// <summary>
    /// Pode gerar estruturas de arquivos a partir de arquivos.
    /// </summary>
    public class TextParser
    {



        #region Methods

        /// <summary>
        /// Gera estrutura do arquivo a partir de um caminho de um arquivo FBX ASC II.
        /// </summary>
        /// <param name="path">Caminho para o arquivo.</param>
        /// <returns></returns>
        public Node ParseFromFile(string path)
        {
            //Verificação de validade d oarquivo.
            FileInfo info = new FileInfo(path);
            if (info.Extension.ToUpper() != ".FBX")
            {
                throw new ArgumentException("The provided path wasn't references to a valid FBX file.");
            }
            return ParseFromStream(File.Open(path, FileMode.Open));
        }


        /// <summary>
        /// Cria uma estrutura de arquivo a partir de um Stream que contenha um arquivo FBX ASCII.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Node ParseFromStream(Stream stream)
        {
            try
            {
                //Lista com linhas do arquivo.
                List<string> arquivo = new List<string>();
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        arquivo.Add(reader.ReadLine());
                    }
                }
                //file representa a estrutura final. Seria como o RootNode / Root Object
                Node file = new Node();
                int i = 0;
                ProcessNode(arquivo, file, ref i);
                return file;
            }
            catch (Exception)
            {
                throw new ArgumentException("O arquivo espeficicado não tem o formato FBX ASC II");
            }
        }

        /// <summary>
        /// Analisa e adiciona informações a uma estrutura de nós.
        /// </summary>
        /// <param name="arquivo">Linhas do arquivo.</param>
        /// <param name="node">Estrutura onde os dados devem ser adicionados.</param>
        /// <param name="actualIndex"> Linha apartir de qual deve ser lida.</param>
        private void ProcessNode(List<string> arquivo, Node node, ref int actualIndex)
        {
            List<string> comments = new List<string>();
            for (; actualIndex < arquivo.Count; actualIndex++)
            {
                string line = arquivo[actualIndex].Trim();


                LineType currentType = GetLineType(line);

                switch (currentType)
                {
                    case LineType.Comment: comments.Add(line.Remove(0, 1).Trim()); continue;
                    case LineType.Property: ParseProperty(arquivo, ref actualIndex, node, comments); comments.Clear(); continue;
                    case LineType.NodeEnd: return;
                    case LineType.NodeBegin: ParseNode(arquivo, ref actualIndex, node, comments); comments.Clear(); continue;
                    case LineType.DisableProperty: ParseProperty(arquivo, ref actualIndex, node, comments, false); comments.Clear(); continue;
                }
            }
        }

        /// <summary>
        /// Analisa o e armazena conteúdo de um nó apartir de strings.
        /// </summary>
        /// <param name="arquivo">Linhas do arquivo a ser analisado.</param>
        /// <param name="actualIndex"> Linha atual a ser lida</param>
        /// <param name="node">Nó onde guardar informações</param>
        /// <param name="comments">Lista de comentários relacionados ao nó</param>
        private void ParseNode(List<string> arquivo, ref int actualIndex, Node node, List<string> comments)
        {
            string nodeLine = arquivo[actualIndex].Trim();
            string name = nodeLine.Split(':')[0].Trim();

            string propsLine = nodeLine.Remove(0, name.Length + 1);
            propsLine = propsLine.Substring(0, propsLine.Length - 1).Trim();

            List<string> eventualProps = new List<string>();
            eventualProps.AddRange(propsLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            Node child = new Node(name, eventualProps);
            child.Comments = comments.ToList();
            node.Properties.Add(new Property(child.Name, child, child.Comments.ToList()));

            actualIndex++;
            ProcessNode(arquivo, child, ref actualIndex);
        }


        /// <summary>
        /// Analisa e guarda valores de uma propriedade a partir strings.
        /// </summary>
        /// <param name="arquivo">Linhas do arquivo a ser analisado.</param>
        /// <param name="actualIndex">Indice da linha em que a leitura deve começar.</param>
        /// <param name="node">Nó "dono" da propriedade.</param>
        /// <param name="previousComments">"Comentário que aparecem antes da propriedade"</param>
        /// <param name="active">"Espeficicação se a propriedade está ativa ou não</param>
        private void ParseProperty(List<string> arquivo, ref int actualIndex, Node node, List<string> previousComments, bool active = true)
        {
            string propLine = arquivo[actualIndex].Trim();
            //Verificar as proximas linhas pra pegar todos os valores daquela prop
            while (actualIndex + 1 < arquivo.Count && GetLineType(arquivo[actualIndex + 1]) == LineType.Value) 
            {
                propLine += arquivo[actualIndex + 1].Trim();
                actualIndex++;
            }
            string name = propLine.Split(':')[0].Trim();
            List<string> values = new List<string>();

            //Pega todos os valores depois do nome da prop, separados por virgula
            values.AddRange(propLine.Remove(0, name.Length + 1).Trim().Split(','));

            Property prop = new Property(name, values, previousComments.ToList());
            prop.Active = active;
            node.Properties.Add(prop);
        }



        /// <summary>
        /// PEga o tipo de linha baseado no conteúdo.
        /// </summary>
        /// <param name="line"> Linha a ser analisada.</param>
        /// <returns></returns>
        private LineType GetLineType(string line)
        {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                return LineType.Empty;
            }
            if (line.StartsWith(";"))
            {
                return LineType.Comment;
            }
            if (line.EndsWith("}"))
            {
                return LineType.NodeEnd;
            }

            if (Regex.IsMatch(line, "^.*:.*{$")) //Node (QualquerCoisa:QualquerCoisa {)
            {
                return LineType.NodeBegin;
            }
            if (Regex.IsMatch(line, @"^.*:.*$"))   // Property (QualquerCoisa:QualquerCoisa)
            {
                return LineType.Property;
            }
            else //Para o caso de propriedades com valores que vão além de uma unica linha.
            {
                return LineType.Value;
            }

        }

        #endregion
    }

    /// <summary>
    /// Represenha um tipo de linha.
    /// </summary>
    public enum LineType
    {
        NodeBegin, //inicio de nodes
        Property,  //propriedades comuns
        NodeEnd,   //fim de nodes
        Comment,   //comentários
        Empty,     // linha vazia
        Value,      // continuação de valores de algum comentário ou coisa do tipo
        DisableProperty
    }
}
