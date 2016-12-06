using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
                return GetFromConfig();
            }
        }

        public static string AzureOriginalImageContainerName
        {
            get
            {
                return GetFromConfig();
            }
        }

        public static string AzureThumbnailImageContainerName
        {
            get
            {
                return GetFromConfig();
            }
        }

        public static string AzureLargeImageContainerName
        {
            get
            {
                return GetFromConfig();
            }
        }

        public static string FacebookAppId
        {
            get
            {
                return GetFromConfig();
            }
        }

        public static int ThumbnailWidth
        {
            get
            {
                return GetFromConfig<int>();
            }
        }

        public static int ThumbnailHeight
        {
            get
            {
                return GetFromConfig<int>();
            }
        }

        public static int LargeImageWidth
        {
            get
            {
                return GetFromConfig<int>();
            }
        }

        public static int LargeImageHeight
        {
            get
            {
                return GetFromConfig<int>();
            }
        }

        static T GetFromConfig<T>([CallerMemberName] string name = null)
        {
            if (Configuration == null)
                throw new InvalidOperationException();

            return Configuration.GetValue<T>(name);
        }

        static string GetFromConfig([CallerMemberName] string name = null)
        {
            return GetFromConfig<string>(name);
        }

    }
}
