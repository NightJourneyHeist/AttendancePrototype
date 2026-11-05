var builder = WebApplication.CreateBuilder(args);

//specifies the port used
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

// 1. Prompt the user for startup configurations in the console
Console.Write("Enter the database file name (e.g., 'Control & computers'): ");
string ClassName = Console.ReadLine()?.Trim();
if (string.IsNullOrEmpty(ClassName)) ClassName = "UnnamedClass";
ClassName = ClassName.Replace("[", "").Replace("]", "");
string dbFileName = $"{ClassName}.attendance";

// 2. Pass inputs to the Database Manager
DatabaseManager.InitializeDatabase(dbFileName, ClassName);
DatabaseManager.SeedNamesIfNeeded(dbFileName);

// 3. API Endpoint
//handles updating the attendance status, to update attendance, send something like this "/attendance?number=3"
app.MapGet("/attendance", async (int? number) =>
{
    if (number == null)
    {
        return Results.BadRequest(new { Error = "Missing parameter", Details = "The 'number' query parameter is required." });
    }

    try
    {
        bool UpdateAttendance = await DatabaseManager.UpdateAttendanceAsync(dbFileName, ClassName, number.Value);

        if (UpdateAttendance)
        {
            return Results.Content("AU"); // Attendance Updated
        }
        else
        {
            return Results.NotFound(new { Error = "Not Found", Details = $"Student number {number.Value} does not exist." });
        }
    }
    catch (Exception ex)
    {
        return Results.Json(new { Error = "Database Error", Message = ex.Message }, statusCode: 500);
    }
});


//handles checking and sending back the attendance status, to ask for attendance, send something like this "/status?number=3"
app.MapGet("/status", async (int? number) =>
{
    if (number == null)
    {
        return Results.BadRequest(new { Error = "Missing parameter", Details = "The 'number' query parameter is required." });
    }

    try
    {
        int? status = await DatabaseManager.GetAttendanceStatusAsync(dbFileName, ClassName, number.Value);

        if (status != null)
        {
            return Results.Ok(new { Number = number.Value, CheckedIn = status.Value == 1 });
        }
        else
        {
            return Results.NotFound(new { Error = "Not Found", Details = $"Student number {number.Value} does not exist." });
        }
    }
    catch (Exception ex)
    {
        return Results.Json(new { Error = "Database Error", Message = ex.Message }, statusCode: 500);
    }
});

app.Run();