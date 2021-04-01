using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using practica_fmi.Models;

[assembly: OwinStartupAttribute(typeof(practica_fmi.Startup))]
namespace practica_fmi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // create roles
            CreateApplicationRoles();
        }

        private void CreateApplicationRoles()
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            // Add application roles
            if (!roleManager.RoleExists("Admin"))
            {
                // Add admin role
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
                // Add admin user
                var admin = new ApplicationUser();
                admin.UserName = "admin@gmail.com";
                admin.Email = "admin@gmail.com";
                var adminCreated = UserManager.Create(admin, "parolaadmin");

                if (adminCreated.Succeeded)
                {
                    UserManager.AddToRole(admin.Id, "Admin");
                }
            }

            if (!roleManager.RoleExists("Profesor"))
            {
                var role = new IdentityRole();
                role.Name = "Profesor";
                roleManager.Create(role);

                // Create dummy account for testing
                var prof = new ApplicationUser();
                prof.UserName = "prof1@gmail.com";
                prof.Email = "prof1@gmail.com";
                var userCreated = UserManager.Create(prof, "parolaprof1");

                if (userCreated.Succeeded)
                {
                    UserManager.AddToRole(prof.Id, "Profesor");
                }
            }

            if (!roleManager.RoleExists("Student"))
            {
                var role = new IdentityRole();
                role.Name = "Student";
                roleManager.Create(role);

                // Create dummy account for testing
                var user = new ApplicationUser();
                user.UserName = "std1@gmail.com";
                user.Email = "std1@gmail.com";
                var userCreated = UserManager.Create(user, "parolastudent1");

                if (userCreated.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Student");
                }
            }
        }
    }
}
