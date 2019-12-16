using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NLog;
using NorthwindConsole.Models;

namespace NorthwindConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories and their description");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display specific Category and its related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Edit Category record");
                    Console.WriteLine("6) Delete Category");
                    Console.WriteLine("7) Add Product");
                    Console.WriteLine("8) Edit Product record");
                    Console.WriteLine("9) Display all Product Names");
                    Console.WriteLine("10) Display Product and all its fields");
                    Console.WriteLine("11) Display all Categories with Active Products");
                    Console.WriteLine("12) Delete Product");

                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");

                    // Display all Categories in the Categories table (CategoryName and Description)
                    if (choice == "1")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($" {item.CategoryId}) {item.CategoryName} - {item.Description}");
                        }
                    }
                    else if (choice == "2")
                    {
                        Category Category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        Category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");

                        Category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(Category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(Category, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == Category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed, category added");
                                // save category to db
                                db.Categories.Add(Category);
                                db.SaveChanges();
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "3")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose active products you want to display:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            if (p.Discontinued == false)
                            {
                                Console.WriteLine(p.ProductName);
                            }
                        }
                    }
                    else if (choice == "4")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                        Console.WriteLine();
                    }
                    // Edit a specified record from the Categories table
                    else if (choice == "5")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName} - {item.Description}");
                        }

                        Console.WriteLine("Choose Category to update by selecting ID");
                        int categoryOption = int.Parse(Console.ReadLine());
                        logger.Info($"CategoryId {categoryOption} selected");
                        query = db.Categories.Include("Products").OrderBy(c => c.CategoryId);
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == categoryOption);
                        Console.WriteLine("Choose which field you would like to update by selecting the number");
                        Console.WriteLine("1) CategoryName");
                        Console.WriteLine("2) CategoryDescription");

                        int fieldOption = int.Parse(Console.ReadLine());

                        Console.WriteLine("Enter what you would like to change the chosen field to: ");

                        if (fieldOption == 1)
                        {
                            category.CategoryName = Console.ReadLine();
                            logger.Info("CategoryName updated");
                        } else if (fieldOption == 2)
                        {
                            category.Description = Console.ReadLine();
                            logger.Info("CategoryDescription updated");
                        }
                        logger.Info($"CategoryID {category.CategoryId} updated.");
                        db.SaveChanges();
                        Console.WriteLine();
                    }
                    // Delete a specified existing record from the Categories table
                    else if (choice == "6")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.WriteLine("Choose a record to delete from the Categories table by selecting the ID");
                        int id = int.Parse(Console.ReadLine());
                        bool idExists = db.Categories.Any(p => p.CategoryId == id);
                        if (idExists)
                        {
                            Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                            logger.Info("Validation passed, category removed");
                            db.Categories.Remove(category);
                            db.SaveChanges();
                        }
                        else
                        {
                            logger.Info("Category ID does not exist");
                        }
                        Console.WriteLine();
                    }
                    // Add new record to Product Table
                    else if (choice == "7")
                    {
                        Product product = new Product();
                        Console.WriteLine("Enter Product Name:");
                        product.ProductName = Console.ReadLine();
                        var db = new NorthwindContext();
                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);
                        if (isValid)
                        {
                            // check for unique name
                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed, product added");
                                // save category to db
                                db.Products.Add(product);
                                db.SaveChanges();
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                        Console.WriteLine();
                    }

                    // Edit a specified record from the Product Table
                    else if (choice == "8")
                    {
                        var db = new NorthwindContext();
                        var query = db.Products.OrderBy(p => p.ProductID);

                        Console.WriteLine("Choose Product to update by selecting ID:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.ProductID}) {item.ProductName}");
                        }
                        string input = Console.ReadLine();
                        int value;
                        if (int.TryParse(input, out value))
                        {
                            int id = int.Parse(input);
                            if(db.Products.Any(p => p.ProductID == id))
                            {
                                Console.Clear();
                                logger.Info($"ProductID {id} selected");
                                Product product = db.Products.FirstOrDefault(p => p.ProductID == id);

                                Console.WriteLine("Select the field you want to edit by selecting number:");
                                Console.WriteLine("1) ProductName: " + product.ProductName);
                                Console.WriteLine("2) QuantityPerUnit: " + product.QuantityPerUnit);
                                Console.WriteLine("3) UnitPrice: " + product.UnitPrice);
                                Console.WriteLine("4) UnitsInStock: " + product.UnitsInStock);
                                Console.WriteLine("5) UnitsOnOrder: " + product.UnitsOnOrder);
                                Console.WriteLine("6) ReorderLevel: " + product.ReorderLevel);
                                Console.WriteLine("7) Discontinued: " + product.Discontinued);
                                int fieldOption = int.Parse(Console.ReadLine());
                                Console.WriteLine("Enter what you like to change the field to: ");
                                if (fieldOption == 1)
                                {
                                    product.ProductName = Console.ReadLine();
                                }
                                else if (fieldOption == 2)
                                {
                                    product.QuantityPerUnit = Console.ReadLine();
                                }
                                else if (fieldOption == 3)
                                {
                                    product.UnitPrice = int.Parse(Console.ReadLine());
                                }
                                else if (fieldOption == 4)
                                {
                                    product.UnitsInStock = short.Parse(Console.ReadLine());
                                }
                                else if (fieldOption == 5)
                                {
                                    product.UnitsOnOrder = short.Parse(Console.ReadLine());
                                }
                                else if (fieldOption == 6)
                                {
                                    product.ReorderLevel = short.Parse(Console.ReadLine());
                                }
                                else if (fieldOption == 7)
                                {
                                    product.Discontinued = bool.Parse(Console.ReadLine());
                                } 
                                logger.Info($"Product {product.ProductName} updated");
                                db.SaveChanges();
                            } else
                            {
                                logger.Info($"ProductID {id} does not exist");
                            }
                        } else
                        {
                            logger.Info($"Invalid input, type an integer");
                        }
                    }

                    // Display all records in the Products table (ProductName only) - 
                    // user decides if they want to see all products, discontinued products, or active (not discontinued) products.
                    // Discontinued products should be distinguished from active products.
                    else if (choice == "9")
                    {
                        Console.WriteLine("Select option for displaying Products");
                        Console.WriteLine("1) Display all products");
                        Console.WriteLine("2) Display active products");
                        Console.WriteLine("3) Display discontinued products");
                        string input = Console.ReadLine();
                        if (input == "1" || input == "2" || input == "3")
                        {
                            int displayOption = int.Parse(input);
                            logger.Info($"Display option {displayOption} selected");
                            var db = new NorthwindContext();
                            var query = db.Products.OrderBy(p => p.ProductID);
                            foreach (var item in query)
                            {

                                if (item.Discontinued == false)
                                {
                                    if (displayOption == 1 || displayOption == 2)
                                    {
                                        Console.WriteLine("Active Product: " + $"{item.ProductName}");
                                    }
                                }
                                if (item.Discontinued == true)
                                {
                                    if (displayOption == 1 || displayOption == 3)
                                    {
                                        Console.WriteLine("Discontinued Product: " + $"{item.ProductName}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            logger.Info($"Invalid Display Option selected, select 1, 2, 3");
                        }

                        Console.WriteLine();
                    }

                    // Display a specific Product (all product fields should be displayed)
                    else if (choice == "10")
                    {
                        var db = new NorthwindContext();
                        var query = db.Products.OrderBy(p => p.ProductID);

                        Console.WriteLine("Select the Product whose fields you want to display:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.ProductID}) {item.ProductName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        bool idExists = db.Products.Any(p => p.ProductID == id);
                        if (idExists)
                        {
                            Console.Clear();
                            logger.Info($"ProductID {id} selected");
                            Product product = db.Products.FirstOrDefault(p => p.ProductID == id);
                            Console.WriteLine("ProductID: " + product.ProductID);
                            Console.WriteLine("ProductName: " + product.ProductName);
                            Console.WriteLine("QuantityPerUnit\n\t" + product.QuantityPerUnit);
                            Console.WriteLine("UnitPrice\n\t" + product.UnitPrice);
                            Console.WriteLine("UnitsInStock\n\t" + product.UnitsInStock);
                            Console.WriteLine("UnitsOnOrder\n\t" + product.UnitsOnOrder);
                            Console.WriteLine("ReorderLevel\n\t" + product.ReorderLevel);
                            Console.WriteLine("Discontinued\n\t" + product.Discontinued);
                            Console.WriteLine("SupplierId\n\t" + product.SupplierId);
                            Console.WriteLine("CategoryId\n\t" + product.CategoryId);
                            Console.WriteLine();
                        } else
                        {
                            logger.Info("Product ID does not exist");
                        }
                    }

                    // Display all Categories and their related active (not discontinued) 
                    // product data (CategoryName, ProductName)
                    else if (choice == "11")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            foreach (Product p in item.Products)
                            {
                                if (p.Discontinued == false)
                                {
                                    Console.WriteLine($"{item.CategoryName}, {p.ProductName}");
                                }
                            }
                        }
                        Console.WriteLine();
                    }
                    // Delete a specified existing record from the Products table
                    else if (choice == "12")
                    {
                        var db = new NorthwindContext();
                        var query = db.Products.OrderBy(p => p.ProductID);

                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.ProductID}) {item.ProductName}");
                        }
                        Console.WriteLine("Choose a record to delete from the Products table by selecting the ID");
                        int id = int.Parse(Console.ReadLine());
                        bool idExists = db.Products.Any(p => p.ProductID == id);
                        if (idExists)
                        {
                            Product product = db.Products.FirstOrDefault(p => p.ProductID == id);
                            logger.Info("Validation passed, product removed");
                            db.Products.Remove(product);
                            db.SaveChanges();
                        } else
                        {
                            logger.Info("Product ID does not exist");
                        }
                        Console.WriteLine();
                    }
                }
                while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }
    }
}
