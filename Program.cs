using Lab10_Anropa_databasen.Models;
using Lab10AnropaDatabasen.Data;
using System;
using System.Data.SqlTypes;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace Lab10_Anropa_databasen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new NorthContext())
            {
                MainMenu(context);
            }
        }

        static void MainMenu(NorthContext context)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome!");
                Console.WriteLine();
                Console.WriteLine("1. Show/select customer");
                Console.WriteLine("2. Create new customer");
                Console.WriteLine("3. Exit");

                // Experimenting with checking that the input string only contains numbers.
                string choice = Console.ReadLine();
                bool containsDigits = IsOnlyInt(choice);
                int checkedChoice = 0;

                while (!containsDigits)
                {
                    Console.Write("Invalid input. Try again: ");
                    choice = Console.ReadLine();
                    containsDigits = IsOnlyInt(choice);
                }

                checkedChoice = Convert.ToInt32(choice);

                if(checkedChoice == 1) 
                {
                    ShowCustomers(context);
                }
                else if(checkedChoice == 2)
                {
                    CreateCustomer(context);
                }
                else if(checkedChoice == 3)
                {
                    Console.WriteLine("Good bye!");
                    Environment.Exit(1000);
                }
                else
                {
                    Console.WriteLine("Invalid choice. Returning to main menu...");
                    Thread.Sleep(1000);
                    Console.Clear();
                }
            }
        }

        static void ShowCustomers(NorthContext context)
        {
            Console.Clear();
            Console.WriteLine("Order customers by ascending or descending (default = ascending)?");
            Console.Write("A/D: ");
            string ascOrDesc = Console.ReadLine().ToUpper();
            Console.WriteLine("Fetching customer data. Please wait...");
            Console.WriteLine();

            // Get the customers from the database
            var customerList = context.Customers
                .Select(c => new
                {
                    c.CompanyName,
                    c.Country,
                    c.Region,
                    c.Phone,
                    NrOfOrders = c.Orders.Count(),
                    c.ContactName,
                    c.ContactTitle,
                    c.Address,
                    c.City,
                    c.PostalCode,
                    c.Fax,
                    c.CustomerId
                })
                .OrderBy(c => c.CompanyName)
                .ToList();
            if (ascOrDesc == "D")
            {
                customerList
                .OrderByDescending(c => c.CompanyName)
                .ToList();
            } 
            
            Console.Clear();

            // Print the customers, with a number for selecting customer
            int i = 1;
            foreach (var c in customerList)
            {
                Console.WriteLine($"{i}.");
                Console.WriteLine($"Company name: {c.CompanyName}");
                if (c.Country != null)  Console.WriteLine($"Country: {c.Country}");
                if (c.Region != null) Console.WriteLine($"Region: {c.Region}");
                if (c.Phone != null) Console.WriteLine($"Phone: {c.Phone}");
                Console.WriteLine($"Number of orders: {c.NrOfOrders}");
                Console.WriteLine("--------------------------------------------");
                i++;
            }

            // Choose whether to select and view information about a single customer or not.
            Console.WriteLine("Select customer?");
            Console.Write("y/n: ");
            char yesNo = char.Parse(Console.ReadLine());

            if (yesNo == 'n')
            {
                Console.WriteLine("Returning to main menu.");
                Thread.Sleep(1000);
                Console.Clear();
                return;
            }
            else if (yesNo == 'y')
            {
                // Select specific customer via numbering
                Console.Write("Enter customer number: ");
                int cNumber = int.Parse(Console.ReadLine()) - 1;
                Console.Clear();

                Console.WriteLine($"{customerList[cNumber].CompanyName}");
                if (customerList[cNumber].ContactName != null)
                {
                    Console.WriteLine($"Contact: {customerList[cNumber].ContactTitle} {customerList[cNumber].ContactName}");
                }
                else
                {
                    Console.WriteLine("Contact: not found");
                }
                if (customerList[cNumber].Address != null)
                {
                    Console.WriteLine($"Address: {customerList[cNumber].Address}, {customerList[cNumber].City}, {customerList[cNumber].Region}, {customerList[cNumber].PostalCode}, {customerList[cNumber].Country}");
                }
                else
                {
                    Console.WriteLine("Address: not found");
                }
                if (customerList[cNumber].Phone != null)
                {
                    Console.WriteLine($"Phone: {customerList[cNumber].Phone}");
                }
                else
                {
                    Console.WriteLine("Phone: not found");
                }
                if (customerList[cNumber].Fax != null)
                {
                    Console.WriteLine($"Fax: {customerList[cNumber].Fax}");
                }
                else
                {
                    Console.WriteLine("Fax: not found");
                }

                var customerOrders = context.Orders
                    .Where(o => o.CustomerId == customerList[cNumber].CustomerId)
                    .Where(o => o.OrderId == o.OrderDetails.Single().OrderId)
                    .Select(o => new
                    {
                        o.OrderId,
                        o.ShipName,
                        o.OrderDate,
                        o.OrderDetails.Single().Product.ProductName
                    })
                    .OrderBy(o => o.OrderDate)
                    .ToList();

                Console.WriteLine("Customers orders:");
                foreach (var order in customerOrders)
                {
                    string date = order.OrderDate.ToString().Substring(0, 10);
                    Console.WriteLine($"{order.OrderId} - ordered product {order.ProductName} on {date}");
                }
                Console.WriteLine("Press enter to return to main menu");
                Console.ReadLine();
            }

        }

        static void CreateCustomer(NorthContext context)
        {
            Console.Clear();
            Console.WriteLine("Welcome to customer registration");
            string[] inputArray = new string[9];

            Console.Write("Enter name: ");
            inputArray[0] = Console.ReadLine();

            Console.Write("Enter name of contact person: ");
            inputArray[1] = Console.ReadLine();

            Console.Write("Enter title of contact person: ");
            inputArray[2] = Console.ReadLine();

            Console.Write("Enter address: ");
            inputArray[3] = Console.ReadLine();

            Console.Write("Enter city: ");
            inputArray[4] = Console.ReadLine();

            Console.Write("Enter postal code: ");
            inputArray[5] = Console.ReadLine();

            Console.Write("Enter country: ");
            inputArray[6] = Console.ReadLine();

            Console.Write("Enter phone: ");
            inputArray[7] = Console.ReadLine();

            Console.Write("Enter fax: ");
            inputArray[8] = Console.ReadLine();

            for (int i = 0; i < 9; i++)
            {
                inputArray[i] = (inputArray[i] == "") ? inputArray[i] : null;
            }

            char[] customerIdArray = new char[5];

            if (inputArray[0].Length >= 5)
            {
                customerIdArray = inputArray[0].Substring(0, 5).ToUpper().ToCharArray();
            }
            else if (inputArray[0].Length < 5)
            {
                char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
                Random random = new Random();
                for (int i = 0; i < 5; i++)
                {
                    int r = random.Next(letters.Length);
                    customerIdArray[i] = letters[r];
                }
            }
            // Converting the char array to string
            string customerId = new string(customerIdArray);
            Console.WriteLine("Registering customer...");

            Customer customer = new Customer()
            {
                CustomerId = customerId,
                CompanyName = inputArray[0],
                ContactName = inputArray[1],
                ContactTitle = inputArray[2],
                Address = inputArray[3],
                City = inputArray[4],
                PostalCode = inputArray[5],
                Country = inputArray[6],
                Phone = inputArray[7],
                Fax = inputArray[8]
            };
            context.Customers.Add(customer);
            context.SaveChanges();

            Console.WriteLine("Done!");
            Console.WriteLine("Press enter to return to main menu");
            Console.ReadLine();
        }

        static bool IsOnlyInt(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }
    }
}