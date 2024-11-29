using System.Windows.Forms;

namespace SystemWrapper.Forms;

public static class ApplicationWrapper
{
    private static IApplication _application = new ApplicationImplementation();

    public static void SetApplication(IApplication application)
    {
        _application = application;
    }

    public static IApplication GetApplication()
    {
        return _application;
    }

    public static void Initialize()
    {
        _application.Initialize();
    }

    public static void Run(Form mainForm)
    {
        _application.Run(mainForm);
    }
}

public interface IApplication
{
    void Initialize();
    void Run(Form mainForm);
}

public class ApplicationImplementation : IApplication
{
    public void Initialize()
    {
        ApplicationConfiguration.Initialize();
    }

    public void Run(Form mainForm)
    {
        Application.Run(mainForm);
    }
}
