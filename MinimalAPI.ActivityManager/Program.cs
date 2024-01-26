using AspNetCore.SwaggerUI.Themes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.ActivityManager.Library;
using System.Linq.Expressions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(Style.Dark);

Data.Activities.Add(new Activity()
{
    Budget = 500,
    Start = DateTime.Now,
    End = DateTime.Now.AddDays(10),
    IsCritical = false,
    Title = "Test Activity",
    Type = ActivityType.Mockup,
    Manager = new Person()
    {
        Name = "Paolo",
        Surname = "Rossi",
        Level = WorkLevel.Manager,
        Birthday = DateTime.Now.AddYears(-40),
        City = "Vergate sul Membro",
        Address = "Via delle mazze, 19",
        PhoneNumber = "1234567890",
        PostalCode = "12345"
    },
    Employees = new List<Person>()
    {
        new Person() { Name = "Domenico", Surname = "Sironi", Level = WorkLevel.Junior },
        new Person() { Name = "Sergio", Surname = "De Cillis", Level = WorkLevel.Junior }
    }
});

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapPost("/Activities", (Activity activity) =>
{
    Dictionary<string, string[]> errors = new Dictionary<string, string[]>();
    if (activity.Start > activity.End)
    {
        errors["Start"] = ["Il campo Start è successivo al campo End"];
        errors["End"] = ["Il campo Start è successivo al campo End"];
    }

    if(activity.Manager == null)
        errors["Manager"] = ["Il campo Manager è obbligatorio"];

    if (activity.Manager?.Level != WorkLevel.Manager)
    {
        if (errors.ContainsKey("Manager"))
            errors["Manager"].Append("Il campo Manager è obbligatorio");
        else
            errors["Manager"] = ["Il campo Manager è obbligatorio"];
    }

    if(activity.Budget < 0)
        errors["Budget"] = ["Il campo Budget deve essere positivo"];

    if (errors.Any())
        return Results.ValidationProblem(errors);

    Data.Activities.Add(activity);
    return Results.Created();
});

app.MapGet("/Activities/All", () =>
{
    //List<string> result = new List<string>();
    //foreach(var activity in Data.Activities)
    //{
    //    result.Add(activity.ToString());
    //}

    //return result;

    return Data.Activities;

    //return Data.Activities.Select(x => x.ToString());
});

// https://localhost:8080/Activities?id=xyz
// https://localhost:8080/Activities/xyz
app.MapGet("/Activities/{id}", (Guid id) =>
{
    //Activity? activity = null;

    //foreach (var a in Data.Activities)
    //{
    //    if (a.Id == id)
    //    {
    //        activity = a; 
    //        break;
    //    }
    //}
    //if (activity == null)
    //    return Results.BadRequest("Attività non trovata");

    //return Results.Ok(activity);

    //foreach (var a in Data.Activities)
    //    if (a.Id == id)
    //        return Results.Ok(a);

    //return Results.BadRequest("Attività non trovata");

    Activity? activity = Data.Activities.SingleOrDefault(a => a.Id == id);
    return activity != null ? 
        Results.Ok(activity) : 
        Results.BadRequest("Attività non trovata");
});

// https://localhost:8080/Activities/Critical => true
// https://localhost:8080/Activities/Critical?isCritical=true => true
// https://localhost:8080/Activities/Critical?isCritical=false => false

app.MapGet("/Activities/Critical", (bool isCritical = true) =>
{
    //var result = new List<Activity>();
    //foreach (var activity in Data.Activities)
    //{
    //    if (activity.IsCritical == isCritical)
    //    {
    //        result.Add(activity);
    //    }
    //}

    var result = Data.Activities.FindAll(x => x.IsCritical == isCritical);
    result = Data.Activities
                    .Where(x => x.IsCritical == isCritical)
                    .ToList();
    return result;
});

app.MapGet("/Activities/Type/{type}", (int type) =>
{
    if (Enum.IsDefined(typeof(ActivityType), type))
    {
    }

    if (Enum.GetValues<ActivityType>().Any(x => (int)x == type))
    {
        return Data.Activities
        .FindAll(x => x.Type == (ActivityType)type);
    }

    return Data.Activities
        .FindAll(x => (int)x.Type == type);
});

