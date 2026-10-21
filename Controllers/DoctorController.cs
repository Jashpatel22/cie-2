using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using HospitalAppointment.Models;
using HospitalAppointment.Extensions;
using System.Text.Json;

namespace HospitalAppointment.Controllers
{
    // This controller handles doctor and appointment operations.
    public class DoctorController : Controller
    {
        // Stores the memory cache service.
        private readonly IMemoryCache _memoryCache;

        // Stores the HttpClient factory.
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor receives required services.
        public DoctorController(
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory)
        {
            // Assign memory cache service.
            _memoryCache = memoryCache;

            // Assign HttpClient factory.
            _httpClientFactory = httpClientFactory;
        }

        // Creates the list of doctors.
        private List<Doctor> GetDoctors()
        {
            // Return sample doctor records.
            return new List<Doctor>
            {
                new Doctor
                {
                    DoctorId = 1,
                    DoctorName = "Dr. Amit Patel",
                    Specialization = "Cardiologist",
                    Experience = 10,
                    ConsultationFee = 500
                },

                new Doctor
                {
                    DoctorId = 2,
                    DoctorName = "Dr. Priya Shah",
                    Specialization = "Dermatologist",
                    Experience = 7,
                    ConsultationFee = 400
                },

                new Doctor
                {
                    DoctorId = 3,
                    DoctorName = "Dr. Raj Mehta",
                    Specialization = "Neurologist",
                    Experience = 12,
                    ConsultationFee = 800
                }
            };
        }

        // Displays the doctor list.
        public IActionResult Index()
        {
            // Try to get the doctor list from memory cache.
            if (!_memoryCache.TryGetValue(
                "DoctorList",
                out List<Doctor> doctors))
            {
                // Get doctor data if it is not cached.
                doctors = GetDoctors();

                // Create cache settings.
                var cacheOptions = new MemoryCacheEntryOptions()
                    // Keep the doctor list for 3 minutes.
                    .SetAbsoluteExpiration(
                        TimeSpan.FromMinutes(3));

                // Store the doctor list in memory cache.
                _memoryCache.Set(
                    "DoctorList",
                    doctors,
                    cacheOptions);
            }

            // Send doctor list to the view.
            return View(doctors);
        }

        // Enable response caching for doctor details.
        [ResponseCache(Duration = 60)]
        public IActionResult Details(int id)
        {
            // Get the list of doctors.
            var doctors = GetDoctors();

            // Find the doctor using DoctorId.
            var doctor = doctors.FirstOrDefault(
                d => d.DoctorId == id);

            // Check whether the doctor exists.
            if (doctor == null)
            {
                // Return 404 if doctor does not exist.
                return NotFound();
            }

            // Calculate total consultation charge.
            ViewBag.TotalCharge =
                doctor.CalculateTotalCharge();

            // Display doctor details.
            return View(doctor);
        }

        // Displays appointment form.
        public IActionResult Appointment(int id)
        {
            // Get doctor list.
            var doctors = GetDoctors();

            // Find selected doctor.
            var doctor = doctors.FirstOrDefault(
                d => d.DoctorId == id);

            // Check whether doctor exists.
            if (doctor == null)
            {
                // Return 404 if doctor does not exist.
                return NotFound();
            }

            // Display appointment form.
            return View(doctor);
        }

        // Handles appointment form submission.
        [HttpPost]
        public IActionResult Appointment(
            int DoctorId,
            string PatientName,
            string Specialization)
        {
            // Store patient name in a cookie.
            Response.Cookies.Append(
                "PatientName",
                PatientName);

            // Store specialization in session.
            HttpContext.Session.SetString(
                "Specialization",
                Specialization);

            // Store success message.
            ViewBag.Message =
                "Appointment request submitted successfully.";

            // Display appointment page.
            return View();
        }

        // Retrieves posts asynchronously from JSONPlaceholder.
        public async Task<IActionResult> Posts()
        {
            // Create HttpClient.
            var client =
                _httpClientFactory.CreateClient();

            // API URL.
            string url =
                "https://jsonplaceholder.typicode.com/posts";

            // Send GET request asynchronously.
            var response =
                await client.GetAsync(url);

            // Check whether request was successful.
            if (!response.IsSuccessStatusCode)
            {
                // Return error message.
                return Content(
                    "Unable to retrieve posts.");
            }

            // Read response as text asynchronously.
            string json =
                await response.Content.ReadAsStringAsync();

            // Convert JSON into Post objects.
           // Create JSON serializer options.
            var options = new JsonSerializerOptions
            {
            // Allow JSON property names such as "id" and "title"
            // to match C# properties "Id" and "Title".
                PropertyNameCaseInsensitive = true
            };

// Convert JSON response into Post objects.
var posts =
    JsonSerializer.Deserialize<List<Post>>(json, options);

            // Send posts to the view.
            return View(posts);
        }
    }

    // Represents a post received from the API.
   public class Post
{
    // Stores the post ID received from the API.
    public int Id { get; set; }

    // Stores the post title received from the API.
    public string Title { get; set; } = string.Empty;
}
}