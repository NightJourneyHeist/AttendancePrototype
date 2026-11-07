var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/attendance", async (StudentData studentData) => 
{
    // ignore the submission if there is no data
    if (string.IsNullOrWhiteSpace(studentData.StudentId))
        return Results.BadRequest("Sorry, I don't know you :( ");

    Console.WriteLine($"[LAPTOP] Received attendance: {studentData.StudentId}");

    // pathing setup to update the attendance list
    string filePath = Path.Combine(AppContext.BaseDirectory,"Attendance.txt");

    // extract studentdata from the post request
    string logEntry = $"StudentID: {studentData.StudentId}{Environment.NewLine}";
    
    await File.AppendAllTextAsync(filePath, logEntry);

    Console.WriteLine($"[LAPTOP] Saved to: {filePath}");

    // add the student to the attendance list
    return Results.Ok($"Welcome {studentData.StudentId}!");
    
});

app.MapGet("/attendance", async (string studentId) =>
{
    // where to find the stuff
    string filePath = Path.Combine(AppContext.BaseDirectory,"Attendance.txt");

    if (!File.Exists(filePath))
        return Results.Ok(new { signedIn = false });
    string[] records = await File.ReadAllLinesAsync(filePath);
    bool signedIn = records.Contains($"StudentID: {studentId}");

    return Results.Ok(new {signedIn});
}); 

app.Run("http://0.0.0.0:5134");


// creates a way to store student data from post request
public record StudentData(string StudentId);

