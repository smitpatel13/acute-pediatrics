using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AcutePediatricsOrientation.Models;
using AcutePediatricsOrientation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcutePediatricsOrientation.Controllers
{
    [Authorize(Policy = "Educator")]
    public class StaffController : Controller
    {
        private readonly AcutePediatricsContext _context;

        public StaffController(AcutePediatricsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var staffListViewModel = new StaffListViewModel();
            var stafSignaturefList = _context.Signature.ToList();
            var totalTopics = (double)_context.Topic.Count();
            staffListViewModel.Users = _context.Account.Select(a => 
                new StaffViewModel
                {
                    UserId = a.Id,
                    Name = a.FirstName + " " + a.LastName,
                    Progress = (((double)stafSignaturefList.Where(sl => sl.UserId == a.Id).Count() / totalTopics) * 100.0)
                }
            ).ToList();

            return View(staffListViewModel);
        }

        public IActionResult StaffPackage(int id)
        {
            var categories = _context.Category.Select(c => new CategoryViewModel
            {
                Name = c.Name,
                Topics = c.Topics.Select(t => new TopicViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Signature = t.Signatures.Where(s => s.User.Id == id)
                                            .Select(s => new SignatureViewModel()
                                            {
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

            var staffAccount = _context.Account.Single(a => a.Id == id);

            return View(new PackageViewModel { Categories = categories.ToList(), StaffName = staffAccount.FirstName + " " + staffAccount.LastName });
        }
    }
}