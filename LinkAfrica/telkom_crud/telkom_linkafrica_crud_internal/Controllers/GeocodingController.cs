using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeocodingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly GoogleMapsProvider _google;
        private readonly OpenCageProvider _openCage;
        private readonly OpenRouteServiceProvider _openRoute;
        private static int dailyGoogleCount = 0;
        private static int dailyGoogleCountConfigure = 0;

        private static DateTime lastResetDate = DateTime.UtcNow.Date;


        public GeocodingController(IConfiguration _conf, GoogleMapsProvider google, OpenCageProvider openCage, OpenRouteServiceProvider openRoute)
        {
            _configuration = _conf;
            _google = google;
            _openCage = openCage;
            _openRoute = openRoute;

            dailyGoogleCountConfigure = int.Parse(_configuration["DailyGoogleCount"].ToString());
        }

        [HttpGet]
        public IActionResult Get()
        {

            string Query = "SpatialQuery Controller Working";

            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(Query);



        }

        [HttpPost("GetCoordinates2")]
        public IActionResult GetCoordinates2(AddressSearch address)
        {
            return new JsonResult(address);
        }

        [HttpPost("GetCoordinates")]
        public IActionResult GetCoordinates(AddressSearch address)
        {
            if (string.IsNullOrWhiteSpace(address.Address))
                return BadRequest("Address is required.");

            ResetCounterIfNeeded();

            GeoResult result = null;

            if (dailyGoogleCount < dailyGoogleCountConfigure)
            {
                result = _google.Geocode(address.Address);
                if (result != null)
                {
                    Interlocked.Increment(ref dailyGoogleCount);
                    result.Source = "Google";
                }
            }

            if (result == null)
            {
                result = _openCage.Geocode(address.Address);
                if (result != null)
                {
                    result.Source = "OpenCage";
                }
            }

            if (result == null)
                return NotFound("Coordinates not found for the given address.");

            return new JsonResult(result);
        }

        [HttpPost("GetAddress")]
        public IActionResult GetAddress([FromForm] string coordinates)
        {
            if (string.IsNullOrWhiteSpace(coordinates))
                return BadRequest("Coordinates are required.");

            var parts = coordinates.Split(',');
            if (parts.Length != 2 ||
                !double.TryParse(parts[0], out var lat) ||
                !double.TryParse(parts[1], out var lng))
            {
                return BadRequest("Invalid coordinates format. Expected 'lat,lng'.");
            }

            ResetCounterIfNeeded();

            AddressResult result = null;

            if (dailyGoogleCount < dailyGoogleCountConfigure)
            {
                result = _google.ReverseGeocode(lat, lng);
                if (result != null)
                {
                    Interlocked.Increment(ref dailyGoogleCount);
                }
            }

            if (result == null)
            {
                result = _openCage.ReverseGeocode(lat, lng);
            }

            if (result == null)
                return NotFound("Address not found for the given coordinates.");

            return new JsonResult(result);
        }

        [HttpPost("GetRoute")]
        public IActionResult GetRoute(RoutInputCoord coordinates)
        {

            ResetCounterIfNeeded();

            RouteResult result1 = null;
            double result1Distance = 0;

            // First try Google (first 20 per day)
            if (dailyGoogleCount < dailyGoogleCountConfigure)
            {
                result1 = _google.GetRoute(coordinates.StartLat_Y, coordinates.StartLng_X, coordinates.EndLat_Y, coordinates.EndLng_X, coordinates.Route_Mode);

                if (result1 == null)
                    return NotFound("Route could not be determined.");


                if (result1.Distance.Split(' ')[1] == "km")
                {
                    result1Distance = double.Parse(result1.Distance.Split(' ')[0], CultureInfo.InvariantCulture) * 1000;
                }
                else if (result1.Distance.Split(' ')[1] == "m")
                {
                    result1Distance = double.Parse(result1.Distance.Split(' ')[0], CultureInfo.InvariantCulture);
                }

                if (result1 != null)
                {
                    Interlocked.Increment(ref dailyGoogleCount);
                }
            }

            //// Fallback to OpenRouteService
            //if (result == null)
            //{
            //    result = _openRoute.GetRoute(double.Parse(coordinates.StartLat_Y, CultureInfo.InvariantCulture), double.Parse(coordinates.StartLng_X, CultureInfo.InvariantCulture), double.Parse(coordinates.EndLat_Y, CultureInfo.InvariantCulture), double.Parse(coordinates.EndLng_X, CultureInfo.InvariantCulture));
            //}




            return new JsonResult(result1);
        }

        [HttpPost("GetRouteBothSide")]
        public IActionResult GetRouteBothSide(RoutInputCoord coordinates)
        {

            ResetCounterIfNeeded();

            RouteResult result1 = null;
            RouteResult result2 = null;
            double result1Distance = 0;
            double result2Distance = 0;

            // First try Google (first 20 per day)
            if (dailyGoogleCount < dailyGoogleCountConfigure)
            {
                result1 = _google.GetRoute(coordinates.StartLat_Y, coordinates.StartLng_X, coordinates.EndLat_Y, coordinates.EndLng_X);
                result2 = _google.GetRoute(coordinates.EndLat_Y, coordinates.EndLng_X, coordinates.StartLat_Y, coordinates.StartLng_X);


                if (result1.Distance.Split(' ')[1] == "km")
                {
                    result1Distance = double.Parse(result1.Distance.Split(' ')[0], CultureInfo.InvariantCulture) * 1000;
                }
                else if (result1.Distance.Split(' ')[1] == "m")
                {
                    result1Distance = double.Parse(result1.Distance.Split(' ')[0], CultureInfo.InvariantCulture);
                }


                if (result2.Distance.Split(' ')[1] == "km")
                {
                    result2Distance = double.Parse(result2.Distance.Split(' ')[0], CultureInfo.InvariantCulture) * 1000;
                }
                else if (result2.Distance.Split(' ')[1] == "m")
                {
                    result2Distance = double.Parse(result2.Distance.Split(' ')[0], CultureInfo.InvariantCulture);
                }



                if (result1 != null || result2 != null)
                {
                    Interlocked.Increment(ref dailyGoogleCount);
                }
            }

            //// Fallback to OpenRouteService
            //if (result == null)
            //{
            //    result = _openRoute.GetRoute(double.Parse(coordinates.StartLat_Y, CultureInfo.InvariantCulture), double.Parse(coordinates.StartLng_X, CultureInfo.InvariantCulture), double.Parse(coordinates.EndLat_Y, CultureInfo.InvariantCulture), double.Parse(coordinates.EndLng_X, CultureInfo.InvariantCulture));
            //}

            if (result1 == null && result2 == null)
                return NotFound("Route could not be determined.");

            if (result1Distance < result2Distance)
                return new JsonResult(result1);

            if (result2Distance < result1Distance)
                return new JsonResult(result2);

            return new JsonResult(result2);
        }

        [HttpPost("GetAddressData")]
        public IActionResult GetAddressData(AddressSearch _Address)
        {
            List<string> listAddress = new List<string>();

            try
            {

                listAddress = _google.GetSuggestAddress(_Address.Address);
            }
            catch (Exception ex) { }

            return new JsonResult(listAddress);
        }
        private void ResetCounterIfNeeded()
        {
            if (DateTime.UtcNow.Date != lastResetDate)
            {
                dailyGoogleCount = 0;
                lastResetDate = DateTime.UtcNow.Date;
            }
        }
    }

    public class GeoResult
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Source { get; set; } = "";
    }

    public class AddressResult
    {
        public string Address { get; set; }
        public string Source { get; set; } = "";
    }

    public class RouteResult
    {
        public List<(double lat, double lng)> Coords { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public string Source { get; set; } = "";
    }



    public interface IGeocodingProvider
    {
        GeoResult? Geocode(string address);
    }


    public class GoogleMapsProvider : IGeocodingProvider
    {
        private readonly HttpClient _http;
        private readonly string _apiKey = "AIzaSyBg7RVV_GE6Jpw-HVUZqK644xTFzs0m75c"; // Replace with your key

        public GoogleMapsProvider(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
        }

        public GeoResult? Geocode(string address)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";
            var response = _http.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode) return null;

            var json = response.Content.ReadAsStringAsync().Result;
            var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("results");
            if (results.GetArrayLength() == 0) return null;

            //var location = "null"; //= results[0].GetProperty("navigation_points").GetProperty("location");

            var firstResult = results[0];
            if (firstResult.TryGetProperty("navigation_points", out var location) && location.GetArrayLength() > 0)
            {
                var navLocation = location[0].GetProperty("location");
                return new GeoResult
                {
                    Latitude = navLocation.GetProperty("latitude").GetDouble(),
                    Longitude = navLocation.GetProperty("longitude").GetDouble()
                };

            }
            else
            {
                var navLocation = firstResult.GetProperty("geometry").GetProperty("location");
                return new GeoResult
                {
                    Latitude = navLocation.GetProperty("lat").GetDouble(),
                    Longitude = navLocation.GetProperty("lng").GetDouble()
                };
            }
            
        }

        public AddressResult? ReverseGeocode(double lat, double lng)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lng}&key={_apiKey}";
            var response = _http.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode) return null;

            var json = response.Content.ReadAsStringAsync().Result;
            var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("results");
            if (results.GetArrayLength() == 0) return null;

            var formattedAddress = results[0].GetProperty("formatted_address").GetString();
            return new AddressResult
            {
                Address = formattedAddress,
                Source = "Google"
            };
        }

        public RouteResult? GetRoute(string startLat, string startLng, string endLat, string endLng, string mode = "W")
        {
            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={endLat},{endLng}&destination={startLat},{startLng}&mode=walking&key={_apiKey}";
            if (mode == "D")
                url = $"https://maps.googleapis.com/maps/api/directions/json?origin={endLat},{endLng}&destination={startLat},{startLng}&mode=driving&key={_apiKey}";

            var response = _http.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode) return null;

            var json = response.Content.ReadAsStringAsync().Result;
            var doc = JsonDocument.Parse(json);
            var routes = doc.RootElement.GetProperty("routes");
            if (routes.GetArrayLength() == 0) return null;

            var coordinates = new List<(double lat, double lng)>();
            using var docJson = JsonDocument.Parse(json);
            var steps = doc.RootElement
                .GetProperty("routes")[0]
                .GetProperty("legs")[0]
                .GetProperty("steps");

            foreach (var step in steps.EnumerateArray())
            {
                string encodedPolyline = step.GetProperty("polyline").GetProperty("points").GetString();
                var stepCoords = DecodePolyline(encodedPolyline);
                coordinates.AddRange(stepCoords);
            }

            var leg = routes[0].GetProperty("legs")[0];
            var distance = leg.GetProperty("distance").GetProperty("text").GetString();
            var duration = leg.GetProperty("duration").GetProperty("text").GetString();

            return new RouteResult
            {
                Coords = coordinates,
                Distance = distance,
                Duration = duration,
                Source = "Google"
            };
        }

        public static List<(double lat, double lng)> DecodePolyline(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints))
                return new List<(double, double)>();

            var poly = new List<(double, double)>();
            char[] polylineChars = encodedPoints.ToCharArray();
            int index = 0;
            int currentLat = 0;
            int currentLng = 0;

            while (index < polylineChars.Length)
            {
                // Decode latitude
                int sum = 0;
                int shifter = 0;
                int next5Bits;
                do
                {
                    next5Bits = polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                } while (next5Bits >= 32 && index < polylineChars.Length);
                currentLat += ((sum & 1) != 0 ? ~(sum >> 1) : (sum >> 1));

                // Decode longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5Bits = polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                } while (next5Bits >= 32 && index < polylineChars.Length);
                currentLng += ((sum & 1) != 0 ? ~(sum >> 1) : (sum >> 1));

                poly.Add((currentLat / 1E5, currentLng / 1E5));
            }

            return poly;
        }

        public List<string> GetSuggestAddress(string address)
        {
            List<string> lstAddress = new List<string>();
            //var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(address)}&types=address&components=country:za&key={_apiKey}";

            var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(address)}&components=country:za&key={_apiKey}";


            var response = _http.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode) return null;

            var json = response.Content.ReadAsStringAsync().Result;
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("predictions", out var predictionsElement))
            {
                //throw new Exception("No predictions found.");
            }
            foreach (var prediction in predictionsElement.EnumerateArray())
            {
                if (prediction.TryGetProperty("description", out var descriptionElement))
                {
                    string description = descriptionElement.GetString();
                    lstAddress.Add(description);
                }
            }

            return lstAddress;
        }
    }

    public class OpenCageProvider : IGeocodingProvider
    {
        private readonly HttpClient _http;
        private readonly string _apiKey = "3005a6ccc43d4f1f9f27ed16b6cb535d"; // Replace with your key

        public OpenCageProvider(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
        }

        public GeoResult Geocode(string address)
        {
            var url = $"https://api.opencagedata.com/geocode/v1/json?q={Uri.EscapeDataString(address)}&key={_apiKey}";
            var response = _http.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode) return null;

            var json = response.Content.ReadAsStringAsync().Result;
            var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("results");
            if (results.GetArrayLength() == 0) return null;

            var geometry = results[0].GetProperty("geometry");
            return new GeoResult
            {
                Latitude = geometry.GetProperty("lat").GetDouble(),
                Longitude = geometry.GetProperty("lng").GetDouble()
            };
        }

        public AddressResult? ReverseGeocode(double lat, double lng)
        {
            var url = $"https://api.opencagedata.com/geocode/v1/json?q={lat}+{lng}&key={_apiKey}";
            var response = _http.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode) return null;

            var json = response.Content.ReadAsStringAsync().Result;
            var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("results");
            if (results.GetArrayLength() == 0) return null;

            var formattedAddress = results[0].GetProperty("formatted").GetString();
            return new AddressResult
            {
                Address = formattedAddress,
                Source = "OpenCage"
            };
        }

    }

    public class OpenRouteServiceProvider
    {
        private readonly HttpClient _http;
        private readonly string _apiKey = "5b3ce3597851110001cf62482a5a8849fd2e4efb9faadfa13b9fbee1"; // Replace with your actual key

        public OpenRouteServiceProvider(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
        }

        public RouteResult? GetRoute(double startLat, double startLng, double endLat, double endLng)
        {
            var url = "https://api.openrouteservice.org/v2/directions/driving-car";
            var body = new
            {
                coordinates = new[] {
                new[] { startLng, startLat }, // Note the order: lng, lat
                new[] { endLng, endLat }
            }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", _apiKey);

            var response = _http.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode) return null;

            var json = response.Content.ReadAsStringAsync().Result;
            var doc = JsonDocument.Parse(json);

            var summary = doc.RootElement
            .GetProperty("routes")[0]
            .GetProperty("summary");


            //var summary = doc.RootElement.GetProperty("features")[0].GetProperty("properties").GetProperty("summary");

            return new RouteResult
            {
                Distance = $"{summary.GetProperty("distance").GetDouble() / 1000:0.0} km",
                Duration = $"{summary.GetProperty("duration").GetDouble() / 60:0.0} min",
                Source = "OpenRouteService"
            };
        }
    }

}
