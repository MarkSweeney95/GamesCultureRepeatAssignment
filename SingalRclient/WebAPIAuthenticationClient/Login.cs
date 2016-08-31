using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIAuthenticationClient
{
    public class Login
    {
        static string baseAddress = "http://localhost:50574/";
        static void Main(string[] args)
        {

            //string baseAddress = "http://cgmonogameserver2015.azurewebsites.net/";


            Registration newPlayer = getRegistration();
            if (Register(newPlayer))
            {

                var client = new HttpClient();
                //var response = client.GetAsync(baseAddress + "api/Values").Result;
                //Console.WriteLine(response);
                //Console.ReadKey();

                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(
                //new MediaTypeWithQualityHeaderValue("application/json"));

                // Construct url encoded Form object
                var content = new FormUrlEncodedContent(new[]             {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", newPlayer.Email),
                new KeyValuePair<string, string>("password", newPlayer.Password), });

                // Call token WEB API with Form data
                var result = client.PostAsync(baseAddress + "Token", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Raw Content " + resultContent);
                Console.ReadKey();

                var tokenContent = result.Content.ReadAsAsync<Token>(
                        new[] { new JsonMediaTypeFormatter() }).Result;

                //Console.WriteLine();
                //Console.WriteLine("Access Token Field extracted " + tokenContent.AccessToken);
                //Console.ReadKey();

                if (!(string.IsNullOrEmpty(tokenContent.AccessToken)))
                {
                    //client.DefaultRequestHeaders.Authorization =
                    //        new AuthenticationHeaderValue("Bearer",
                    //        tokenContent.AccessToken);
                    //var authorizedResponse = client.GetAsync(baseAddress + "api/Values").Result;
                    ////Console.WriteLine(authorizedResponse);
                    //Console.WriteLine(authorizedResponse.Content.ReadAsStringAsync().Result);
                    //Console.ReadKey();


                }
                else Console.WriteLine("Invalid credentials");
            }
        }

        static public Registration getRegistration()
        {
            Registration r = new Registration();
            Console.WriteLine("Enter Email, Password, Confirm Password");
            r.Email = Console.ReadLine();
            r.Password = Console.ReadLine();
            r.ConfirmPassword = Console.ReadLine();

            return r;
        }

        static public bool Register(Registration playerReg)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]             {
                new KeyValuePair<string, string>("Email", playerReg.Email),
                new KeyValuePair<string, string>("Password", playerReg.Password),
                new KeyValuePair<string, string>("ConfirmPassword", playerReg.ConfirmPassword),});

                var result = client.PostAsync(baseAddress + "api/Account/Register", content).Result;
                if (!result.IsSuccessStatusCode)
                {
                    var message = result.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Error {0}", message);
                    Console.ReadKey();
                }

                return result.IsSuccessStatusCode;
            }

        }
    }
}
