using Microsoft.AspNetCore.Mvc;
using Azure.Security.KeyVault.Secrets;

namespace Music.Controllers
{
	[ApiController]
	[Route("api/albums")]
	[Produces("application/json")]
	public class MusicController : ControllerBase
	{
		private readonly SecretClient _secretClient;

		public MusicController(SecretClient secretClient)
		{
			_secretClient = secretClient;
		}


		[HttpGet]
		public List<Album> Get()
		{
			return Album.Albums;
		}


		/// <summary>
		/// Creates an Album.
		/// </summary>
		/// <param name="album"></param>
		/// <returns>A newly created Album</returns>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public Album Post(Album album)
		{
			Album.Albums.Add(album);

			return album;
		}


		[HttpPut]
		public IActionResult Put([FromBody] Album album)
		{
			var existingAlbum =
				Album.Albums.FirstOrDefault(a => a.Id == album.Id);

			if (existingAlbum == null)
			{
				return NotFound();
			}

			existingAlbum.Name = album.Name;
			existingAlbum.Artist = album.Artist;
			existingAlbum.ReleaseDate = album.ReleaseDate;
			existingAlbum.Genre = album.Genre;
			existingAlbum.Price = album.Price;

			return Ok(existingAlbum);
		}


		/// <summary>
		/// Deletes a specific Album.
		/// </summary>
		/// <param name="id"></param>
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var existingAlbum =
				Album.Albums.FirstOrDefault(a => a.Id == id);

			if (existingAlbum == null)
			{
				return NotFound();
			}

			Album.Albums.Remove(existingAlbum);

			return NoContent();
		}


		// Fetch secret directly from Azure Key Vault
		[HttpGet("secret-check")]
		public async Task<IActionResult> GetSecret()
		{
			try
			{
				var secret =
					await _secretClient.GetSecretAsync("ld-music-api");

				return Ok(
					$"Secret loaded successfully from Key Vault. Secret: {secret.Value.Value}");
			}
			catch (Exception ex)
			{
				return Problem(ex.Message);
			}
		}
	}
}