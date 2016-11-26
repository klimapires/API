using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Files
{
    /// <summary>
    /// Representa o conteúdo de um arquivo, como um Object ou uma Animmation
    /// </summary>
    public class Content
    {
        public Content(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public string Name { get; set; }

        public string ID { get; set; }

    }
}
