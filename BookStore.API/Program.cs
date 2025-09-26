using BookStore.API.Repositories;
using BookStore.API.Utility.DBInitializer;
using Ecommerce517.api.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace BookStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add the DB service to connect 
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            // for Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/Identity/Account/Login";
            //    options.AccessDeniedPath = "/Customer/Home/NotFound";

            //});

            //for email sender
            builder.Services.AddTransient<IEmailSender, EmailSender>();



            //scoped servieces
            builder.Services.AddScoped<IRepository<Book>,Repository<Book>>();
            builder.Services.AddScoped<IRepository<Auther>,Repository<Auther>>();
            builder.Services.AddScoped<IRepository<Cart>, Repository<Cart>>();
            builder.Services.AddScoped<IRepository<Promotion>, Repository<Promotion>>();
            builder.Services.AddScoped<IRepository<UserOTP>, Repository<UserOTP>>();
            builder.Services.AddScoped<IDBInitializer, DBIntializer>();

            //integration for => stripe
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];




            builder.Services.AddControllers();

            //for => swagger
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // service for => dbInitailizer 
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDBInitializer>();
            service?.Initialize();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                //app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseStaticFiles();
            app.MapControllers();

            app.Run();
        }
    }
}
