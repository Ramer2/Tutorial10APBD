using Microsoft.EntityFrameworkCore;
using Tutorial10.RestAPI;
using Tutorial10.RestAPI.DTOs;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("EmployeeDatabase");

builder.Services.AddDbContext<SampleCompanyContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/jobs", async (SampleCompanyContext context, CancellationToken cancellationToken) => {
    try
    {
        return Results.Ok(await context.Jobs.ToListAsync(cancellationToken));
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/departments", async (SampleCompanyContext context, CancellationToken cancellationToken) => {
    try
    {
        return Results.Ok(await context.Departemnts.ToListAsync(cancellationToken));
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/employees", async (SampleCompanyContext context, CancellationToken cancellationToken) =>
{   
    try
    {
        var employees = await context.Employees.ToListAsync(cancellationToken);
        var employeeDtos = new List<EmployeeDto>();
        foreach (var employee in employees)
        {
            employeeDtos.Add(new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                JobId = employee.JobId,
                HireDate = employee.HireDate,
                Salary = employee.Salary,
                Commission = employee.Commission,
                DepartmentId = employee.DepartmentId
            });
        }
        
        return Results.Ok(employeeDtos);

    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/employees/{id}", async (SampleCompanyContext context, CancellationToken cancellationToken, int id) =>
{
    try
    {
        var employee = await context.Employees.FindAsync(id, cancellationToken);

        if (employee == null)
        {
            return Results.NotFound("Employee not found");
        }
        
        return Results.Ok(new EmployeeDto
        {
            Id = employee.Id,
            Name = employee.Name,
            JobId = employee.JobId,
            HireDate = employee.HireDate,
            Salary = employee.Salary,
            Commission = employee.Commission,
            DepartmentId = employee.DepartmentId
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);      
    }
});

app.MapPost("/api/employees", async (SampleCompanyContext context, CancellationToken cancellationToken, Employee employeeDto) =>
{
    try
    {
        await context.Employees.AddAsync(employeeDto, cancellationToken);
        await context.SaveChangesAsync();
        return Results.Created($"/api/employees/{employeeDto.Id}", employeeDto);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPut("/api/employees/{id}", (int id) =>
{
    
});

app.MapDelete("/api/employees/{id}", (int id) =>
{
    
});

app.Run();
