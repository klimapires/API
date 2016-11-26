using System;
using System.Collections.Generic;
using System.Data;
using API.Extends;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using API.Models;
using FBXParser;
using FBXStructure.Basic;
using FBXStructure.Specific;
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Text;
using System.Net.Http.Headers;
using DTO.Files;
using System.Windows;

namespace API.Controllers
{
    /// <summary>
    /// Classe responsável por operações de input e output de arquivos pelo usuário
    /// </summary>
    [RoutePrefix("File"), Authorize]
    public class FilesController : ApiController
    {
        #region Fields

        private DBContext db = new DBContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion

        #region Methods

        #region Actions

        #region Input
        /// <summary>
        /// POST - Cadastra novo arquivo no banco de dados. Headers:
        ///     Content: multipart/form-data .
        ///     Authorization: 'Bearer ' + User Token
        /// Busca os seguintes dados no form : 
        ///     categoryId - ID da categoria relacionada à imagem
        ///     tags - Tags da imagem, em formato de string CSV
        ///     filename - Nome do arquivo
        /// </summary>
        /// <returns></returns>
        [Route("Novo")]
        public async Task<IHttpActionResult> Novo()
        {

            //Verifica se há um usuário válido logado.
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
                return Unauthorized();

            // Checar se o request é multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            //Processo de leitura do arquivo//
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            string CategoryID = provider.FormData["categoryId"];
            string CSVTags = provider.FormData["tags"];
            string Filename = provider.FormData["filename"];

            //Análise e estruturação do arquivo
            TextParser parser = new TextParser();
            byte[] binaryData = File.ReadAllBytes(provider.FileData[0].LocalFileName);

            Node fbxFile;

            try
            {
                fbxFile = parser.ParseFromStream(new MemoryStream(binaryData));
            }
            catch (Exception ex)
            {
                return BadRequest("Arquivo FBX inválido. - " + ex.Message);
            }


            //Carregamento da lista de objetos existentes no arquivo
            var objects = new List<FBXObject>();
            foreach (Property prop in (fbxFile.Properties.First(d => d.Name == "Objects").Value as Node).Properties)
            {
                var obj = FBXObject.GetFBXObjectFromObjectNode(prop.Value as Node);
                objects.Add(obj);
            }

            //Pega todas as conexões existentes e seus objetos.
            FBXObject rootNode = new FBXObject() { UID = "0", Name = "Root" };
            var connections = FBXConnection.GetConnectionsFromConnectionNode(fbxFile.Properties.
                First(D => D.Name == "Connections").Value as Node, objects, rootNode).ToList();

            //Objeto final a ser armazenado contendo estrutura, objetos e conexões
            Final final = new Final(connections, objects, fbxFile);
            MemoryStream memory = new MemoryStream();
            (new BinaryFormatter()).Serialize(memory, final);


            //Dado a ser guardado no banco
            var file = new Files()
            {
                Id = Guid.NewGuid().ToString(),
                BinaryFile = binaryData,
                ApplicationUser = db.Users.First(d => d.Id == user.Id),
                Name = Filename,
                FinalInformation = memory.GetBuffer(),
                Category = db.Categories.First(d => d.Id == CategoryID),
                UpdateDateTime = DateTime.Now
            };


            db.Files.Add(file);
            db.SaveChanges();
            SaveTagsForFile(CSVTags, file);
            SaveContentsForFile(final, file);
            db.SaveChanges();
            return Ok(file.Id);
        }

        //        //Verifica se há um usuário válido logado.
        //        ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //            if (user == null)
        //                return Unauthorized();

        //            // Checar se o request é multipart/form-data.
        //            if (!Request.Content.IsMimeMultipartContent("form-data"))
        //                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

        //        //Processo de leitura do arquivo//
        //        string root = HttpContext.Current.Server.MapPath("~/App_Data");
        //        var provider = new MultipartFormDataStreamProvider(root);
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        string CategoryID = provider.FormData["categoryId"];
        //        string CSVTags = provider.FormData["tags"];
        //        string Filename = provider.FormData["filename"];

        //        //Análise e estruturação do arquivo
        //        TextParser parser = new TextParser();
        //        byte[] binaryData = File.ReadAllBytes(provider.FileData[0].LocalFileName);

        //        Node fbxFile;

