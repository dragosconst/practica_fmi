using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(practica_fmi.Startup))]
namespace practica_fmi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
