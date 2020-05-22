using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DailyReportSystem.Startup))]
namespace DailyReportSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
