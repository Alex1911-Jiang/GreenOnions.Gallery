using GreenOnions.Gallery.Interface;
using GreenOnions.Gallery.Models;
using System;
using System.Collections.Generic;

namespace GreenOnions.Gallery.Service
{
    public class CacheHelper : ICacheHelper
    {
        private readonly Dictionary<string, DateTime> _LimitCacheTime = new();
        private readonly Dictionary<string, PictureResponseModel> _LimitCacheModel = new();
        public Dictionary<string, DateTime> LimitCacheTime => _LimitCacheTime;
        public Dictionary<string, PictureResponseModel> LimitCacheModel => _LimitCacheModel;
        public static string PublicImageUrlCache { get; set; } = null;
        public static int PublicImageWidthCache { get; set; } = 0;
        public static int PublicImageHeightCache { get; set; } = 0;
    }
}
