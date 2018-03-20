using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FabrikProject.Startup))]
namespace FabrikProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
