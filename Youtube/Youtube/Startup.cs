using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Youtube.Models;

[assembly: OwinStartupAttribute(typeof(Youtube.Startup))]
namespace Youtube
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
        }

        //AUTHORIZATION CONFIGURATION DOUBLE PENETRATION
        private void CreateRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Regular"))
            {
                var role = new IdentityRole();
                role.Name = "Regular";
                roleManager.Create(role);
            }
        }
    }
}
