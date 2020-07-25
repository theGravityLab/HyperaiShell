using Hyperai;

namespace HyperaiShell.App.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IHyperaiApplicationBuilder UseBots(this IHyperaiApplicationBuilder app)
        {
            app.Use<BotMiddleware>();
            return app;
        }

        public static IHyperaiApplicationBuilder UseTranslator(this IHyperaiApplicationBuilder app)
        {
            app.Use<TranslatorMiddleware>();
            return app;
        }

        public static IHyperaiApplicationBuilder UseLogging(this IHyperaiApplicationBuilder app)
        {
            app.Use<LoggingMiddleware>();
            return app;
        }

        public static IHyperaiApplicationBuilder UseBlacklist(this IHyperaiApplicationBuilder app)
        {
            app.Use<BlockMiddleware>();
            return app;
        }
    }
}