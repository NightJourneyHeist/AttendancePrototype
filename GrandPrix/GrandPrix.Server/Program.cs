var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/attendance", async (StudentData studentData) => 
{
    // ignore the submission if there is no data
    if (string.IsNullOrWhiteSpace(studentData.StudentId))
    {
        return Results.BadRequest("Sorry, I don't know you :( ");
    }

    // extract studentdata from the post request
    string logEntry = $"\nStudentID: {studentData.StudentId}";
    
    // pathing setup to update the attendance list
    string filePath = "Attendance.txt";
    await File.AppendAllTextAsync(filePath, logEntry);

    // add the student to the attendance list
    return Results.Ok($"Welcome {studentData.StudentId}!");
    
});

app.Run();


// creates a way to store student data from post request
public record StudentData(string StudentId);

