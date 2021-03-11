using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Passion_231.Startup))]
namespace Passion_231
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
