using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Music
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Key Vault
			var keyVaultUri =
				new Uri("https://kv-brights-common-01.vault.azure.net/");

			var managedIdentityClientId =
				Environment.GetEnvironmentVariable("MANAGED_IDENTITY_CLIENT_ID");

			var credential = string.IsNullOrEmpty(managedIdentityClientId)
				? new DefaultAzureCredential()
				: new DefaultAzureCredential(
					new DefaultAzureCredentialOptions
					{
						ManagedIdentityClientId = managedIdentityClientId
					});

			var secretClient =
				new SecretClient(keyVaultUri, credential);

			// Make SecretClient available to controllers
			builder.Services.AddSingleton(secretClient);

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();

				app.UseSwaggerUI(options =>
				{
					options.InjectStylesheet("/swagger-ui/custom.css");
				});
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.UseStaticFiles();

			// Endpoint that crashes the container
			app.MapGet("/evil", () =>
			{
				Environment.Exit(1);
			});

			app.MapControllers();

			app.Run();
		}
	}
}