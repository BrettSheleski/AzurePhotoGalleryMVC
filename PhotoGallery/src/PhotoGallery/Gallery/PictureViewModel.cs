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
    public class PictureViewModel : GalleryViewModel
    {
        public bool Exists { get; set; }
        public ItemInfo Picture { get; private set; }
        public Uri OriginalPictureUrl { get; private set; }

        public override async Task PrepareForViewAsync()
        {
            await base.PrepareForViewAsync();

            var connectionString = Config.AzureStorageConnectionString;

            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);

            var blobClient = account.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(Config.AzureOriginalImageContainerName);
            
            await container.CreateIfNotExistsAsync();

            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            var originalPicture = container.GetBlockBlobReference(Path);

            this.Exists = await originalPicture.ExistsAsync();

            if (this.Exists)
            {
                this.OriginalPictureUrl = originalPicture.Uri;
                
                this.Picture = new ItemInfo
                {
                    Path = originalPicture.Name,
                    Name = originalPicture.Name.Split('/').Last()
                };
            }
        }
    }
}
