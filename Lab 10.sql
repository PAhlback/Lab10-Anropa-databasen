SELECT CategoryName, ProductName, UnitPrice FROM Products
JOIN Categories ON Products.CategoryID = Categories.CategoryID
ORDER BY CategoryName, ProductName;

SELECT Customers.CompanyName, COUNT(Orders.OrderID) as NrOfOrders FROM Orders
JOIN Customers ON Orders.CustomerID = Customers.CustomerID
GROUP BY CompanyName
ORDER BY NrOfOrders DESC;

SELECT LastName AS EmployeeLastName, Territories.TerritoryDescription AS Territory FROM Employees
JOIN EmployeeTerritories ON Employees.EmployeeID = EmployeeTerritories.EmployeeID
JOIN Territories ON EmployeeTerritories.TerritoryID = Territories.TerritoryID
ORDER BY LastName;

SELECT Customers.CompanyName, SUM(Orders.OrderID) as OrdersTotalValue FROM Orders
JOIN Customers ON Orders.CustomerID = Customers.CustomerID
GROUP BY CompanyName
ORDER BY OrdersTotalValue DESC;