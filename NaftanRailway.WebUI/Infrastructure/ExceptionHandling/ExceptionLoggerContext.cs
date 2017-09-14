namespace NaftanRailway.WebUI.Infrastructure.ExceptionHandling {
    public class ExceptionLoggerContext {
        public ExceptionContext ExceptionContext { get; set; }
        public bool CanBeHandled { get; set; }
    }
}