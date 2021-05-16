namespace GreenOnions.Gallery.Models
{
    public class PictureCountModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public int Total => PixivCount + TwitterCount;
        public int PixivCount => Pixiv0Count + Pixiv1Count + Pixiv2Count + Pixiv3Count + Pixiv9Count;
        public int Pixiv0Count { get; set; } = 0;
        public int Pixiv1Count { get; set; } = 0;
        public int Pixiv2Count { get; set; } = 0;
        public int Pixiv3Count { get; set; } = 0;
        public int Pixiv9Count { get; set; } = 0;
        public int TwitterCount => Twitter0Count + Twitter1Count + Twitter2Count + Twitter3Count + Twitter9Count;
        public int Twitter0Count { get; set; } = 0;
        public int Twitter1Count { get; set; } = 0;
        public int Twitter2Count { get; set; } = 0;
        public int Twitter3Count { get; set; } = 0;
        public int Twitter9Count { get; set; } = 0;
    }
}
