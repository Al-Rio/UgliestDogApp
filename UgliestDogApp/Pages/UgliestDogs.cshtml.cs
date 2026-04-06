using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

// This is a PageModel for a Razor Page that handles displaying the Dogs
   public class UgliestDogsModel : PageModel
   {
        // List that will hold all dogs for the dropdown selection
       public List<SelectListItem> DogList { get; set; }
       // Property that will store the currently selected dog object
       public Dog SelectedDog { get; set; }

        // Handles HTTP GET requests to the page - loads the list of dogs
       public void OnGet()
       {
           LoadDogList();
       }

       public void OnPost(string selectedDog)
       {
           LoadDogList();
           if (!string.IsNullOrEmpty(selectedDog))
           {
                // Parse the selected Id and retrieve the Dog record.
               SelectedDog = GetDogById(int.Parse(selectedDog));
           }
       }
    
        // Helper method that loads the list of dogs from the SQLite database
        // for displaying in a dropdown menu
       private void LoadDogList()
       {
           DogList = new List<SelectListItem>();
           using (var connection = new SqliteConnection("Data Source=UgliestDogs.db"))
           {
               connection.Open();
               var command = connection.CreateCommand();
               command.CommandText = "SELECT Id, Name FROM Dogs";
               using (var reader = command.ExecuteReader())
               {
                   while (reader.Read())
                   {
                       DogList.Add(new SelectListItem
                       {
                           Value = reader.GetInt32(0).ToString(), // Dog ID as the value
                           Text = reader.GetString(1)   // Dog name as the display text
                       });
                   }
               }
           }
       }

        // Helper method that retrieves a specific dog by its ID from the database
        // Returns all details of the duck
       private Dog GetDogById(int id)
       {
           using (var connection = new SqliteConnection("Data Source=UgliestDogs.db"))
           {
               connection.Open();
               var command = connection.CreateCommand();
               command.CommandText = "SELECT * FROM Dogs WHERE Id = @Id"; 
               command.Parameters.AddWithValue("@Id", id); // Using parameterized query for security
               using (var reader = command.ExecuteReader())
               {
                   if (reader.Read())
                   {
                       return new Dog
                       {
                           Id = reader.GetInt32(0),
                           Name = reader.GetString(1),
                           Breed = reader.GetString(2),
                           Year = reader.GetInt32(3),
                           ImageFileName = reader.GetString(4)
                       };
                   }
               }
           }
           return null;
       }
   }

    // Simple model class representing a dog
   public class Dog
   {
       public int Id { get; set; }
       public string Name { get; set; }
       public string Breed { get; set; }
       public int Year { get; set; }
       public string ImageFileName { get; set; }
   }