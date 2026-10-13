namespace TuneBox.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public string Genre { get; set; }
        public int ArtistId { get; set; }
    }
}