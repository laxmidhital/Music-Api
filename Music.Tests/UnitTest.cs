using Xunit;
using Music.Controllers;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace Music.Tests;
public class UnitTest
{
    [Fact]
    public void Get_ReturnsAlbums()
    {
        // Arrange
        var secretClient = new SecretClient(
            new Uri("https://test.vault.azure.net/"),
            new DefaultAzureCredential());

        var controller = new MusicController(secretClient);

        // Act
        var result = controller.Get();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Album.Albums.Count, result.Count);
    }
}
