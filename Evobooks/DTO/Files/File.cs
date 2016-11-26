using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Files
{
    /// <summary>
    /// Representa a especificação de um formato de arquivo (Qual arquivo, quais os objetos e animações a serem desativados)
    /// </summary>
    public class SpecificFile
    {
        public string FileID { get; set; }

        public string[] DisabledAnimmationNames { get; set; }
        public string[] DisabledObjectNames { get; set; }
    }

    /// <summary>
    /// Informações básicas sobre um arquivo no banco.
    /// </summary>
    public class FileBase
    {
        public string ID { get; set; }

        public string Name { get; set; }

    }

    /// <summary>
    /// Detalhes sobre um arquivo no banco.
    /// </summary>
    public class FileDetails : FileBase
    {
     
        public string CategoryName { get; set; }


        public string CreatorUserName { get; set; }

        public DateTime UploadDate { get; set; }

    }
}
