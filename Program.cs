
using API_PEDIDOS.funciones;
using API_PEDIDOS.Jobs;
using API_PEDIDOS.Middlewares;
using API_PEDIDOS.ModelsBD2Prueba;
using API_PEDIDOS.ModelsDB2;
using API_PEDIDOS.ModelsDBP;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using static Quartz.Logging.OperationName;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var DBPConnection = builder.Configuration.GetConnectionString("DBPConnection");
var DB2Connection = builder.Configuration.GetConnectionString("DB2Connection");
var DB2PConnection = builder.Configuration.GetConnectionString("DB2PConnection");

builder.Services.AddDbContext<DBPContext>(options => options.UseSqlServer(DBPConnection))
    .AddDbContext<BD2Context>(options => options.UseSqlServer(DB2Connection))
    .AddDbContext<BD2ContextPrueba>(options => options.UseSqlServer(DB2PConnection));

builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod())
);

builder.Services.AddScoped<FuncionesPedidos>();

// Configurar Quartz
builder.Services.AddQuartz(q =>
{
    // Just use the name of your job that you created in the Jobs folder.
    var jobKey = new JobKey("SendEmailJob");
    q.AddJob<JobEmail>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmailJob-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)  
        // 0 0 9 ? * MON *
        .WithCronSchedule("0 0 8 ? * MON *")
    );
});

builder.Services.AddQuartz(q =>
{
    // Just use the name of your job that you created in the Jobs folder.
    var jobKey = new JobKey("SendEmailMesJob");
    q.AddJob<JobEmailMes>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmailMesJob-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)  
        .WithCronSchedule("0 10 8 1 * ?")
    );
});


builder.Services.AddQuartz(q =>
{
    // Just use the name of your job that you created in the Jobs folder.
    var jobKey = new JobKey("SendEmailJobMermas");
    q.AddJob<JobEmailMermasAla>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmailJobMermas-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)  
        .WithCronSchedule("0 45 8 * * ?")
    );
});

builder.Services.AddQuartz(q =>
{
    // Just use the name of your job that you created in the Jobs folder.
    var jobKey = new JobKey("SendEmailJobMermasB");
    q.AddJob<JobEmailMermasBoneless>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmailJobMermasB-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)  
        .WithCronSchedule("0 50 8 * * ?")
    );
});

builder.Services.AddQuartz(q =>
{
    // Just use the name of your job that you created in the Jobs folder.
    var jobKey = new JobKey("SendEmail25pts");
    q.AddJob<JobEmail25pts>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmail25pts-trigger")
        // This Cron interval can be described as "run every minute"(when second is zero)
        .WithCronSchedule("0 55 8 * * ?")
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API PEDIDOS",
        Version = "v0.0.1",
        Description = "API para administración de pedidos"
    });

    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key requerida en el header: x-api-key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "x-api-key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
