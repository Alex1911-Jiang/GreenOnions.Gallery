namespace GreenOnions.Gallery.Models
{
    public class PictureRequestModel
    {
        public string Apikey { get; set; } = "";
        public string Grading { get; set; } = "0,1";
        public string Tag { get; set; } = "";
        public string Illustrator { get; set; } = "";
        public int Count { get; set; } = 1;
        public string Proxy { get; set; } = "i.pixiv.re";
        public bool Size1200 { get; set; } = false;
        public string Origin { get; set; } = "";
    }
}
