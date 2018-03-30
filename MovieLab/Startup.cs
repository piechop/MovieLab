using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieLab.Startup))]
namespace MovieLab
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
