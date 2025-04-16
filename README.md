## Library System

ASP.NET CORE MVC project to manage users, books and borrowings, Built with .Net 8.0 using 3-tier architecture.

## Setting up the project
1. Restore the application database located in database folder with Microsoft SQL Server Management Studio.
2. Open the app in **Visual Studio** or **Visual Studio Code**.
3. Run command **dotnet restore** to download and use all the packages needed to run the app.
4. You can now run the app and use the app interfaces.

## Information about the app
1. To manage users accounts login using **admin** account (Username:'Admin', Password:'Admin123').
2. There is one user which is the **admin**, To register new users use the register page.
3. There are 15 records of books data to retrieve and update.
4. There are no records of books borrowings, You can borrow books after logging into the app.
5. You can browse books and search for them with ISBN, Title, Author name or Availabilty status.
6. You can see all your books borrowings with borrowing date and return date for each.
7. You can return books from your borrowings details page.
8. You cannot borrow a book unless it is available in the browse books page.
9. Only **admin** can add new books in the add book page and update the info of the books by clicking on the ISBN in the browse page.
10. Only **admin** can Activate and Deactivate user accounts in the users page.
11. Only **admin** can see all users books borrowings with all details.
12. With the **admin** account you can search for a specifc user by searching his Id, Name or Activation status.
13. You can deactivate your account by going to the update page which has the label of your name and click on the delete my account link.
14. You can update your account information in the update page.
15. You cannot login again to your account if you deactivated it and will need the **admin** to reactivate it to login again.

## Information about the project infrastructure
1. **Data Access Layer** is built to read, write and update data to the database tables.
2. Used repository pattern in **Data Access Layer**, And created two repositories (User, Book).
3. **Business Logic Layer** is built to deliver functionality to **Presentation Layer** and have two services for managing users accounts, books and borrowings.
4. **Business Logic Layer** validates user inputs and requests, and checks results from **Data Access Layer**.
5. **Presentation Layer** created to deliver design, inputs and forms for users to interact with.
6. User Passwords will be hashed before storring it in the database using a helper class in **Data Access Layer**.
7. ADO.NET is used to connect to the database in **Data Access Layer**.
