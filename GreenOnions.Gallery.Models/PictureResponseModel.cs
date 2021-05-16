using System.Collections.Generic;

namespace GreenOnions.Gallery.Models
{
    public class PictureResponseModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Origin { get; set; }
        public List<PictureResponseDataModel> Data { get; set; }
        public int? Count => Data?.Count;
    }

}
