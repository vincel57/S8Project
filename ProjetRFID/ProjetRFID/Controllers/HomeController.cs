﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetRFID.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using ProjetRFID.Controllers;
using Microsoft.EntityFrameworkCore;
using ProjetRFID.Data;
using Microsoft.AspNetCore.Http;
using NuGet.Protocol;
using System.Security.Claims;
using System.Drawing;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using System.Reflection.Metadata;


namespace ProjetRFID.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Simple"))
            {
                return RedirectToAction("Index1", "Home");
            }
            else if (User.IsInRole("Expert"))
            {
                return RedirectToAction("Indexx", "Home");
            }
            else
                return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        [Authorize(Roles = "Expert")]
        public IActionResult Indexx()
        {
            return View("Index");
        }
        [Authorize(Roles = "Simple")]
        public IActionResult Index1()
        {
            return View();
        }
        [Authorize(Roles = "Expert")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Traitement(string checkbox_Analytique, string checkbox_KNN, string checkbox_RandomForest, string checkbox_SVM, int n_neighbors, string weight, string metric, float p,
            string metric_params, string algorithm, int leaf_size, int n_estimators, string criterion,
            int min_samples_split, int min_samples_leaf, float min_weight_fraction_leaf, int max_leaf_nodes,
            float min_impurity_decrease, int n_jobs, int entier_detail, int max_depth, float C, string kernel, string gamma,
            float coef0, float tol, float cache_size, int max_iter, string decision_function_shape, IFormFile file)
        {
            if (checkbox_Analytique == "Analytique")
            {
                var filePath = Path.Combine("Folder", file.FileName);

                // Enregistrer le fichier dans le répertoire spécifié
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Créer un objet FormData avec le chemin du fichier au lieu du fichier lui-même
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(filePath), "filePath");

                // Envoyer la requête HTTP avec le chemin du fichier
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync("http://localhost:5000/result", formData);
                    var result = await response.Content.ReadAsStringAsync();

                    ViewBag.Result = result;
                }
                return View("Index1");

            }

            if (checkbox_KNN == "KNN")
            {
                using (var client = new HttpClient())
                {
                    var requestData = new
                    {
                     n_neighbors = n_neighbors,
                    weight = weight,
                    metric = metric,
                    p = p,
                    metric_params = metric_params,
                    algorithm = algorithm,
                    leaf_size = leaf_size
                };

                    var content = new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("http://localhost:5000/knn", content);
                    var result = await response.Content.ReadAsStringAsync();

                    ViewBag.Result = result;
                }

                return View("Index");

            }
            if (checkbox_RandomForest == "RandomForest")
            {
                using (var client = new HttpClient())
                {
                    var requestData = new
                    {
                      n_estimators = n_estimators,
                    criterion = criterion,
                    min_samples_split = min_samples_split,
                    min_samples_leaf = min_samples_leaf,
                    min_weight_fraction_leaf = min_weight_fraction_leaf,
                    max_leaf_nodes = max_leaf_nodes,
                    min_impurity_decrease = min_impurity_decrease,
                    n_jobs = n_jobs,
                    entier_detail = entier_detail,
                    max_depth = max_depth
                };

                    var content = new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("http://localhost:5000/randomforest", content);
                    var result = await response.Content.ReadAsStringAsync();

                    ViewBag.Result = result;
                }

                return View("Index");


            }
            if (checkbox_SVM == "SVM")
            {
                using (var client = new HttpClient())
                {
                    var requestData = new
                    {
                        C = C,
                    kernel = kernel,
                    gamma = gamma,
                    coef0 = coef0,
                    tol = tol,
                    cache_size = cache_size,
                    max_iter = max_iter,
                    decision_function_shape = decision_function_shape
                };

                    var content = new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("http://localhost:5000/svm", content);
                    var result = await response.Content.ReadAsStringAsync();

                    ViewBag.Result = result;
                }

                return View("Index");


            }

            return default;
        }

        public async Task<ActionResult> Traitement(string checkbox_Analytique, string checkbox_KNN, string checkbox_RandomForest, string checkbox_SVM, int n_neighbors, string weight, string metric,float p,
            string metric_params, string algorithm,int leaf_size,int n_estimators,string criterion,
            int min_samples_split,int min_samples_leaf,float min_weight_fraction_leaf,int max_leaf_nodes,
            float min_impurity_decrease,int n_jobs,int entier_detail,int max_depth,float C,string kernel,string gamma,
            float coef0, float tol, float cache_size,int max_iter, string decision_function_shape)
        {
            if (checkbox_Analytique == "Analytique")
            {
                Analytique analytique = new Analytique();
                _context.Analytique.Add(analytique);
                await _context.SaveChangesAsync();
                Simulation simulation = new Simulation
                {
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),//Pour récupérer l'Id de la personne qui est connectée
                    idA = analytique.id,
                    time = DateTime.Now,
                };

                _context.Simulation.Add(simulation);


                await _context.SaveChangesAsync();

                await _context.SaveChangesAsync();
            }
           
            if (checkbox_KNN == "KNN")
            {
                KNN knn = new KNN();
                {
                    n_neighbors = knn.n_neighbors;
                    weight = knn.weight.ToString();
                    metric = knn.metric.ToString();
                    p = knn.p;
                    metric_params=knn.metric_params.ToString();
                    algorithm=knn.algorithm.ToString();
                    leaf_size=knn.leaf_size;
                    _context.KNN.Add(knn);
                    await _context.SaveChangesAsync();
                    Simulation simulation = new Simulation
                    {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),//Pour récupérer l'Id de la personne qui est connectée
                        idk = knn.id,
                        time = DateTime.Now,
                    };

                    _context.Simulation.Add(simulation);
                     await _context.SaveChangesAsync();
                };

            }
            if (checkbox_RandomForest == "RandomForest")
            {
                Random_Forest randomforest = new Random_Forest();
                {
                    n_estimators=randomforest.n_estimators;
                    criterion = randomforest.criterion.ToString();
                    min_samples_split = randomforest.min_samples_split;
                    min_samples_leaf = randomforest.min_samples_leaf;
                    min_weight_fraction_leaf = randomforest.min_weight_fraction_leaf;
                    max_leaf_nodes = randomforest.max_leaf_nodes;
                    min_impurity_decrease = randomforest.min_impurity_decrease;
                    n_jobs = randomforest.n_jobs;
                    entier_detail = randomforest.entier_detail;
                    max_depth=randomforest.max_depth;
                    _context.Random_Forest.Add(randomforest);
                    await _context.SaveChangesAsync();
                    Simulation simulation = new Simulation
                    {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),//Pour récupérer l'Id de la personne qui est connectée
                        idR = randomforest.id,
                        time = DateTime.Now,
                    };

                    _context.Simulation.Add(simulation);
                    await _context.SaveChangesAsync();
                };


            }
             if (checkbox_SVM == "SVM")
            {
                SVM svm = new SVM();
                {

                    C = svm.C;
                    kernel=svm.kernel.ToString();
                    gamma=svm.gamma.ToString();
                    coef0 = svm.coef0;
                    tol=svm.tol;
                    cache_size=svm.cache_size;
                    max_iter=svm.max_iter;
                    decision_function_shape=svm.decision_function_shape;
                    _context.SVM.Add(svm);
                    await _context.SaveChangesAsync();
                    Simulation simulation = new Simulation
                    {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),//Pour récupérer l'Id de la personne qui est connectée
                        idS = svm.id,
                        time = DateTime.Now,
                    };

                    _context.Simulation.Add(simulation);
                    await _context.SaveChangesAsync();

                };


            }

            return default;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    


}
