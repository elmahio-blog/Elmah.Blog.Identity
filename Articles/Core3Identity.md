<p style="align:center;">
    <img src="https://asociacionaepi.es/wp-content/uploads/2019/01/banner-core.png" alt="ASP.Net Core 3" caption="ASP.Net Core 3">
</p>

# ASP.Net Core 3 Identity
The aim of this post is to teach you how to teach you how to implement Identity Authentication & Authorization with ASP.Net Core 3. We will begin with a cookie-cutter Microsoft starter project and then modify that to use Kendo in a future lesson. 

## First Things First
> Because Core 3 is only officially a few days old as of the moment of this article, I'll walk you through the basics of getting it working on your computer. 
> - [ ] You'll want to download the x64 and/or x86 SDK from [here](https://dotnet.microsoft.com/download/dotnet-core/3.0). 
> - [ ] Don't forget to update to the latest version of Visual Studio 2019 with "Visual Studio Installer" or you won't see ASP.Net Core 3. 

- [ ] Microsoft does a pretty good job of setting the stage for you and basic authentication works out of the box. We'll just use localdb for this project that is already setup in your appsettings.json file. The only thing you need to do in order to get **authentication** working is simply go to the Package Manager Console and type: `update-database`. 
- [ ] Hit F5 and wait impatiently 🥱 for your project to load. Click on Register to create a local account. Here, you will get a message saying "This app does not currently have a real email sender registered..." Instead of simply clicking the link provided, let's fix the issue instead.
- [ ] Because we didn't click that link, we have effectively orphaned that email address so it "cannot" be accessed ever again (without manually confirming it in the database). So, let's start with a fresh database. Click on Package Manager Console and enter the following three commands in sequence:

`Remove-Migration -Force

Add-Migration CreateIdentitySchema

Update-Database`

Now, you will have a fresh copy of your database to start from.

## Sending Emails
The following code will send out the intrensic emails (registration confirmation, forgotten password, et cetera) from Identity, but you will most likely use the same methods for your own code. If you don't already have a SMTP server that you want to use, I'd recommend the older Windows App version of [Smtp4Dev](https://archive.codeplex.com/?p=smtp4dev). There is a much newer version, but those new to .net core or that don't want to run it in a docker container should just download the old version. 

- The first thing that we are going to want to do is define the email service. I do that by creating a directory called Services from the root and then create a new class called EmailSender.cs inside of that directory. The code for that service should look as follows:

```csharp
namespace Elmah.Blog.IdentityKendo.Services
{
    using System.Net;
    using System.Net.Mail;
    
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity.UI.Services;

    /// <summary>
    /// This class is responsible for sending emails.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// The SMTP server.
        /// </summary>
        private readonly string host;

        /// <summary>
        /// The port that the SMTP server listens on.
        /// </summary>
        private readonly int port;

        /// <summary>
        /// The flag either enables or disables SSL.
        /// </summary>
        private readonly bool enableSsl;

        /// <summary>
        /// The user name for the SMTP server. Use "" if not required.
        /// </summary>
        private readonly string userName;

        /// <summary>
        /// The password for the SMTP server. Use "" if not required.
        /// </summary>
        private readonly string password;

        /// <summary>
        /// The email address that the email should come from.
        /// </summary>
        private readonly string senderEmail;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSender"/> class.
        /// </summary>
        public EmailSender(string host, int port, bool enableSSL, string userName, string password, string senderEmail)
        {
            this.host = host;
            this.port = port;
            this.enableSsl = enableSSL;
            this.userName = userName;
            this.password = password;
            this.senderEmail = senderEmail;
        }

        /// <summary>
        /// This method sends emails asynchronously.
        /// </summary>
        /// <param name="email">
        /// The email address the message will be sent to.
        /// </param>
        /// <param name="subject">
        /// The subject of the message.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(this.host, this.port)
                             {
                                 Credentials = new NetworkCredential(this.userName, this.password), EnableSsl = this.enableSsl
                             };

            return client.SendMailAsync(new MailMessage(this.senderEmail, email, subject, message) { IsBodyHtml = true });
        }
    }
}
```

- Now that we have done that, you'll want to define some properties in your ~/appsettings.json file, which should look like the following:

```json
"EmailSender": {
        "Host": "localhost",
        "Port": 25,
        "EnableSSL": false,
        "UserName": "", 
        "Password": "",
        "SenderEmail": "info@coderpro.net" 
    }
```

- Lastly, we'll want to configure the services in our ~/Startup.cs class:

```csharp
        /// <summary>
        /// The configure services routine.
        /// </summary>
        /// <param name="services">
        /// A collection of services used in the application.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    this.Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddTransient<IEmailSender, Services.EmailSender>(
                implementation => new Services.EmailSender(
                    this.Configuration["EmailSender:Host"],
                    this.Configuration.GetValue<int>("EmailSender:Port"),
                    this.Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    this.Configuration["EmailSender:UserName"],
                    this.Configuration["EmailSender:Password"],
                    this.Configuration["EmailSender:SenderEmail"]));
            services.AddRazorPages();
        }
```

## Setting up oAuth & Facebook Authentication
This post presumes you know how to go about creating an app on the Facebook Developer's (Apps Page) https://developers.facebook.com/apps/. So, we'll get straight to the coding. 🙂 

The first thing you're going to want to do is create user secrets to keep this data secure. There are two ways of accomplishing this: Through the Developer's command prompt with:

```
dotnet user-secrets set Authentication:Facebook:AppId <app-id>
dotnet user-secrets set Authentication:Facebook:AppSecret <app-secret>
```

Next, you'll want to open 



## Summary
In this article we covered how to create a barebones site that properly implements ASP.Net C3 Identity Services and how to use oAuth to allow users to login from external providers. 

The full source for this article can be found on [GitHub](https://github.com/elmahio-blog/Elmah.Blog.Identity).

## Coming Up Next Time
In the next post, I'll be covering how you go about giving this app a mobile first look and feel that mimics a native application with Terlik Kendo-UI. Until then: Happy Coding!