        //            try
        //            {
        //                fbxFile = parser.ParseFromStream(new MemoryStream(binaryData));
        //            }
        //            catch (Exception ex)
        //            {
        //                return BadRequest("Arquivo FBX inválido. - " + ex.Message);
        //}


        ////Carregamento da lista de objetos existentes no arquivo
        //var objects = new List<FBXObject>();
        //            foreach (Property prop in (fbxFile.Properties.First(d => d.Name == "Objects").Value as Node).Properties)
        //            {
        //                var obj = FBXObject.GetFBXObjectFromObjectNode(prop.Value as Node);
        //objects.Add(obj);
        //            }

        //            //Pega todas as conexões existentes e seus objetos.
        //            FBXObject rootNode = new FBXObject() { UID = "0", Name = "Root" };
        //var connections = FBXConnection.GetConnectionsFromConnectionNode(fbxFile.Properties.
        //    First(D => D.Name == "Connections").Value as Node, objects, rootNode).ToList();

        ////Objeto final a ser armazenado contendo estrutura, objetos e conexões
        //Final final = new Final(connections, objects, fbxFile);
        //MemoryStream memory = new MemoryStream();
        //            (new BinaryFormatter()).Serialize(memory, final);


        ////Dado a ser guardado no banco
        //var file = new Files()
        //{
        //    Id = Guid.NewGuid().ToString(),
        //    BinaryFile = binaryData,
        //    ApplicationUser = db.Users.First(d => d.Id == user.Id),
        //    Name = Filename,
        //    FinalInformation = memory.GetBuffer(),
        //    Category = db.Categories.First(d => d.Id == CategoryID),
        //    UpdateDateTime = DateTime.Now
        //};


        //db.Files.Add(file);
        //            db.SaveChanges();
        //            SaveTagsForFile(CSVTags, file);
        //            SaveContentsForFile(final, file);
        //db.SaveChanges();
        //            return Ok(file.Id);

        [Route("Delete")]
        public async Task<IHttpActionResult> DeletarArquivo(string FileID)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
                return Unauthorized();
            if (string.IsNullOrWhiteSpace(FileID))
                return BadRequest();
            if (!db.Files.Any(d => d.Id == FileID))
                return BadRequest("Arquivo não existe");

            var file = db.Files.First(d => d.Id == FileID);

            if (!CheckIfUserHasAccess(user, file))
                return Unauthorized();

            foreach (var i in db.ImageContents.Where(d => d.File.Id == file.Id).ToList())
            {
                db.ImageContents.Remove(i);
            }

            db.Files.Remove(file);

            db.SaveChanges();
            return Ok();
        }
        #endregion

        #region Output

        #region General Search
        //[HttpGet]
        //[AllowAnonymous]
        //[Route("All")]
        //public async Task<IHttpActionResult> GetAllFiles()
        //{
        //    return Ok(db.Files.ToDTOFile());
        //}

        [HttpGet]
        [Route("Evobooks")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetEvobooksFiles()
        {
            return Ok(db.Files.Where(d => d.ApplicationUser.UserName == "Evobooks").ToDTOFile());
        }
        /// <summary>
        /// Pega Files (DTO) baseado em parâmetros de pesquisa. Para ignorar um parâmetro, passe-o vazio.
        /// </summary>
        /// <param name="CategoryNames">Lista de nomes de categorias a pesquisar</param>
        /// <param name="Tags"> Lista de tags a pesquisar</param>
        /// <param name="SearchText"> Texto a se procurar no nome dos arquivos</param>
        /// <returns></returns>
        [Route("Search")]
        public async Task<IHttpActionResult> GetFilesByParams(string[] CategoryNames, string[] Tags, string SearchText)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
                return BadRequest();


            List<DTO.Files.FileBase> results = new List<DTO.Files.FileBase>();
            if (!string.IsNullOrEmpty(SearchText))
            {
                results.AddRange(db.Files.
                    Where(d =>
                    d.Name.ToLower().Contains(SearchText.ToLower().Trim()) ||
                    SearchText.Trim().ToLower().Contains(d.Name.ToLower()) &&
                    CheckIfUserHasAccess(user, d)
                    ).ToDTOFile());
            }
            if (CategoryNames.Length > 0)
            {
                results.AddRange(db.Files.Where(d => CategoryNames.Contains(d.Category.Name) && CheckIfUserHasAccess(user, d)).ToDTOFile());
            }
            if (Tags.Length > 0)
            {
                results.AddRange(db.FileTag.Where(d => Tags.Contains(d.Tag.Name) && CheckIfUserHasAccess(user, d.File)).Select(d => d.File).ToDTOFile());
            }


            return Ok(results.Distinct().ToList());
        }

