using Microsoft.AspNetCore.Mvc;
using TuneBox.Models;
using TuneBox.Services;

namespace TuneBox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly DataService _dataService;

        public SongsController(DataService dataService)
        {
            _dataService = dataService;
        }

        // GET
        [HttpGet]
        public ActionResult<IEnumerable<Song>> GetAllSongs()
        {
            var data = _dataService.LoadData();
            return Ok(data.Songs);
        }

        // GET by ID
        [HttpGet("{id}")]
        public ActionResult<Song> GetSongById(int id)
        {
            var data = _dataService.LoadData();
            var song = data.Songs.FirstOrDefault(s => s.Id == id);

            if (song == null) return NotFound($"Song with ID {id} not found.");
            return Ok(song);
        }

        // POST
        [HttpPost]
        public ActionResult<Song> CreateSong([FromBody] Song newSong)
        {
            var data = _dataService.LoadData();

            if (!data.Artists.Any(a => a.Id == newSong.ArtistId))
            {
                return BadRequest(
                    $"Artist with ID {newSong.ArtistId} does not exist."
                );
            }

            int newId = data.Songs.Any() ? data.Songs.Max(s => s.Id) + 1 : 1;
            newSong.Id = newId;

            data.Songs.Add(newSong);
            _dataService.SaveData(data);

            return CreatedAtAction(nameof(GetSongById), new { id = newSong.Id }, newSong);
        }

        // PUT
        [HttpPut("{id}")]
        public IActionResult UpdateSong(int id, [FromBody] Song updatedSong)
        {
            var data = _dataService.LoadData();
            var existingSong = data.Songs.FirstOrDefault(s => s.Id == id);

            if (existingSong == null) return NotFound($"Song with ID {id} not found.");

            if (!data.Artists.Any(a => a.Id == updatedSong.ArtistId))
            {
                return BadRequest(
                    $"Artist with ID {updatedSong.ArtistId} does not exist."
                );
            }

            existingSong.Title = updatedSong.Title;
            existingSong.Duration = updatedSong.Duration;
            existingSong.Genre = updatedSong.Genre;
            existingSong.ArtistId = updatedSong.ArtistId;

            _dataService.SaveData(data);
            return NoContent();
        }

        // DELETE by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteSong(int id)
        {
            var data = _dataService.LoadData();
            var songToRemove = data.Songs.FirstOrDefault(s => s.Id == id);

            if (songToRemove == null) return NotFound($"Song with ID {id} not found.");

            data.Songs.Remove(songToRemove);
            _dataService.SaveData(data);
            return NoContent();
        }

        // DELETE
        [HttpDelete]
        public IActionResult DeleteAllSongs()
        {
            var data = _dataService.LoadData();
            data.Songs.Clear();
            _dataService.SaveData(data);
            return NoContent();
        }
    }
}
