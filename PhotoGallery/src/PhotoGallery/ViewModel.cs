using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery
{
    public abstract class ViewModel
    {
        public virtual Task PrepareForViewAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
