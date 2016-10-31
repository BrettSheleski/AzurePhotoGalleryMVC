using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PhotoGallery
{
    public static class Config
    {
        public static IConfiguration Configuration { get; set; }

        public static string AzureStorageConnectionString
        {
            get
            {
                return Configuration?.GetValue<string>(nameof(AzureStorageConnectionString));
            }
        }

        public static string AzureOriginalImageContainerName
        {
            get
            {
                return Configuration?.GetValue<string>(nameof(AzureOriginalImageContainerName));
            }
        }

        public static string AzureThumbnailImageContainerName
        {
            get
            {
                return Configuration?.GetValue<string>(nameof(AzureThumbnailImageContainerName));
            }
        }

        public static string AzureLargeImageContainerName
        {
            get
            {
                return Configuration?.GetValue<string>(nameof(AzureLargeImageContainerName));
            }
        }

        public static string FacebookAppId
        {
            get
            {
                return Configuration?.GetValue<string>(nameof(FacebookAppId));
            }
        }

    }
}
