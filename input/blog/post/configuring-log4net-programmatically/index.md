[Log4Net][1] is a widely used logging framework by Apache, who ported it from log4j, also made by Apache. It can be configured for a number of different things, such as where to output the logs and how to format the log text.

Using Log4Net is quite easy once the configuration has been completed, and here is a common way of doing just that (using a file appender as an example).

Create a file called log4net.config and add the following content:

    <log4net>
        <appender name="FileAppender" type="log4net.Appender.FileAppender">
            <file value="log.txt" />
            <encoding value="utf-8" />
            <layout type="log4net.Layout.PatternLayout">
                <conversionPattern value="%date %-5level %logger - %message%newline" />
            </layout>
        </appender>
        
        <root>
            <level value="DEBUG" />
            <appender-ref ref="FileAppender" />
        </root>
    </log4net>


Then add the following line in the startup process of your application:

    XmlConfigurator.Configure( "log4net.config" );

You may also place the xml configuration in the App.config and then instead of the above method call, add the following section to the configuration element:

    <configSections>
      <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
    </configSections>


Usually this is easy enough, however there might be cases where one can not, or simply do not want to, configure the logging in an external file, which is a perfectly understandable requirement. For these situations the following approach can be used.

    class Logger
    {
        public static void ConfigureFileAppender( string logFile )
        {
            var fileAppender = GetFileAppender( logFile );
            BasicConfigurator.Configure( fileAppender );
            ( ( Hierarchy ) LogManager.GetRepository() ).Root.Level = Level.Debug;
        }

        private static IAppender GetFileAppender( string logFile )
        {
            var layout = new PatternLayout( "%date %-5level %logger - %message%newline" );
            layout.ActivateOptions(); // According to the docs this must be called as soon as any properties have been changed.

            var appender = new FileAppender
                {
                    File = logFile,
                    Encoding = Encoding.UTF8,
                    Threshold = Level.Debug,
                    Layout = layout
                };

            appender.ActivateOptions();

            return appender;
        }
    }

Simply add the above class, and then use it like this:

    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger( typeof ( Program ) );

        private static void Main( string[] args )
        {
            Logger.ConfigureFileAppender( "log.txt" ); // This only has to be called once.
            Log.Info( "This is not a log line." );
        }
    }

That's it, it's easy when you know it. I hope someone finds it useful.

  [1]: http://logging.apache.org/log4net/