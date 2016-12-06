using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PhotoGallery.Gallery
{
    public abstract class GalleryViewModel : ViewModel
    {
        public string Path { get; set; }
        public string Name
        {
            get
            {
                if (Path == null)
                    return null;

                return Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
            }
        }

        protected async Task ResizeAndCopyAsync(CloudBlockBlob sourceBlob, CloudBlockBlob targetBlob, int width, int height)
        {
            using (Stream sourceStream = await sourceBlob.OpenReadAsync())
            {
                Image sourceImage = Image.FromStream(sourceStream);

                SizeF originalSize = new SizeF(sourceImage.Width, sourceImage.Height);
                SizeF targetSize = new SizeF(width, height);
                if (originalSize.Width > originalSize.Height)
                {
                    // set the height based on the original aspect ratio

                    targetSize.Height = width * (originalSize.Height / originalSize.Width);
                }
                else
                {
                    // set the width based on the original aspect ratio
                    targetSize.Width = height * (originalSize.Width / originalSize.Height);
                }


                var targetRect = new Rectangle(Point.Empty, targetSize.ToSize());
                Bitmap targetImage = new Bitmap(targetRect.Width, targetRect.Height);

                using (var graphics = Graphics.FromImage(targetImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(sourceImage, targetRect, 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                using (Stream targetStream = await targetBlob.OpenWriteAsync())
                {
                    targetImage.Save(targetStream, sourceImage.RawFormat);
                }

                targetBlob.Properties.ContentDisposition = sourceBlob.Properties.ContentDisposition;
                targetBlob.Properties.ContentType = sourceBlob.Properties.ContentType;

                await targetBlob.SetPropertiesAsync();
            }
        }

        public class ItemInfo
        {
            public string Path { get; set; }
            public string Name { get; set; }
        }
    }
}
