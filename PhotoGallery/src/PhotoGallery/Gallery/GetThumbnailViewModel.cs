using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PhotoGallery.Gallery
{
    public class GetThumbnailViewModel : GalleryViewModel
    {
        public async Task<Uri> GetThumbnailUriAsync(string path)
        {
            return await GetImageUriAsync(Config.AzureThumbnailImageContainerName, path, Config.ThumbnailWidth, Config.ThumbnailHeight);
        }


        public async Task<Uri> GetLargeImageUriAsync(string path)
        {
            return await GetImageUriAsync(Config.AzureLargeImageContainerName, path, Config.LargeImageWidth, Config.LargeImageHeight);
        }


        private async Task<Uri> GetImageUriAsync(string containerName, string path, int width, int height)
        {
            var connectionString = Config.AzureStorageConnectionString;

            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);

            var blobClient = account.CreateCloudBlobClient();

            var sourcecontainer = blobClient.GetContainerReference(Config.AzureOriginalImageContainerName);

            var container = blobClient.GetContainerReference(containerName);

            await sourcecontainer.CreateIfNotExistsAsync();
            await container.CreateIfNotExistsAsync();

            await sourcecontainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            var sizedImage = container.GetBlockBlobReference(path);

            if (await sizedImage.ExistsAsync())
            {
                return sizedImage.Uri;
            }
            else
            {
                var original = sourcecontainer.GetBlockBlobReference(path);

                if (await original.ExistsAsync())
                {
                    await ResizeAndCopyAsync(original, sizedImage, width, height);

                    return sizedImage.Uri;
                }
            }


            return null;
        }

    }
}
