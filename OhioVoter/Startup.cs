using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OhioVoter.Startup))]
namespace OhioVoter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
