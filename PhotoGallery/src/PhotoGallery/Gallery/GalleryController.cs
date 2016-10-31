using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;



// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PhotoGallery.Gallery
{
    public class GalleryController : Controller
    {
        [NonAction]
        public Task<IActionResult> Index()
        {
            return Index(null);
        }

        public async Task<IActionResult> Index(string path)
        {
            var vm = new IndexViewModel();

            vm.Path = path;

            await vm.PrepareForViewAsync();

            return View(vm);
        }

        public async Task<IActionResult> Picture(string path)
        {
            var vm = new PictureViewModel();

            vm.Path = path;

            await vm.PrepareForViewAsync();

            return View(vm);
        }
    }
}
