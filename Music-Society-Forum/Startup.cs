using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Music_Society_Forum.Startup))]
namespace Music_Society_Forum
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
