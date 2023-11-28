namespace Assignment5.Models
{
    public class Song
    {
        public required int SongId { get; set; }

        public required string Title { get; set; }

        public required string Artist { get; set; }

        public required string Genre { get; set; }

        public required decimal Price { get; set; }

    }
}
