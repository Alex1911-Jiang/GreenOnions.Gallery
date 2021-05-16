namespace GreenOnions.Gallery.Models
{
    public class SavePixivPictureModel
    {
        public long Pid { get; set; }
        public int P { get; set; }
        public int PageCount { get; set; }
        public string Title { get; set; }
        public long Uid { get; set; }
        public string Illustrator { get; set; }
        public string Alias { get; set; }
        public string Url { get; set; }
        public int Grading { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Tags { get; set; }
    }
}
