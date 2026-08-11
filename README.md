# ODEN STORE






ODEN STORE is an ASP.NET Core MVC e-commerce application for browsing and managing sportswear products. It combines a public product catalog, authenticated shopping cart and review features, ASP.NET Core Identity, SQL Server persistence, and role-protected product and category administration.



## Overview



ODEN STORE demonstrates a complete MVC-based web application built with .NET 10, Entity Framework Core, ASP.NET Core Identity, Razor Views, and SQL Server.



The application provides a customer-facing sportswear catalog together with authentication, cart management, customer reviews, contact submissions, inventory validation, product image management, and admin-only catalog management.



The project applies practical ASP.NET Core concepts including MVC separation, Identity integration, authorization policies, validation, database constraints, rate limiting, dependency injection, asynchronous database access, secure authentication cookies, and EF Core migrations.



## Key Features



### Product Catalog



\- Public product catalog with responsive product cards

\- Category-based product browsing

\- Product pricing and discounted pricing

\- Product stock and availability information

\- Product images and descriptions

\- Database-backed catalog persistence



The user interface presents product categories as sportswear brands in parts of the storefront, while the underlying domain model uses a `Category` entity.



### Shopping Cart



\- Shopping cart for authenticated users

\- Add products to cart

\- Update product quantities

\- Remove individual cart items

\- Clear the cart

\- Cart subtotal calculations

\- Total cart quantity calculations

\- Stock-aware quantity validation

\- Prevention of quantities exceeding available inventory

\- Unique cart item per user/product combination



Cart additions use database transaction handling to help preserve data consistency.



### Product Management



Administrators can:



\- View products

\- View product details

\- Create products

\- Edit products

\- Delete products

\- Assign product categories

\- Manage product pricing

\- Configure discounted pricing

\- Manage inventory quantities

\- Upload and replace product images



Product image handling includes validation for supported image formats, file size, and file contents.



### Category Management



Administrators can:



\- View categories

\- Create categories

\- View category details

\- Edit categories

\- Delete unused categories

\- Configure category descriptions

\- Configure category icon classes



Categories referenced by existing products are protected from deletion.



### Customer Reviews



\- Latest customer reviews displayed on the storefront

\- Review submission for authenticated users

\- Server-side review validation

\- Review submission rate limiting

\- Authenticated account email retrieved server-side through ASP.NET Core Identity



Sensitive stored review information is not exposed through the public review cards.



### Contact Form



\- Public contact form

\- Server-side validation

\- SQL Server persistence

\- Submission timestamps

\- Contact submission rate limiting



### Authentication



The application uses ASP.NET Core Identity and supports:



\- User registration

\- Login

\- Logout

\- Unique email accounts

\- Email confirmation

\- Resending confirmation emails

\- Forgot-password workflow

\- Password reset

\- Remember-me login

\- Account management



Email delivery is implemented through `IEmailSender`, MailKit, and configurable SMTP settings.



### Authorization



\- Authentication required for shopping cart operations

\- Authentication required for review submission

\- Explicit `Admin` role

\- `AdminOnly` authorization policy

\- Product management restricted to administrators

\- Category management restricted to administrators



## User Roles



The application defines one explicit privileged role: `Admin`.



### Guest



Unauthenticated visitors can:



\- Browse the home page

\- Browse products

\- Browse products by category

\- View customer reviews

\- View informational pages

\- Submit the contact form

\- Register

\- Sign in



### Authenticated User



Authenticated users can additionally:



\- Access their shopping cart

\- Add products to the cart

\- Update cart quantities

\- Remove cart items

\- Clear the cart

\- Submit customer reviews

\- Access Identity account management



### Admin



Users assigned to the `Admin` role can additionally access:



\- Product management

\- Product creation and editing

\- Product deletion

\- Product image management

\- Inventory and pricing management

\- Category management



Admin functionality is protected through the `AdminOnly` authorization policy.



## Tech Stack



### Backend



\- .NET 10

\- C#

\- ASP.NET Core MVC

\- ASP.NET Core Razor Pages

\- ASP.NET Core Identity

\- Entity Framework Core 10.0.10

\- ASP.NET Core Rate Limiting

\- MailKit 4.17.0



### Frontend



\- Razor Views

\- HTML5

\- CSS3

\- JavaScript

\- Bootstrap 5.2.0

\- Bootstrap Icons

\- AOS animations

\- Swiper