        /// <summary>
        /// Pega todos os arquivos guardados aos quais o usuário tem acesso.
        /// </summary>
        /// <returns></returns>
        [Route("UserFiles")]
        public async Task<IHttpActionResult> GetFilesByUser()
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(db.Files.Where(d =>
            d.ApplicationUser.Id == user.Id || //Onde o usuário é dono do arquivo
            db.LibrariesFiles.Where(e => (db.LibrariesUsers.Where(f => f.User_Id == user.Id).Select(f => f.Library_Id)). //Bibliotecas as quais o usuário tem acesso
            Contains(e.Library_Id)).Select(e => e.File_Id).Contains(d.Id))
                .ToDTOFile());
        }

        /// <summary>
        /// Retorna uma lista de arquivos relacionados a uma determinada biblioteca.
        /// </summary>
        /// <param name="LibraryID"></param>
        /// <returns></returns>
        [Route("LibraryFiles")]
        public async Task<IHttpActionResult> GetFilesByLibrary(string LibraryID)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return Unauthorized();
            }
            if (!db.Libraries.Any(d => d.ID == LibraryID))
            {
                return BadRequest("Library doesnt exist.");
            }
            if (db.Libraries.First(d => d.ID == LibraryID).Creator != user || !db.LibrariesUsers.Any(d => d.Library_Id == LibraryID && d.User == user))
            {
                return Unauthorized();
            }


            return Ok(db.LibrariesFiles.Where(D => D.Library_Id == LibraryID).Select(d => d.File).ToDTOFile());
        }

        /// <summary>
        /// Retorna uma lista de IDs de libraries relacionadas ao usuário.
        /// </summary>
        /// <returns></returns>
        [Route("UserLibraries")]
        public async Task<IHttpActionResult> GetLibrariesByUser()
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(db.LibrariesUsers.Where(d => d.User_Id == user.Id).Select(d => d.Library_Id).ToList());
        }

        #endregion

        #region Specific File

        /// <summary>
        /// Retorna os dados originais de determinado arquivo. (Sem edições de camadas)
        /// </summary>
        /// <param name="FileID">ID do arquivo a ser retornado.</param>
        /// <returns></returns>
        [Route("Original")]
        public async Task<HttpResponseMessage> DownloadOriginalFile(string FileID)
        {

            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            if (!db.Files.Any(d => d.Id == FileID))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var file = db.Files.First(d => d.Id == FileID);

            if (!CheckIfUserHasAccess(user, file))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new MemoryStream(file.BinaryFile);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }


        /// <summary>
        /// Retorna os dados de um arquivo seguindo espeficiações de edições.
        /// </summary>
        /// <param name="specification">Espeficação de arquivo, camadas de objetos e camadas de animações.</param>
        /// <returns></returns>
        [Route("Specific")]
        public async Task<HttpResponseMessage> DownloadFileSpeficated(SpecificFile specification)
        {
            var t = Request.Content.ReadAsStringAsync();
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            if (!db.Files.Any(d => d.Id == specification.FileID))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var file = db.Files.First(d => d.Id == specification.FileID);

            if (!CheckIfUserHasAccess(user, file))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }



            var final = (new BinaryFormatter().Deserialize(new MemoryStream(file.FinalInformation)) as Final);

            //Atravessa todas as conexões de um arquivo em que um objeto desativado esteja relacionado e desativa todos os seus nodes e propriedades filhos. Isso faz com que o node seja desconsiderado na hora da reescrita do arquivo a ser enviado como resposta.
            if (specification.DisabledObjectNames != null && specification.DisabledObjectNames.Count() > 0)
            {
                try
                {

                    foreach (var connection in final.Connections.Where(d => d != null && d.Child != null && specification.DisabledObjectNames.Contains(d.Child.Class)))
                    {
                        ToggleChildNodes(connection, false, final.Connections);
                    }

                }
                catch (Exception ex) { }
            }

            //Atravessa todas as conexões de animações desativadas e adiciona nelas uma propriedade que faz com que leitores desses arquivos desconsiderem essa animação. (Mute)
            if (specification.DisabledAnimmationNames != null && specification.DisabledAnimmationNames.Count() > 0)
            {
                foreach (var connection in final.Connections.Where(d => d != null && d.Child != null && specification.DisabledAnimmationNames.Contains(d.Child.Class)))
                {
                    var prop70 = connection.Child.RelatedNode.Properties.First(d => d.Name == "Properties70").Value as Node;
                    prop70.Properties.RemoveAll(d => (d.Value as List<string>).Contains("\"Mute\""));
                    prop70.Properties.Add(new Property("P", new List<string>() { "\"Mute\"", "\"bool\"", "\"\"", "\"\"", "1" }, new List<string>()));
                }
            }
            //Processo de reescrita do arquivo.
            FBXWritter writter = new FBXWritter();
            MemoryStream stream = new MemoryStream();
            writter.WriteFBX(final.FBXFile, stream);

            //Envio da mensagem.
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(new MemoryStream(stream.GetBuffer()));
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            writter.stWritter.Dispose();
            return result;
        }



        /// <summary>
        /// Retorna uma lista com dados de todos os objetos associados a uma imagem.
        /// </summary>
        /// <param name="FileID">ID do arquivo relacionado aos objetos desejados.</param>
        /// <returns></returns>
        [Route("Objects")]
        public async Task<IHttpActionResult> ObjectsFromFile(string FileID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return Unauthorized();
            }

            if (!db.Files.Any(d => d.Id == FileID))
            {
                return BadRequest("File doest exist");
            }
            if (!CheckIfUserHasAccess(user, db.Files.First(d => d.Id == FileID)))
            {
                return Unauthorized();
            }

            return Ok(db.ImageContents.Where(d => d.ContentType.Name == "Object" && d.File.Id == FileID).ToDTOContent());
        }

        /// <summary>
        /// Retorna uma lista com dados de todas as animações associadas a uma imagem.
        /// </summary>
        /// <param name="FileID">ID do arquivo relacionado as animações desejadas.</param>
        /// <returns></returns>
        [Route("Animmations")]
        public async Task<IHttpActionResult> AnimmationsFromFile(string FileID)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return Unauthorized();
            }

            if (!db.Files.Any(d => d.Id == FileID))
            {
                return BadRequest("File doest exist");
            }

            if (!CheckIfUserHasAccess(user, db.Files.First(d => d.Id == FileID)))
            {
                return Unauthorized();
            }
            return Ok(db.ImageContents.Where(d => d.ContentType.Name == "Animmation" && d.File.Id == FileID).ToDTOContent());
        }

        /// <summary>
        /// Pega um arquivo FBX previamente gerado relacionado a um determinado objeto de uma imagem.
        /// </summary>
        /// <param name="ObjectID">ID do objeto.</param>
        /// <returns></returns>
        [Route("ObjectImage")]
        public async Task<HttpResponseMessage> DetailsFromObject(string ObjectID)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            if (!db.ImageContents.Any(d => d.Id == ObjectID))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            var content = db.ImageContents.First(d => d.Id == ObjectID);

            if (!CheckIfUserHasAccess(user, content.File))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new MemoryStream(content.Image);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }



        #endregion



        #endregion



        #endregion

        #region Helpers

        /// <summary>
        /// Método recursivo para desativar/ativar todos os filhos decendentes de uma conexão
        /// </summary>
        /// <param name="connection">Conexão à qual desativar/ativar a sí e aos filhos</param>
        /// <param name="value">Boolean para ativação(true)/desativação(false)</param>
        /// <param name="connections">Lista com todas as conexões relacionadas a conexão alvo.</param>
        private void ToggleChildNodes(FBXConnection connection, bool value, List<FBXConnection> connections)
        {

            if (connection != null)
            {
                if (connection.RelatedProperty != null)
                    connection.RelatedProperty.Active = value;
                if (connection.Child.RelatedNode != null)
                    connection.Child.RelatedNode.Active = value;

                foreach (var i in connections.Where(d => d != null && d.Parent != null && d.Parent.UID == connection.Child.UID))
                {
                    ToggleChildNodes(i, value, connections);
                }
            }


        }

        /// <summary>
        /// Busca os conteúdos (objetos e layers de animação) de um arquivo e guarda-os no banco.
        /// </summary>
        /// <param name="final">Estrutura, Objetos e Conexões do arquivo inserido</param>
        /// <param name="file">Instância da Entidade já adicionada ao banco</param>
        private void SaveContentsForFile(Final final, Files file)
        {
            //Desativa conexão com todos os objetos
            foreach (var con in final.Connections.Where(d => d != null && d.Child != null && d.Parent != null && d.Parent.UID == "0"))
            {
                ToggleChildNodes(con, false, final.Connections);
            }

            //Ativa cada objeto, salva os dados necessários no banco, desativa o objeto
            foreach (var con in final.Connections.Where(d => d != null && d.Child != null && d.Parent != null && d.Parent.UID == "0"))
            {
                db.ImageContents.Add(new ImageContents()
                {
                    Id = Guid.NewGuid().ToString(),
                    ContentType = db.ContentType.First(d => d.Name == "Object"),
                    Name = con.Child.Class,
                    File = file,
                    Image = GetImageFromLayer(final, con, final.Connections)
                });

                ToggleChildNodes(con, false, final.Connections);
            }

            foreach (var con in final.Connections.Where(d => d != null && d.Child != null && d.Child.MainClass == "AnimationLayer" && d.Child.Class != "BaseLayer"))
            {
                var prop70 = con.Child.RelatedNode.Properties.First(d => d.Name == "Properties70").Value as Node;
                if (prop70.Properties.Any(d => (d.Value as List<string>).Contains("\"Mute\"") && (d.Value as List<string>).Contains("\"bool\"") && (d.Value as List<string>).Contains("1")))
                    continue;

                db.ImageContents.Add(new ImageContents()
                {
                    Id = Guid.NewGuid().ToString(),
                    ContentType = db.ContentType.First(d => d.Name == "Animmation"),
                    Name = con.Child.Class,
                    File = file,
                });
            }


        }


        /// <summary>
        /// Gera uma imagem relativa a um unico objeto de um arquivo FBX
        /// </summary>
        /// <param name="final">Estrutura, Objetos e Conexões do arquivo inserido</param>
        /// <param name="connectionRelated">A conexão na qual o objeto Child vai ser renderizado</param>
        /// <returns></returns>
        private byte[] GetImageFromLayer(Final final, FBXConnection connectionRelated, List<FBXConnection> connections)
        {
            ToggleChildNodes(connectionRelated, true, connections);
            FBXParser.FBXWritter writter = new FBXWritter();
            using (MemoryStream memory = new MemoryStream())
            {
                writter.WriteFBX(final.FBXFile, memory);
                return memory.GetBuffer();
            }
        }

        /// <summary>
        /// Gera e associa Tags a uma entidade FBX.
        /// </summary>
        /// <param name="CSVTags">Texto em CSV(Valores separados por vírgula) com todas as tags</param>
        /// <param name="file">Entidade FBX</param>
        private void SaveTagsForFile(string CSVTags, Files file)
        {
            foreach (var tagName in CSVTags.Split(',').Select(d => d.Trim().ToLower()))
            {
                //Pular tags repetidas no mesmo arquivo.
                if (db.FileTag.Any(d => d.Tag.Name == tagName && d.File_Id == file.Id))
                    continue;
                //Caso ainda não esteja cadastrada, é adicionada no banco.
                if (!db.Tags.Any(d => d.Name == tagName))
                {
                    db.Tags.Add(new Tags() { Id = Guid.NewGuid().ToString(), Name = tagName });
                    db.SaveChanges();
                }
                Tags tag = db.Tags.First(d => d.Name == tagName);

                db.FileTag.Add(new FileTag() { File = file, Tag = tag });
            }
        }

        /// <summary>
        /// Checa se determinado usuário deveria ter acesso a um arquivo. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckIfUserHasAccess(ApplicationUser user, Files file)
        {
            var libraries = db.LibrariesUsers.Where(d => d.User_Id == user.Id).Select(d => d.Library).ToList();
            libraries.AddRange(db.Libraries.Where(d => d.Creator.Id == user.Id && !libraries.Contains(d)));
            return file.ApplicationUser.Id == user.Id || db.LibrariesFiles.Any(d => d.File_Id == file.Id && libraries.Any(e => d.Library_Id == e.ID));
        }


        #endregion

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}