using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CustomerApp.Models;
using System.Data;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Data.Common;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;


namespace CustomerApp.Controllers;

public class HomeController : Controller
{

    
    private readonly ILogger<HomeController> _logger;

    string baseURL ="http://localhost:5107/api/";
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }


[HttpGet]
public IActionResult Login()
{
    // Código para mostrar la página de inicio de sesión
    return View();
}
[HttpPost]
[Route("login")]

public async Task<IActionResult> Login(LoginViewModel model)
{
    
    if (ModelState.IsValid)
    {
        // Crear un objeto para enviar las credenciales a la API
        var loginData = new
        {
            Username = model.Username,
            Password = model.Password
        };

        // Serializar el objeto a JSON
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(loginData),
            Encoding.UTF8,
            "application/json"
        );

        // Enviar la solicitud POST a la API para verificar las credenciales
        using (var httpClient = new HttpClient())
        {
             Console.WriteLine("Enviando solicitud POST a la API");
            
            var apiUrl2 = $"{baseURL}Main/login";

                Console.WriteLine($"URL de solicitud: {apiUrl2}");
                 Console.WriteLine($"Contenido de la solicitud: {await jsonContent.ReadAsStringAsync()}");

            var response = await httpClient.PostAsync(apiUrl2, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // La autenticación fue exitosa
                // Aquí puedes redirigir al usuario a la página principal u otra página después del inicio de sesión
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // La autenticación falló
                ModelState.AddModelError("", "Credenciales inválidas");
                var responseContent = await response.Content.ReadAsStringAsync();
                 Console.WriteLine($"Respuesta de la API en caso de error: {responseContent}");
            }
        }
    }

    // Si la autenticación falla o el modelo no es válido, muestra la vista de inicio de sesión
    return View(model);
}

private async Task<DataTable> GetDataFromApiAsync()
{
    using (var client = new HttpClient())
    {
        // Establece la URL de la API
        string apiUrl = "http://localhost:5107/api/Main";

        // Realiza una solicitud GET a la API
        HttpResponseMessage response = await client.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            // Lee la respuesta como una cadena JSON
            string json = await response.Content.ReadAsStringAsync();

            // Deserializa el JSON en un DataTable
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(json);

            return dataTable;
        }
        else
        {
            // Manejar el error de la solicitud
            Console.WriteLine("Error llamando a la API");
            return null; // O devuelve un DataTable vacío o maneja el error según tus necesidades
        }
    }
}
    public async Task<IActionResult> Index()
    {
    // Llama a GetDataFromApiAsync para obtener los datos de la API
    DataTable dt = await GetDataFromApiAsync();

    if (dt != null)
    {
        // Los datos se han cargado correctamente, pásalos a la vista
        ViewData.Model = dt;
    }
    else
    {
        // Aquí puedes manejar el caso de error según tus necesidades
    }

    return View();
}

public IActionResult Create()
{
    // Redirige a la acción "Create" en el controlador "CustomerController"
    return Redirect("/customer/create");
}

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
