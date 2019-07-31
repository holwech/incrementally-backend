using System.Text;
using System.Threading.Tasks;
using incrementally.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;

namespace incrementally_backend
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IHostingEnvironment env)
    {
      Configuration = configuration;
      if (env.IsDevelopment())
      {
          IdentityModelEventSource.ShowPII = true; 
      }
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthentication(options =>
      {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      })
        .AddJwtBearer(jwtOptions =>
        {
          jwtOptions.Authority = $"https://login.microsoftonline.com/tfp/{Configuration["AzureAdB2C:Tenant"]}/{Configuration["AzureAdB2C:Policy"]}/v2.0/";
          jwtOptions.Audience = Configuration["AzureAdB2C:ClientId"];
          jwtOptions.Events = new JwtBearerEvents
          {
            // OnAuthenticationFailed = AuthenticationFailed
          };
        });
      services.AddCors(options =>
        {
            options.AddPolicy("AllowCors",
            builder =>
            {
                builder.WithOrigins(
                  "http://localhost:8080",
                  "https://incrementally.xyz"
                )
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            });
        });
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseCors("AllowCors");

      app.UseAuthentication();
      app.UseHttpsRedirection();
      app.UseMvc();
    }

    private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
    {
      string databaseName = configurationSection.GetSection("DatabaseName").Value;
      string containerName = configurationSection.GetSection("ContainerName").Value;
      string account = configurationSection.GetSection("Account").Value;
      string key = configurationSection.GetSection("Key").Value;
      CosmosClientBuilder clientBuilder = new CosmosClientBuilder(account, key);
      CosmosClient client = clientBuilder
        .WithConnectionModeDirect()
        .Build();
      CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);
      Database database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
      await database.CreateContainerIfNotExistsAsync(containerName, "/id");

      return cosmosDbService;
    }

    private Task AuthenticationFailed(AuthenticationFailedContext arg)
    {
      // For debugging purposes only!
      var s = $"AuthenticationFailed: {arg.Exception.Message}";
      arg.Response.ContentLength = s.Length;
      arg.Response.Body.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
      return Task.FromResult(0);
    }
  }
}