\- Additional bundled frontend libraries



### Database



\- Microsoft SQL Server

\- Entity Framework Core SQL Server Provider

\- EF Core Code First migrations



### Authentication \& Security



\- ASP.NET Core Identity

\- Role-based authorization

\- Authorization policies

\- Email confirmation

\- Password reset tokens

\- Account lockout

\- Secure authentication cookies

\- Anti-forgery validation

\- Request rate limiting

\- HTTPS redirection

\- HSTS outside Development

\- Server-side validation



## Architecture



ODEN STORE follows the standard ASP.NET Core MVC architecture.



```text

Browser

&#x20;  |

&#x20;  v

Razor Views / Identity Razor Pages

&#x20;  |

&#x20;  v

MVC Controllers / Identity PageModels

&#x20;  |

&#x20;  v

Entity Framework Core + ASP.NET Core Identity

&#x20;  |

&#x20;  v

SQL Server

```



Identity email workflows use a dedicated email infrastructure component:



```text

ASP.NET Core Identity

&#x20;       |

&#x20;       v

&#x20;   IEmailSender

&#x20;       |

&#x20;       v

&#x20;SmtpEmailSender

&#x20;       |

&#x20;       v

&#x20;  MailKit / SMTP

```



The project uses ASP.NET Core Dependency Injection for components including:



\- `SouqcomContext`

\- ASP.NET Core Identity

\- `IEmailSender`

\- Authorization services

\- Logging



Controllers currently access the EF Core `DbContext` directly. The project does not introduce a Repository Pattern or general Service Layer solely for abstraction.



## Security Features



### ASP.NET Core Identity



Identity configuration includes:



\- Unique user emails

\- Confirmed account/email requirement

\- Minimum password length

\- Digit requirement

\- Lowercase requirement

\- Uppercase requirement

\- Account lockout after repeated failed authentication attempts



### Authentication Cookies



Authentication cookies are configured with security-related options including:



\- HTTP-only cookies

\- HTTPS-only cookie transmission

\- SameSite configuration

\- Expiration configuration

\- Sliding expiration



### Authorization



The application uses:



\- Role-based authorization

\- An `Admin` role

\- An `AdminOnly` policy

\- Authorization protection for shopping cart operations

\- Authorization protection for customer review submission



### Anti-Forgery Protection



Unsafe MVC HTTP operations are protected through ASP.NET Core anti-forgery validation.



### Rate Limiting



Fixed-window rate limiting is applied to sensitive operations including:



\- Authentication

\- Registration and email-related Identity operations

\- Password reset operations

\- Contact submissions

\- Customer review submissions



Requests exceeding configured limits receive HTTP `429 Too Many Requests`.



### Database Validation



Database and model validation protect important business rules including:



\- Positive product prices

\- Non-negative product inventory

\- Valid discounted pricing

\- Positive cart quantities

\- Unique cart entries per user/product pair



### Transport Security



The application enables:



\- HTTPS redirection

\- HSTS outside Development

\- Forwarded header handling



## Project Structure



```text

StoreOde/

├── StoreOde.sln

├── README.md

├── .gitignore

└── StoreOde/

&#x20;   ├── Areas/

&#x20;   │   └── Identity/

&#x20;   ├── Controllers/

&#x20;   ├── Infrastructure/

&#x20;   │   └── Email/

&#x20;   ├── Migrations/

&#x20;   ├── Models/

&#x20;   ├── ViewModels/

&#x20;   ├── Views/

&#x20;   ├── wwwroot/

&#x20;   ├── Program.cs

&#x20;   ├── appsettings.json

&#x20;   └── StoreOde.csproj

```



## Database



ODEN STORE uses Microsoft SQL Server with Entity Framework Core.



The main application entities include:



### Product



Stores product information such as:



\- Name

\- Description

\- Price

\- Discounted price

\- Category

\- Image path

\- Product type

\- Supplier information

\- Quantity

\- Entry date



### Category



Stores product grouping information including:



\- Name

\- Description

\- Icon class



A category can contain multiple products.



### Cart



Stores cart items belonging to authenticated ASP.NET Core Identity users.



Each cart entry associates:



\- A user

\- A product

\- A requested quantity



Database constraints prevent invalid quantities and duplicate user/product cart entries.



### Review



Stores customer reviews including review content and associated account information.



Only appropriate public review information is displayed in the storefront.



### ContactMessage



Stores contact form submissions including:



\- Name

\- Email

\- Subject

\- Message

\- Submission timestamp



ASP.NET Core Identity tables are stored in the same SQL Server database through the application's Identity-enabled `DbContext`.



Database schema evolution is managed through Entity Framework Core migrations.



## Getting Started



### Prerequisites



Install:



\- .NET 10 SDK

\- Microsoft SQL Server

\- Visual Studio, Visual Studio Code, or another compatible .NET IDE

\- Entity Framework Core CLI tools



If required, install the EF Core CLI:



```bash

dotnet tool install --global dotnet-ef

```



### 1. Clone the Repository



```bash

git clone <repository-url>

cd ODEN-STORE/StoreOde

```



### 2. Configure the Database



The application uses the following configuration key:



```text

ConnectionStrings:SouqcomContext

```



Sensitive database credentials should not be committed to source control.



For development, a connection string can be configured using .NET User Secrets:



```bash

dotnet user-secrets set "ConnectionStrings:SouqcomContext" "<your-sql-server-connection-string>"

```



### 3. Configure Email



SMTP configuration is required for Identity email confirmation and account recovery workflows.



Store SMTP credentials using User Secrets or environment variables instead of committing credentials to `appsettings.json`.



Example:



```bash

dotnet user-secrets set "Email:Host" "<smtp-host>"

dotnet user-secrets set "Email:Port" "587"

dotnet user-secrets set "Email:SecurityMode" "StartTls"

dotnet user-secrets set "Email:RequireAuthentication" "true"

dotnet user-secrets set "Email:UserName" "<smtp-username>"

dotnet user-secrets set "Email:Password" "<smtp-password>"

dotnet user-secrets set "Email:FromEmail" "<sender-email>"

dotnet user-secrets set "Email:FromName" "ODEN STORE"

```



Never commit real passwords, API keys, SMTP credentials, tokens, or other secrets to the repository.



### 4. Apply Database Migrations



From the project directory:



```bash

dotnet ef database update

```



### 5. Run the Application



```bash

dotnet run

```



Open the HTTPS development URL shown in the terminal.



## Configuration



Important application configuration keys include:



| Configuration Key | Purpose |

|---|---|

| `ConnectionStrings:SouqcomContext` | SQL Server database connection |

| `Email:Host` | SMTP server hostname |

| `Email:Port` | SMTP port |

| `Email:SecurityMode` | SMTP transport security |

| `Email:RequireAuthentication` | SMTP authentication setting |

| `Email:UserName` | SMTP username |

| `Email:Password` | SMTP password |

| `Email:FromEmail` | Sender email address |

| `Email:FromName` | Sender display name |

| `Email:TimeoutSeconds` | SMTP timeout |

| `AllowedHosts` | ASP.NET Core host filtering |



Sensitive values should be stored using:



\- .NET User Secrets during local development

\- Environment Variables or another secure secret store in deployed environments



## Screenshots



Project screenshots can be added to this section after repository setup.



### Home Page



<!-- Add Home Page screenshot here -->



### Product Catalog



<!-- Add Product Catalog screenshot here -->



### Shopping Cart



<!-- Add Shopping Cart screenshot here -->



### Customer Reviews



<!-- Add Customer Reviews screenshot here -->



### Authentication



<!-- Add Login / Registration screenshot here -->



### Product Management



<!-- Add Admin Product Management screenshot here -->



### Category Management



<!-- Add Admin Category Management screenshot here -->



## Recent UI Improvements



The current application includes several storefront and usability improvements:



\- Redesigned responsive navigation and header

\- Full-height home hero section

\- Responsive About page

\- Premium-quality informational section

\- Shopping features section

\- Customer trust section

\- Responsive product cards

\- Customer review cards

\- Authenticated review submission interface

\- Shopping cart interface improvements

\- Updated footer

\- Responsive Bootstrap layouts



## Future Improvements



Potential future extensions include:



\- Order and checkout workflow

\- Payment gateway integration

\- Order history and order management

\- Automated unit and integration tests

\- Admin management for reviews and contact submissions

\- CI/CD pipeline

\- Docker support

\- External or cloud storage for uploaded product images

\- Additional product search and filtering capabilities



These are planned improvements and are not part of the current implementation.



## Author



\*\*Ibrahim Elsaid\*\*



\- GitHub: `<GitHub profile URL>`

\- LinkedIn: `<LinkedIn profile URL>`