app.MapPut("/Activities/{id}", ([FromRoute]Guid id, [FromBody]Activity activity) =>
{
    #region Codice brutto
    //int count = 0;
    //Activity toEdit = null;
    //foreach(var a in Data.Activities)
    //{
    //    if(a.Id == activity.Id)
    //    {
    //        toEdit = a;
    //        count++;
    //    }

    //}
    //if(count == 1)
    //{
    //    toEdit.Title = activity.Title;
    //    toEdit.Budget = activity.Budget;
    //    return Results.NoContent();
    //} else if(count > 1)
    //{
    //    return Results.Problem();
    //} else
    //{
    //    return Results.BadRequest();
    //}
    #endregion
    try
    {
        Activity? toEdit = Data.Activities.SingleOrDefault(x => x.Id == id);

        if (toEdit != null)
        {
            toEdit.Title = activity.Title;
            toEdit.Budget = activity.Budget;
            return Results.NoContent();
        }
        return Results.BadRequest("ID Attività non trovato");
    }
    catch (InvalidOperationException)
    {
        return Results.Problem("Errore nel database");
    }
    catch(Exception)
    {
        return Results.Problem("Errore nel server");
    }
});

app.MapDelete("/Activities/{id}", (Guid id) =>
{
    //try
    //{
    //    Activity? activity = Data.Activities.SingleOrDefault(a => a.Id == id);
    //    if (activity != null)
    //    {
            return Data.Activities.RemoveAll(a => a.Id == id) == 1
                ? Results.NoContent()
                : Results.BadRequest();
    //    }
    //    return Results.BadRequest("Attività non trovata");
    //}
    //catch (InvalidOperationException)
    //{
    //    return Results.Problem("Errore nel database");
    //}
    //catch (Exception)
    //{
    //    return Results.Problem("Errore nel server");
    //}
});

app.MapGet("/Activities/{id}/Description", (Guid id) =>
{
    try
    {
        Activity? activity = Data.Activities.SingleOrDefault(a => a.Id == id);

        // DOMANDA ? SE VERA : SE FALSA

        return activity != null 
            ? Results.Ok(activity.ToDescription()) 
            : Results.BadRequest("Attività non trovata");
    }
    catch(InvalidOperationException)
    {
        return Results.Problem("Errore nel database");
    }
    catch(Exception)
    {
        return Results.Problem("Errore nel server");
    }
});

app.MapPost("/Activities/{id}/Employees", ([FromRoute] Guid id, [FromBody] Person employee) =>
{
    try
    {
        Activity? activity = Data.Activities.SingleOrDefault(a => a.Id == id);

        // DOMANDA ? SE VERA : SE FALSA

        if (activity == null)
            return Results.BadRequest("Attività non trovata");

        activity.Employees.Add(employee);
        return Results.NoContent();
    }
    catch (InvalidOperationException)
    {
        return Results.Problem("Errore nel database");
    }
    catch (Exception)
    {
        return Results.Problem("Errore nel server");
    }
});

app.MapDelete("/Activities/{activityId}/Employees/{employeeId}", 
    (Guid activityId, Guid employeeId) =>
{
    try
    {
        Activity? activity = Data.Activities.SingleOrDefault(a => a.Id == activityId);

        // DOMANDA ? SE VERA : SE FALSA

        if (activity == null)
            return Results.BadRequest("Attività non trovata");

        activity.Employees.RemoveAll(p => p.Id == employeeId);
        return Results.NoContent();
    }
    catch (InvalidOperationException)
    {
        return Results.Problem("Errore nel database");
    }
    catch (Exception)
    {
        return Results.Problem("Errore nel server");
    }
});

app.MapGet("/Employees/{id}/IsInActivity", (Guid id) =>
{
    return GetPersonById(id);
});

app.MapPut("/Activities/{id}/SetCritical/{critical}", (Guid id, bool critical) =>
{
    try
    {
        Activity? activity = Data.Activities.SingleOrDefault(a => a.Id == id);

        // DOMANDA ? SE VERA : SE FALSA

        if (activity == null)
            return Results.BadRequest("Attività non trovata");
        
        if(activity.IsCritical == critical)
            return Results.NoContent();

        if(critical)
        {
            activity.Budget += 1000;
        } 
        else
        {
            //activity.Budget -= 1000;
            //activity.Budget = activity.Budget >= 1000 ? activity.Budget : 1000;
            activity.Budget = Math.Max(activity.Budget - 1000, 1000);
        }
        activity.IsCritical = critical;
        return Results.NoContent();
    }
    catch (InvalidOperationException)
    {
        return Results.Problem("Errore nel database");
    }
    catch (Exception)
    {
        return Results.Problem("Errore nel server");
    }
});

app.MapGet("/Employees/{id}/Description", (Guid id) =>
{
    Person? p = GetPersonById(id);
    //return p != null ? p.ToDescription() : null;
    return p?.ToDescription();
});

app.UseStaticFiles();
app.Run();

Person? GetPersonById(Guid id)
{
    return Data.Activities.SelectMany(a => a.Employees)
        .Union(Data.Activities.Select(a => a.Manager))
        .FirstOrDefault(p => p.Id == id);
}