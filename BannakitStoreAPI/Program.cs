using BannakitStoreApi.Data;
using BannakitStoreApi.Reponsitory.IReponsitory;
using BannakitStoreApi.Reponsitory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BannakitStoreApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173") // URL of React app
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // if cookies or authentication
        });
});

// Add services to the container.

// Add database connection string
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();

builder.Services.AddResponseCaching();

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IManagementRepository, ManagementRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; // http -> false, https -> true
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
    };
});

builder.Services.AddControllers(option =>
{
    option.CacheProfiles.Add("Default30",
       new CacheProfile()
       {
           Duration = 30
       });
    //option.ReturnHttpNotAcceptable=true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Bannakit Store API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme // เพิ่มการกำหนดความปลอดภัย ให้กับ Swagger โดยใช้ JWT Authentication
    {
        Description = "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization", // Name: กำหนดให้เป็น "Authorization" ซึ่งจะเป็นชื่อของ Header ที่ใช้ส่ง JWT token มาใน API.
        In = ParameterLocation.Header, // In: กำหนดให้ข้อมูล JWT ถูกส่งผ่าน Header ของ HTTP request.
        Type = SecuritySchemeType.ApiKey, // Type: กำหนดเป็น ApiKey ซึ่งหมายความว่า JWT จะถูกใช้เป็น "API Key" ในคำขอ.
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    // เพิ่มข้อกำหนดความปลอดภัยให้กับเอกสาร Swagger API เพื่อให้ Swagger UI แสดงกลไกการยืนยันตัวตนด้วย JWT.
    // ใช้ OpenApiSecurityRequirement เพื่อกำหนดว่า JWT ต้องเป็นส่วนหนึ่งของการเรียกใช้งาน API ทั้งหมด (ขึ้นอยู่กับการตั้งค่า).
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference // Reference: อ้างอิงถึงการตั้งค่าการรักษาความปลอดภัยที่กำหนดไว้ใน AddSecurityDefinition
                {
                    Type = ReferenceType.SecurityScheme, // Type: เป็น SecurityScheme ซึ่งหมายถึงการใช้ JWT authentication.
                    Id = JwtBearerDefaults.AuthenticationScheme // Id: อ้างอิงถึง JwtBearerDefaults.AuthenticationScheme เพื่อบอกว่าเรากำลังใช้ JWT.
                },
                Scheme = "Oauth2", // Scheme: กำหนดเป็น "Oauth2", ซึ่งเป็นโครงสร้างพื้นฐานที่ทำงานกับ JWT.
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header // In: กำหนดให้ JWT ถูกส่งมาผ่าน Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bannakit_StoreV1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Bannakit_StoreV2");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    // PhysicalFileProvider ใช้ในการกำหนดตำแหน่งของไฟล์ที่เก็บอยู่ในเครื่องเซิร์ฟเวอร์ (Physical Files)
    // Path.Combine(Directory.GetCurrentDirectory(), "Images"): จะกำหนดให้โฟลเดอร์ Images ใน directory ปัจจุบัน (ที่โปรเจกต์ ASP.NET Core ทำงานอยู่) เป็นที่เก็บไฟล์ static
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
    // https://localhost:44306/Images
});

app.MapControllers();

app.Run();
