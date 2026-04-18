using EmployeePairs.Api.Services;
using EmployeePairs.Api.Services.Implementations;
using EmployeePairs.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmployeePairService, EmployeePairService>();

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAngularApp",
//         policy =>
//         {
//             policy
//                 .AllowAnyOrigin()
//                 .AllowAnyHeader()
//                 .AllowAnyMethod();
//         });
// });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// app.UseCors("AllowAll");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();