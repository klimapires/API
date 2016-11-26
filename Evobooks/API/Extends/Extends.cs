using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DTO.Files;

namespace API.Extends
{
    public static class Extends
    {

        /// <summary>
        /// Converte uma lista de Files(model) em uma lista de FileDetails(DTO)
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static List<FileDetails> ToDTOFile(this IQueryable<API.Models.Files> files)
        {

            return files.ToList().Select(file => new FileDetails()
            {
                ID = file.Id,
                CreatorUserName = file.ApplicationUser.UserName,
                Name = file.Name,
                UploadDate = file.UpdateDateTime,
                CategoryName = file.Category.Name,
            }).ToList();
        }

        /// <summary>
        /// Converte uma lista de ImageContents (model) em uma lista de Contents(DTO)
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static List<DTO.Files.Content> ToDTOContent(this IQueryable<API.Models.ImageContents> contents)
        {

            return contents.ToList().Select(content => new DTO.Files.Content(content.Id, content.Name)).ToList();
        }
    }
}