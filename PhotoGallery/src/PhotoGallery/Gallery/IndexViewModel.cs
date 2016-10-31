using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PhotoGallery.Gallery
{
    public class IndexViewModel : GalleryViewModel
    {

        public override async Task PrepareForViewAsync()
        {
            var connectionString = Config.AzureStorageConnectionString;

            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);

            var blobClient = account.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(Config.AzureOriginalImageContainerName);

            var thumbnailContainer = blobClient.GetContainerReference(Config.AzureThumbnailImageContainerName);

            await container.CreateIfNotExistsAsync();
            await thumbnailContainer.CreateIfNotExistsAsync();

            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            await thumbnailContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            var items = container.ListBlobs(prefix: Path);

            List<Task> copyTasks = new List<Task>();
            CloudBlockBlob thumb;

            Directories.AddRange(items.OfType<CloudBlobDirectory>().Select(dir => new DirectoryInfo
            {
                Name = dir.Prefix.Split('/').Reverse().Skip(1).Take(1).FirstOrDefault(),
                Path = dir.Prefix,
                Directory = dir
            }).OrderBy(x => x.Name));


            foreach (var dir in Directories)
            {
                var blob = dir.Directory.ListBlobs(useFlatBlobListing: true).OfType<CloudBlockBlob>().OrderBy(x => Guid.NewGuid()).Take(1).FirstOrDefault();

                thumb = thumbnailContainer.GetBlockBlobReference(blob.Name);

                if (!await thumb.ExistsAsync())
                {
                    copyTasks.Add(ResizeAndCopyAsync(blob, thumb, 150, 150));
                }

                dir.Thumbnail = new ItemInfo
                {
                    Name = System.IO.Path.GetFileNameWithoutExtension(thumb.Name.Split('/').Last()),
                    Url = thumb.Uri,
                    Path = thumb.Name
                };
            }

            var pictures = items.OfType<CloudBlockBlob>().ToList();

            List<CloudBlockBlob> thumbnailBlobs = new List<CloudBlockBlob>();

            foreach (var pic in pictures)
            {
                thumb = thumbnailContainer.GetBlockBlobReference(pic.Name);

                thumbnailBlobs.Add(thumb);

                if (!await thumb.ExistsAsync())
                {
                    copyTasks.Add(ResizeAndCopyAsync(pic, thumb, 150, 150));
                }
            }

            Task.WaitAll(copyTasks.ToArray());

            Items.AddRange(thumbnailBlobs.Select(blob => new ItemInfo { Name = System.IO.Path.GetFileNameWithoutExtension(blob.Name.Split('/').Last()), Url = blob.Uri, Path = blob.Name }).OrderBy(x => x.Name));

            await base.PrepareForViewAsync();
        }

        public List<DirectoryInfo> Directories { get; } = new List<DirectoryInfo>();
        public List<ItemInfo> Items { get; } = new List<ItemInfo>();

        public class DirectoryInfo
        {
            public string Name { get; set; }
            public string Path { get; set; }

            public CloudBlobDirectory Directory { get; internal set; }
            public ItemInfo Thumbnail { get; internal set; }

        }


    }
}
