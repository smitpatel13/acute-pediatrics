using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using AcutePediatricsOrientation.Enums;
using AcutePediatricsOrientation.Models;
using AcutePediatricsOrientation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcutePediatricsOrientation.Controllers
{
    [Authorize]
    public class PackageController : Controller
    {
        private readonly AcutePediatricsContext _context;

        public PackageController(AcutePediatricsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var currentUser = User.Claims.Single(c => c.Type == ClaimTypes.Name).Value;

            var categories = _context.Category.Select(c => new CategoryViewModel
            {
                Name = c.Name,
                Topics = c.Topics.Select(t => new TopicViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Signature = t.Signatures.Where(s => s.User.Email == currentUser)
                                            .Select(s => new SignatureViewModel() {
                                                Name = s.User.FirstName + " " + s.User.LastName,
                                                Date = s.Date.ToString("MMMM dd, yyyy  h:mm tt")
                                            }).SingleOrDefault(),
                    Documents = t.Documents.Select(d => new DocumentsViewModel
                    {
                        Id = d.Id,
                        Name = d.Name
                    })
                })
            });

            var staffAccount = _context.Account.Single(a => a.Email == currentUser);

            return View(new PackageViewModel { Categories = categories.ToList(), StaffName = staffAccount.FirstName + " " + staffAccount.LastName });
        }

        public IActionResult ViewDocument(int id)
        {
            var document = _context.Document.SingleOrDefault(d => d.Id == id);

            if(document != null)
            {
                var documentViewModel = new DocumentsViewModel {
                    DocumentTypeId = document.DocumentTypeId,
                    Name = document.Name,
                    Path = document.DocumentTypeId == (int)ProjectEnum.DocumentType.Video ? ConvertToYoutubeEmbed(document.Path) : document.Path
                };
                return View(documentViewModel);
            }
            else
            {
                return View("Error");
            }
        }

        public IActionResult SignTopic(int id)
        {
            var currentUser = User.Claims.Single(c => c.Type == ClaimTypes.Name).Value;
            var topic = _context.Topic.SingleOrDefault(d => d.Id == id);
            var user = _context.Account.SingleOrDefault(a => a.Email == currentUser);

            if (topic != null && user != null)
            {
                var newSignature = new Signature
                {
                    Date = DateTime.Now,
                    TopicId = topic.Id,
                    UserId = user.Id
                };
                _context.Signature.Add(newSignature);
                _context.SaveChanges();

                return Json(new { Success = true, Name = user.FirstName + " " + user.LastName, Date = newSignature.Date.ToString("MMMM dd, yyyy  h:mm tt") });
            }
            else
            {
                return Json(new { Success = false });
            }
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