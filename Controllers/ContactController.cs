using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;
using ShoppingOnline.Models;
using ShoppingOnline.ViewModels;

namespace ShoppingOnline.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public ContactController(IContactRepository contactRepository,
            IMapper mapper)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }
        [Route("lien-he")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PostContact(ContactViewModel contactVM)
        {
            var code = new { Success = false, code = -1 };
            if (ModelState.IsValid)
            {
                contactVM.CreatedDate = DateTime.Now;
                contactVM.ModifiedDate = DateTime.Now;
                var contact = _mapper.Map<Contact>(contactVM);
                _contactRepository.Update(contact);
                code = new { Success = true, code = 1 };
                return new JsonResult(code);
            }
            return new JsonResult(code);
        }
       
    }
}
