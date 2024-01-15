using Microsoft.AspNetCore.Http.HttpResults;
using MinimalAPI.ActivityManager.Library;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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
    List<string> result = new List<string>();
    foreach(var activity in Data.Activities)
    {
        result.Add(activity.ToString());
    }

    return result;

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

app.Run();
