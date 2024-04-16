using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var animals = new List<Animal>();
var visits = new List<Visit>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/animals", () => Results.Ok(animals))
   .WithName("GetAllAnimals");

app.MapGet("/animals/{id}", (int id) => Results.Ok(animals.FirstOrDefault(a => a.Id == id)))
   .WithName("GetAnimalById");

app.MapPost("/animals", (Animal animal) =>
{
    animal.Id = animals.Count + 1;
    animals.Add(animal);
    return Results.Created($"/animals/{animal.Id}", animal);
})
.WithName("AddAnimal");

app.MapPut("/animals/{id}", (int id, Animal animal) =>
{
    var existingAnimal = animals.FirstOrDefault(a => a.Id == id);
    if (existingAnimal != null)
    {
        existingAnimal.Name = animal.Name;
        existingAnimal.Category = animal.Category;
        existingAnimal.Weight = animal.Weight;
        existingAnimal.FurColor = animal.FurColor;
        return Results.Ok(existingAnimal);
    }
    return Results.NotFound($"Animal with id {id} not found.");
})
.WithName("UpdateAnimal");

app.MapDelete("/animals/{id}", (int id) =>
{
    var animalToRemove = animals.FirstOrDefault(a => a.Id == id);
    if (animalToRemove != null)
    {
        animals.Remove(animalToRemove);
        return Results.Ok($"Animal with id {id} removed.");
    }
    return Results.NotFound($"Animal with id {id} not found.");
})
.WithName("DeleteAnimal");

app.MapGet("/animals/{id}/visits", (int id) => Results.Ok(visits.Where(v => v.AnimalId == id)))
   .WithName("GetVisitsByAnimalId");

app.MapPost("/visits", (Visit visit) =>
{
    visit.Id = visits.Count + 1;
    visits.Add(visit);
    return Results.Created($"/visits/{visit.Id}", visit);
})
.WithName("AddVisit");

app.Run();

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public double Weight { get; set; }
    public string FurColor { get; set; }
}

public class Visit
{
    public int Id { get; set; }
    public DateTime VisitDate { get; set; }
    public int AnimalId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}