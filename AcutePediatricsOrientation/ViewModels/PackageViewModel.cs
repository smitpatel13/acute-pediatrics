using AcutePediatricsOrientation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcutePediatricsOrientation.ViewModels
{
    public class PackageViewModel
    {
        public List<CategoryViewModel> Categories { get; set; }
        public string StaffName { get; set; }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<TopicViewModel> Topics { get; set; }
    }

    public class TopicViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public IEnumerable<DocumentsViewModel> Documents { get; set; }
        public SignatureViewModel Signature { get; set; }
    }

    public class DocumentsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DocumentTypeId { get; set; }
        public string Path { get; set; }
    }

    public class SignatureViewModel
    {
        public string Name { get; set; }
        public string Date { get; set; }
    }


    public class CreateDocumentViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int DocumentType { get; set; }
        public IEnumerable<SelectListItem> DocumentTypes { get; set; }
        [Required]
        public int TopicId { get; set; }
        [Url]
        [Required]
        public string Url { get; set; }
        [Required]
        public IFormFile File { get; set; }
        public string FilePath { get; set; }
    }

    public class EditDocumentViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int DocumentType { get; set; }
        public IEnumerable<SelectListItem> DocumentTypes { get; set; }
        [Required]
        public int TopicId { get; set; }
        [Url]
        [Required]
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string FilePath { get; set; }
    }
}
