
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using network_server.DataAccess;
using network_server.Services.s_user;
using network_server.Settings;
using System.Text;

namespace network_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            // Configure Swagger for API documentation
            builder.Services.AddEndpointsApiExplorer();

            /*** start coding ***/

            // Configure Swagger with JWT Bearer Authentication
            builder.Services.AddSwaggerGen(swg =>
            {
                swg.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Define the JWT Bearer Authentication scheme in Swagger
                swg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Bearer Authentication",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // Apply the JWT Bearer Authentication scheme globally
                swg.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            // Configure Entity Framework Core with SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        //The operator ! mean make sure the value not null
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            // Configure Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Manager", policy => policy.RequireRole("Manager"));
                options.AddPolicy("Employee", policy => policy.RequireRole("Employee"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));
            });

            // Register scoped and transient services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Configure Email Settings from environment variables
            builder.Services.Configure<EmailSettings>(options =>
            {
                options.FromEmail = Environment.GetEnvironmentVariable("E_FROMEMAIL") ?? string.Empty;
                options.SmtpServer = Environment.GetEnvironmentVariable("E_SMTPSERVER") ?? string.Empty;
                options.Port = Environment.GetEnvironmentVariable("E_PORT") ?? string.Empty;
                options.Username = Environment.GetEnvironmentVariable("E_SMTP_USERNAME") ?? string.Empty;
                options.Password = Environment.GetEnvironmentVariable("E_SMTP_PASSWORD") ?? string.Empty;
            });

            /*** end coding ***/

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            /* *** 
             * Serve static files (e.g., user-uploaded files) before authentication
             * This ensures that static files are accessible without authentication 
             * and authorization is enforced for other requests.
             * ***/
            app.UseStaticFiles();

            // Enable authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Map controllers to routes
            app.MapControllers();
            // Run the application
            app.Run();
        }
    }
}
