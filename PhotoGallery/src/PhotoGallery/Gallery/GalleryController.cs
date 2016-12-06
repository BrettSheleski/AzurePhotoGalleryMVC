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
        const int TEN_DAYS_SECONDS = 60 * 60 * 24 * 10;

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

        [ResponseCache(Duration = TEN_DAYS_SECONDS, VaryByHeader = "path")]
        public async Task<IActionResult> GetThumbnail(string path)
        {
            var vm = new GetThumbnailViewModel();

            var uri = await vm.GetThumbnailUriAsync(path);

            if (uri != null)
            {
                return Redirect(uri.AbsoluteUri);
            }
            else
            {
                return NotFound();
            }
        }

        

        [ResponseCache(Duration = TEN_DAYS_SECONDS, VaryByHeader = "path")]
        public async Task<IActionResult> GetLargePicture(string path)
        {
            var vm = new GetThumbnailViewModel();

            var uri = await vm.GetLargeImageUriAsync(path);

            if (uri != null)
            {
                return Redirect(uri.AbsoluteUri);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
