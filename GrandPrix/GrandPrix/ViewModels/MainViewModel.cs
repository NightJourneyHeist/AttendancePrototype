using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System;

namespace GrandPrix.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private static readonly HttpClient _httpClient = new HttpClient();
    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";
    [ObservableProperty]
    public partial string StudentId {get; set;} = "";

    [RelayCommand]
    private async Task SubmitAttendanceAsync()
    {
        if(string.IsNullOrWhiteSpace(StudentId))
        {
            Greeting = "Please enter your ID";
            return;
        }
        Greeting = "Submitting. . .";

        try 
        {
            // set up the destination (this is from phone POV)
            string serverIp = "192.168.1.100";
            string port = "5134";
            string url = $"http://{serverIp}:{port}/attendance";

            // preapre paylod
            var payload = new {StudentId = StudentId};
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // yeet it!
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                Greeting = $"Nice to see you,{StudentId}!";
            }
            else
            {
                Greeting = $"Server error: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            Greeting = $"Mission Faild, Levi squad. Get her!: \n  {ex.Message}";
        }
    }
}
