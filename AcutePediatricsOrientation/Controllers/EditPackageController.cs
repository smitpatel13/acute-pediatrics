using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AcutePediatricsOrientation.Enums;
using AcutePediatricsOrientation.Models;
using AcutePediatricsOrientation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AcutePediatricsOrientation.Controllers
{
    [Authorize(Policy = "Educator")]
    public class EditPackageController : Controller
    {
        private readonly AcutePediatricsContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EditPackageController(AcutePediatricsContext context, IHostingEnvironment environment)
        {
            _context = context;
            _hostingEnvironment = environment;
        }

        public IActionResult Index()
        {
            var categories = _context.Category.Select(c => new CategoryViewModel {
                Id = c.Id,
                Name = c.Name,
                Topics = c.Topics.Select(t => new TopicViewModel {
                    Id = t.Id,
                    Name = t.Name,
                    Documents = t.Documents.Select(d => new DocumentsViewModel {
                        Id = d.Id,
                        Name = d.Name
                    })
                })
            });
            return View(new PackageViewModel { Categories = categories.ToList()});
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(Category category)
        {
            if (_context.Category.Any(c => c.Name == category.Name))
            {
                return View();
            }
            else
            {
                _context.Category.Add(new Category { Name = category.Name });
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Category.SingleOrDefault(t => t.Id == id);
            if (category == null)
            {
                // TODO 
                return View("Error");
            }
            else
            {
                return View(category);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(Category category)
        {
            if(_context.Category.Any(c => c.Id == category.Id))
            {
                var topicIds = _context.Topic.Where(t => t.CategoryId == category.Id).Select(t => t.Id);

                foreach(var document in _context.Document.Where(d => topicIds.Contains(d.TopicId)))
                {
                    if (document.DocumentTypeId == (int)ProjectEnum.DocumentType.PDF)
                    {
                        // Deleting the file if the document type is pdf
                        DeleteDocumentFile(document);
                    }
                }

                // Deleting the documents from this category
                foreach (var topicId in topicIds)
                {
                    _context.Document.RemoveRange(_context.Document.Where(d => d.TopicId == topicId));
                    _context.Signature.RemoveRange(_context.Signature.Where(s => s.TopicId == topicId));
                }

                // Deleting the topics from this category
                _context.Topic.RemoveRange(_context.Topic.Where(t => t.CategoryId == category.Id));

                _context.Category.RemoveRange(_context.Category.Where(t => t.Id == category.Id));

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {   // TODO HERE
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var category = _context.Category.SingleOrDefault(t => t.Id == id);
            if (category == null)
            {
                // TODO 
                return View("Error");
            }
            else
            {
                return View(category);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(Category category)
        {
            var oldCategory = _context.Category.SingleOrDefault(t => t.Id == category.Id);
            if (oldCategory == null)
            {
                // TODO 
                return View("Error");
            }
            else
            {
                oldCategory.Name = category.Name;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult CreateTopic(int id)
        {
            return View(new Topic { CategoryId = id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTopic(Topic topic)
        {
            if (ModelState.IsValid)
            {
                _context.Topic.Add(new Topic { CategoryId = topic.CategoryId, Name = topic.Name});
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult EditTopic(int id)
        {
            var topic = _context.Topic.SingleOrDefault(t => t.Id == id);
            if (topic == null)
            {
                // TODO 
                return View("Error");
            }
            else
            {
                return View(topic);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTopic(Topic topic)
        {
            var oldTopic = _context.Topic.SingleOrDefault(t => t.Id == topic.Id);
            if (oldTopic == null)
            {
                // TODO 
                return View("Error");
            }
            else
            {
                oldTopic.Name = topic.Name;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult DeleteTopic(int id)
        {
            var topic = _context.Topic.Include(t => t.Category).SingleOrDefault(t => t.Id == id);
            if (topic == null)
            {
                // TODO 
                return View("Error");
            }
            else
            {
                return View(topic);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTopic(Topic topic)
        {
            var topicToBeRemoved = _context.Topic.Include(t => t.Documents).SingleOrDefault(t => t.Id == topic.Id);
            if (topicToBeRemoved == null)
            {
                // TODO 
                return View("Error");
            }
            else
            {
                foreach (var document in topicToBeRemoved.Documents)
                {
                    if (document.DocumentTypeId == (int)ProjectEnum.DocumentType.PDF)
                    {
                        // Deleting the file if the document type is pdf
                        DeleteDocumentFile(document);
                    }
                }

                _context.Signature.RemoveRange(_context.Signature.Where(s => s.TopicId == topic.Id));
                
                // Deleting the documents from this topic
                _context.Document.RemoveRange(topicToBeRemoved.Documents);

                _context.Topic.Remove(topicToBeRemoved);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult CreateDocument(int id)
        {
            var createDocumentViewModel = new CreateDocumentViewModel {
                TopicId = id,
                DocumentTypes = _context.DocumentType.Select(dt => new SelectListItem {
                    Text = dt.Name,
                    Value = dt.Id.ToString()
                })
            };

            return View(createDocumentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDocument(CreateDocumentViewModel document)
        {
            if (document.DocumentType == (int)ProjectEnum.DocumentType.PDF)
            {
                ModelState.Remove("Url");
            }
            else if (document.DocumentType == (int)ProjectEnum.DocumentType.Video)
            {
                ModelState.Remove("File");
            }

            if (ModelState.IsValid)
            {

                if (document.DocumentType == (int)ProjectEnum.DocumentType.PDF)
                {
                    var fileName = document.File.FileName;
                    var trainingFilesFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "trainingfiles");
                    var filePath = Path.Combine(trainingFilesFolderPath, fileName);
                    if (!_context.Document.Any(d => d.Path == filePath))
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            document.File.CopyTo(fileStream);
                        }
                        _context.Document.Add(new Documents
                        {
                            DocumentTypeId = document.DocumentType,
                            Path = Path.Combine("/trainingfiles/", fileName),
                            Name = document.Name,
                            TopicId = document.TopicId
                        });

                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("File", "That file is already used in the package");
                    }
                }
                else if (document.DocumentType == (int)ProjectEnum.DocumentType.Video)
                {
                    // COnvert the youtube url into an embed url
                    try
                    {
                        ConvertToYoutubeEmbed(document.Url);
     
                        _context.Document.Add(new Documents
                        {
                            DocumentTypeId = document.DocumentType,
                            Path = document.Url,
                            Name = document.Name,
                            TopicId = document.TopicId
                        });

                        _context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch(ArgumentException ex)
                    {
                        ModelState.AddModelError("Url", "Invalid YouTube video URL");
                    }
                }
            }

            document.DocumentTypes = _context.DocumentType.Select(dt => new SelectListItem
            {
                Text = dt.Name,
                Value = dt.Id.ToString()
            });
            return View(document);
        }

        [HttpGet]
        public IActionResult DeleteDocument(int id)
        {
            var document = _context.Document.Include(d => d.DocumentType).SingleOrDefault(d => d.Id == id);
            if (document == null)
            {
                // TODO 
                return View("Error");
            }
            else
            {
                return View(document);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDocument(Documents document)
        {
            var documentToBeRemoved = _context.Document.SingleOrDefault(d => d.Id == document.Id);
            if (documentToBeRemoved == null)
            {
                // TODO 
                return View("Error");
            }
            else
            {
                if(documentToBeRemoved.DocumentTypeId == (int)ProjectEnum.DocumentType.PDF)
                {
                    // Deleting the file if the document type is pdf
                    DeleteDocumentFile(documentToBeRemoved);
                }

                _context.Document.Remove(documentToBeRemoved);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult EditDocument(int id)
        {
            var document = _context.Document.SingleOrDefault(t => t.Id == id);
            if (document == null)
            {
                // TODO 
                return View("Error");
            }
            else
            {
                var documentViewModel = new EditDocumentViewModel()
                {
                    DocumentType = document.DocumentTypeId,
                    Id = document.Id,
                    Name = document.Name,
                    DocumentTypes = _context.DocumentType.Select(dt => new SelectListItem
                    {
                        Text = dt.Name,
                        Value = dt.Id.ToString()
                    }),
                    TopicId = document.TopicId,
                    FilePath = document.DocumentTypeId == (int)ProjectEnum.DocumentType.PDF ? document.Path : null,
                    Url = document.DocumentTypeId == (int)ProjectEnum.DocumentType.Video ? document.Path : null
                };
                return View(documentViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDocument(EditDocumentViewModel documentViewModel)
        {
            if (documentViewModel.DocumentType == (int)ProjectEnum.DocumentType.PDF)
            {
                ModelState.Remove("Url");
            }
            else if (documentViewModel.DocumentType == (int)ProjectEnum.DocumentType.Video)
            {
                ModelState.Remove("File");
            }

            if (ModelState.IsValid)
            {
                var document = _context.Document.SingleOrDefault(d => d.Id == documentViewModel.Id);

                if(document == null)
                {
                    return View("Error");
                }

                // Update some values of the document
                document.Name = documentViewModel.Name;
                document.TopicId = documentViewModel.TopicId;

                if (documentViewModel.DocumentType == (int)ProjectEnum.DocumentType.PDF && documentViewModel.File != null)
                {
                    if (document.DocumentTypeId == (int)ProjectEnum.DocumentType.PDF)
                    {
                        // Deleting the old file of the document if it was a pdf file before
                        DeleteDocumentFile(document);
                    }
                    var fileName = documentViewModel.File.FileName;
                    var trainingFilesFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "trainingfiles");
                    var filePath = Path.Combine(trainingFilesFolderPath, fileName);
                    if (!_context.Document.Any(d => d.Path == filePath))
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            documentViewModel.File.CopyTo(fileStream);
                        }

                        // Update the document path as a pdf document type
                        document.Path = Path.Combine("/trainingfiles/", fileName);
                        document.DocumentTypeId = documentViewModel.DocumentType;

                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("File", "That file is already used in the package");
                    }
                }
                else if (documentViewModel.DocumentType == (int)ProjectEnum.DocumentType.Video)
                {
                    if (document.DocumentTypeId == (int)ProjectEnum.DocumentType.PDF)
                    {
                        // Deleting the old file of the document if it was a pdf file before
                        DeleteDocumentFile(document);
                    }

                    try
                    {
                        // Update the document path as a video document type
                        ConvertToYoutubeEmbed(documentViewModel.Url);
                        document.Path = documentViewModel.Url;
                        document.DocumentTypeId = documentViewModel.DocumentType;

                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch(ArgumentException ex)
                    {
                        ModelState.AddModelError("Url", "Invalid YouTube video URL");
                    }
                }
                else
                {
                    document.DocumentTypeId = documentViewModel.DocumentType;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            documentViewModel.DocumentTypes = _context.DocumentType.Select(dt => new SelectListItem
            {
                Text = dt.Name,
                Value = dt.Id.ToString()
            });
            return View(documentViewModel);
        }


        private void DeleteDocumentFile(Documents document)
        {
            var filePath = _hostingEnvironment.WebRootPath + document.Path;
            System.IO.File.Delete(filePath);
        }

        private string ConvertToYoutubeEmbed(string youtubeUrl)
        {
            var uri = new Uri(youtubeUrl);
            var youtubeUrlParameter = HttpUtility.ParseQueryString(uri.Query).Get("v");
            if (youtubeUrlParameter == null)
            {
                throw new ArgumentException("Invalid youtube url");
            }
            var youtubeEmbedUrl = "https://www.youtube.com/embed/" + youtubeUrlParameter;
            return youtubeEmbedUrl;
        }
    }
}