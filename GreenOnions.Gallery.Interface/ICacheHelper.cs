using GreenOnions.Gallery.Models;
using System;
using System.Collections.Generic;

namespace GreenOnions.Gallery.Interface
{
    public interface ICacheHelper
    {
        public Dictionary<string, DateTime> LimitCacheTime { get; }
        public Dictionary<string, PictureResponseModel> LimitCacheModel { get; }
    }
}
