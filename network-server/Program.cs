
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using network_server.DataAccess;
using network_server.Services.s_user;
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
            builder.Services.AddEndpointsApiExplorer();

            /*** start coding ***/

            //Swagger
            builder.Services.AddSwaggerGen(swg =>
            {
                swg.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                //JWT Bearer Authentication in Swagger
                swg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Bearer Authentication",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

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


            // Configure database context
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

            // Add scoped services
            builder.Services.AddScoped<IUserService, UserService>();

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
             * Use static files to serve user-uploaded files
             * Use a static file before using authentication and authorization 
             * to make sure the static file has a higher priority 
             * than using authen and author
             * ***/
            app.UseStaticFiles();

            // Use authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
