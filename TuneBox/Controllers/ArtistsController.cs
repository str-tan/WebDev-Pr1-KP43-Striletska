using Microsoft.AspNetCore.Mvc;
using TuneBox.Models;
using TuneBox.Services;

namespace TuneBox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistsController : ControllerBase
    {
        private readonly DataService _dataService;

        public ArtistsController(DataService dataService)
        {
            _dataService = dataService;
        }

        // GET
        [HttpGet]
        public ActionResult<IEnumerable<Artist>> GetAllArtists()
        {
            var data = _dataService.LoadData();
            return Ok(data.Artists);
        }

        // GET by ID
        [HttpGet("{id}")]
        public ActionResult<Artist> GetArtistById(int id)
        {
            var data = _dataService.LoadData();
            var artist = data.Artists.FirstOrDefault(a => a.Id == id);
            if (artist == null)
            {
                return NotFound($"Artist with ID {id} not found.");
            }
            return Ok(artist);
        }

        // POST
        [HttpPost]
        public ActionResult<Artist> CreateArtist([FromBody] Artist newArtist)
        {
            var data = _dataService.LoadData();

            int newId = data.Artists.Any() ? data.Artists.Max(a => a.Id) + 1 : 1;
            newArtist.Id = newId;

            data.Artists.Add(newArtist);
            _dataService.SaveData(data);
            
           
            return CreatedAtAction(nameof(GetArtistById), new { id = newArtist.Id }, newArtist);
        }

        // PUT
        [HttpPut("{id}")]
        public IActionResult UpdateArtist(int id, [FromBody] Artist updatedArtist)
        {
            var data = _dataService.LoadData();
            var existingArtist = data.Artists.FirstOrDefault(a => a.Id == id);

            if (existingArtist == null)
            {
                return NotFound($"Artist with ID {id} not found.");
            }

            existingArtist.Name = updatedArtist.Name;
            existingArtist.Country = updatedArtist.Country;

            _dataService.SaveData(data);

            return NoContent();
        }

        // DELETE by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteArtist(int id)
        {
            var data = _dataService.LoadData();
            var artistToRemove = data.Artists.FirstOrDefault(a => a.Id == id);

            if (artistToRemove == null)
            {
                return NotFound($"Artist with ID {id} not found.");
            }

            data.Artists.Remove(artistToRemove);

            //Cascade delete
            data.Songs.RemoveAll(s => s.ArtistId == id);

            _dataService.SaveData(data);

            return NoContent();
        }

        // DELETE all
        [HttpDelete]
        public IActionResult DeleteAllArtists()
        {
            var data = _dataService.LoadData();

            data.Artists.Clear();
            data.Songs.Clear();

            _dataService.SaveData(data);

            return NoContent();
        }

        // GET songs by artist ID, one-to-many
        [HttpGet("{id}/songs")]
        public ActionResult<IEnumerable<Song>> GetSongsByArtist(int id)
        {
            var data = _dataService.LoadData();

            if (!data.Artists.Any(a => a.Id == id))
            {
                return NotFound($"Artist with ID {id} not found.");
            }

            var artistSongs = data.Songs.Where(s => s.ArtistId == id).ToList();
            return Ok(artistSongs);
        }
    }
}
