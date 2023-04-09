using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Business.AuthenticationService;
using Microsoft.OpenApi.Models;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Data.Repositories.Interface;
using Data.Repositories.implement;
using Business.UserService.Implements;
using Microsoft.Extensions.DependencyInjection;
using Business.UserService.Interfaces;

using Business.ExamSchedule.interfaces;
using Business.ExamSchedule.Implements;

using Business.ExamPaperService.Interfaces;
using Business.ExamPaperService.Implements;
using AutoMapper;
using Business;
using Business.TypeService.@interface;
using Business.TypeService.implement;
using Business.AvailableSubjectService.Implement;
using Business.AvailableSubjectService.Interface;
using System.Text.Json.Serialization;
using Business.NotificationService.Interfaces;
using Business.NotificationService.implement;
using Business.RegisterSubjectService.Interfaces;
using Business.RegisterSubjectService.implement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(
             options =>
             {
                 options.AddDefaultPolicy(
                      builder => builder
                                 .AllowAnyMethod()
                                 .AllowAnyOrigin()
                                 .AllowAnyHeader()

                      );

             }
             );
builder.Services.AddDbContext<CFManagementContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CFManagement")));
var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfile());
});
//Tao mapper
IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IExamPaperRepository, ExamPaperRepository>();
builder.Services.AddScoped<IExamScheduleRepository, ExamScheduleRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IRegisterSubjectRepository, RegisterSubjectRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IAvailableSubjectRepository, AvailableSucjectRepository>();
builder.Services.AddScoped<ITypeRepository, TypeRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IRegisterSubjectRepository, RegisterSubjectRepository>();

builder.Services.AddScoped<IExamScheduleService>(x => new ExamScheduleService(x.GetRequiredService<IExamScheduleRepository>(), x.GetRequiredService<IRegisterSubjectRepository>(),
    x.GetRequiredService<IAvailableSubjectRepository>(), x.GetRequiredService<IMapper>(), x.GetRequiredService<CFManagementContext>(), x.GetRequiredService<INotificationRepository>()));
builder.Services.AddScoped<ITypeService>(x => new TypeService(x.GetRequiredService<ITypeRepository>()));
builder.Services.AddScoped<INotificationService>(x => new NotificationService(x.GetRequiredService<INotificationRepository>(), x.GetRequiredService<IMapper>()));
builder.Services.AddScoped<IRegisterSubjectService>(x => new RegisterSubjectService(x.GetRequiredService<IRegisterSubjectRepository>(), x.GetRequiredService<IMapper>(), x.GetRequiredService<CFManagementContext>()));
//Add UserService
builder.Services.AddScoped<IUserService>(x => new UserService(x.GetRequiredService<IUserRepository>(), x.GetRequiredService<CFManagementContext>(), x.GetRequiredService<IMapper>()));
builder.Services.AddScoped<IExamPaperService>(x => new ExamPaperService(x.GetRequiredService<CFManagementContext>(),x.GetRequiredService<IExamPaperRepository>()
    , x.GetRequiredService<ICommentRepository>(), x.GetRequiredService<IMapper>(), x.GetRequiredService<IExamScheduleRepository>()));

builder.Services.AddScoped<IAvailableSubjectService>(x => new AvailableSubjectService(
        x.GetRequiredService<IAvailableSubjectRepository>(),
        x.GetRequiredService<IRegisterSubjectRepository>(),
        x.GetRequiredService<IMapper>()
    ));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Teacher Management", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                  {
                    new OpenApiSecurityScheme
                    {
                      Reference = new OpenApiReference
                        {
                          Type = ReferenceType.SecurityScheme,
                          Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                  }
                });
});
var Configuration = builder.Configuration;


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)


                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                        ValidAudience = builder.Configuration["AppSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSettings:JwtSecret"]))

                    };
                });
builder.Services.AddScoped<LoginService>(x => new LoginService(Configuration, x.GetRequiredService<CFManagementContext>()));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger CFT v1");
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger CFT v1");
    });
}

app.UseHttpsRedirection();

app.UseCors();


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
