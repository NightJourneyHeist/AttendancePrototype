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

    // This is a GET request, from phone -> router -> bailey -> laptop
    [RelayCommand]
    private async Task CheckAttendanceAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(StudentId))
            {
                Greeting = "Please enter your ID";
                return;
            }
            string baileyIp = "192.168.1.100";
            string url = $"http://{baileyIp}/attendance?studentId={StudentId}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            string result = await response.Content.ReadAsStringAsync();

            Greeting = result;
        }
        catch (Exception ex)
        {
            Greeting = $"Check failed: {ex.Message}";
        }
    }

    // This is a POST request, from the phone -> router -> bailey -> laptop
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
            string baileyIp = "192.168.1.100";
            
            string url = $"http://{baileyIp}/attendance";

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
