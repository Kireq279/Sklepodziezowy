using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SklepOdziezowy
{
    // Klasa reprezentująca produkt
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }

        public override string ToString()
        {
            return $"{Id};{Name};{Size};{Price}";
        }

        public static Product FromCsv(string csvLine)
        {
            string[] parts = csvLine.Split(';');
            return new Product
            {
                Id = int.Parse(parts[0]),
                Name = parts[1],
                Size = parts[2],
                Price = decimal.Parse(parts[3])
            };
        }
    }

    // Zarządzanie danymi i plikami - CRUD 
    public class StoreManager
    {
        private List<Product> _products = new List<Product>();
        private const string FileName = @"C:\Users\ja\Desktop\Bazasklepu.txt";

        public StoreManager()
        {
            LoadFromFile();
        }

        // CREATE
        public void AddProduct(Product product)
        {
            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            _products.Add(product);
            SaveToFile();
        }

        // READ
        public List<Product> GetAll() => _products;

        
        public bool UpdateProduct(int id, string newName, string newSize, decimal newPrice)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return false;

            product.Name = newName;
            product.Size = newSize; 
            product.Price = newPrice;

            SaveToFile();
            return true;
        }

        // DELETE
        public bool DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return false;

            _products.Remove(product);
            SaveToFile();
            return true;
        }

        private void SaveToFile()
        {
            File.WriteAllLines(FileName, _products.Select(p => p.ToString()));
        }

        private void LoadFromFile()
        {
            if (File.Exists(FileName))
            {
                var lines = File.ReadAllLines(FileName);
                _products = lines.Select(Product.FromCsv).ToList();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StoreManager manager = new StoreManager();
            bool running = true;

            while (running)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("========================================");
                Console.WriteLine("   SKLEP ODZIEŻOWY - PANEL ZARZĄDZANIA  ");
                Console.WriteLine("========================================");
                Console.ResetColor();
                Console.WriteLine("1. [Lista]   Przeglądaj asortyment");
                Console.WriteLine("2. [Dodaj]   Wprowadź nowy produkt");
                Console.WriteLine("3. [Edytuj]  Zmień dane produktu");
                Console.WriteLine("4. [Usuń]    Wykreśl produkt z bazy");
                Console.WriteLine("5. [Wyjście] Zamknij program");
                Console.Write("\nWybierz opcję: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Wyświetlanie listy
                        var produkty = manager.GetAll();
                        if (produkty.Count == 0) Console.WriteLine("Lista jest pusta.");
                        produkty.ForEach(p => Console.WriteLine($"ID: {p.Id} | {p.Name} | Rozmiar: {p.Size} | Cena: {p.Price} zł"));
                        break;

                    case "2":
                        // Dodawanie
                        Console.Write("Nazwa: "); string name = Console.ReadLine();
                        Console.Write("Rozmiar: "); string size = Console.ReadLine();
                        Console.Write("Cena: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal price))
                        {
                            manager.AddProduct(new Product { Name = name, Size = size, Price = price });
                            Console.WriteLine("Dodano produkt.");
                        }
                        else Console.WriteLine("Błędna cena!");
                        break;

                    case "3":
                        // EDYCJA
                        Console.Write("Podaj ID produktu do edycji: ");
                        if (int.TryParse(Console.ReadLine(), out int editId))
                        {
                            Console.Write("Nowa nazwa: ");
                            string newName = Console.ReadLine();

                            Console.Write("Nowy rozmiar: "); 
                            string newSize = Console.ReadLine();

                            Console.Write("Nowa cena: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal newPrice))
                            {
                                // TUTAJ BYŁ BŁĄD - teraz przekazujemy 4 parametry, w tym newSize
                                bool success = manager.UpdateProduct(editId, newName, newSize, newPrice);

                                if (success)
                                    Console.WriteLine("Produkt został zaktualizowany.");
                                else
                                    Console.WriteLine("Nie znaleziono produktu o podanym ID.");
                            }
                            else Console.WriteLine("Błędny format ceny!");
                        }
                        else Console.WriteLine("Błędne ID!");
                        break;

                    case "4":
                        // USUWANIE 
                        Console.Write("Podaj ID do usunięcia: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            if (manager.DeleteProduct(id))
                                Console.WriteLine("Usunięto produkt.");
                            else
                                Console.WriteLine("Nie znaleziono produktu o tym ID.");
                        }
                        else Console.WriteLine("Błędny format ID!");
                        break;

                    case "5":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Niepoprawny wybór.");
                        break;
                }
             }
          }
       }
   }
