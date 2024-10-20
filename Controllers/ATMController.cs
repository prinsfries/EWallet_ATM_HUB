using HUB.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HUB.Controllers
{

    //Note: Fixed Get User API so now it *does* show the money in the account
    //      CREATE, EDIT, DELETE are all working
    public class ATMController : Controller
    {
        // GET: ATMController
        public async Task<ActionResult> Index()
        {
            string apiUrl = "https://localhost:7272/api/user/get-users";

            List<EWallet> ewallet = new List<EWallet>();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                var result = await response.Content.ReadAsStringAsync();
                ewallet = JsonConvert.DeserializeObject<List<EWallet>>(result);
            }
            return View(ewallet);
        }

        // GET: ATMController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ATMController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EWallet ewallet)
        {
            string apiUrl = "https://localhost:7272/api/user/add-user";
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(ewallet), Encoding.UTF8, "application/json");
                try
                {
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", "Error deleting user: " + errorMessage);
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", "Request error: " + ex.Message);
                }
            }
            return View(ewallet);
        }

        // GET: ATMController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ATMController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(EWallet ewallet)
        {
            string apiUrl = "https://localhost:7272/api/user/update-ewallet";

            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(ewallet), Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PatchAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", "Error updating e-wallet: " + errorMessage);
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", "Request error: " + ex.Message);
                }
            }
            return View(ewallet);
        }



        // GET: ATMController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ATMController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(EWallet ewallet)
        {
            string apiUrl = $"https://localhost:7272/api/user/delete-user/{ewallet.EWPin}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", "Error deleting user: " + errorMessage);
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", "Request error: " + ex.Message);
                }
            }
            return View(ewallet);
        }

        public async Task<string> GetRandomJoke()
        {
            string jokeApiUrl = "https://geek-jokes.sameerkumar.website/api?format=json";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(jokeApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    dynamic jokeData = JsonConvert.DeserializeObject(result);
                    return jokeData.joke.ToString();
                }
            }
            return "No joke available.";
        }

        public async Task<ActionResult> Joke()
        {
            string randomJoke = await GetRandomJoke();
            return View((object)randomJoke);
        }


    }
}
