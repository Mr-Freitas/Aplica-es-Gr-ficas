using UnityEngine;
using UnityEngine.UIElements;
using Unity.Services.Authentication;
using UnityEngine.InputSystem; // NOVO: Biblioteca do novo sistema de inputs

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    [Header("Telas (Arraste os arquivos UXML aqui)")]
    [SerializeField] private VisualTreeAsset loginUXML;
    [SerializeField] private VisualTreeAsset homeUXML;
    [SerializeField] private VisualTreeAsset quickTasksUXML;
    [SerializeField] private VisualTreeAsset calendarUXML;

    private LoginController loginController;
    private QuickTasksController quickTasksController;
    private HomeController homeController;
    private CalendarController calendarController;

    public enum TelaAtual { Login, Home, TarefasRapidas, Calendario }
    private TelaAtual telaAtual = TelaAtual.Login;

    private void Awake()
    {
        CarregarTelaLogin();
    }

    // ==========================================
    // SISTEMA DO BOTÃO VOLTAR DO CELULAR (NOVO INPUT SYSTEM)
    // ==========================================
    private void Update()
    {
        // Verifica se o teclado/sistema de input mobile existe e se a tecla de voltar (Escape) foi pressionada
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TratarBotaoVoltar();
        }
    }

    private void TratarBotaoVoltar()
    {
        switch (telaAtual)
        {
            case TelaAtual.Login:
                Application.Quit(); // Fecha o app
                break;

            case TelaAtual.Home:
                if (homeController != null && homeController.IsModalAberto())
                {
                    homeController.FecharModal();
                }
                else
                {
                    Application.Quit();
                }
                break;

            case TelaAtual.TarefasRapidas:
            case TelaAtual.Calendario:
                CarregarTelaHome();
                break;
        }
    }

    // ==========================================
    // MÉTODOS DE TROCA DE TELA
    // ==========================================
    private void CarregarTelaLogin()
    {
        telaAtual = TelaAtual.Login;
        uiDocument.visualTreeAsset = loginUXML;
        var root = uiDocument.rootVisualElement;
        loginController = new LoginController(root, CarregarTelaHome);
    }

    private void CarregarTelaHome()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            telaAtual = TelaAtual.Home;
            uiDocument.visualTreeAsset = homeUXML;
            var root = uiDocument.rootVisualElement;

            ConfigurarNavegacao(root);
            homeController = new HomeController(root);
        }
        else CarregarTelaLogin();
    }

    private void CarregarTelaTarefasRapidas()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            telaAtual = TelaAtual.TarefasRapidas;
            uiDocument.visualTreeAsset = quickTasksUXML;
            var root = uiDocument.rootVisualElement;

            ConfigurarNavegacao(root);
            quickTasksController = new QuickTasksController(root);
        }
    }

    private void CarregarTelaCalendario()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            telaAtual = TelaAtual.Calendario;
            uiDocument.visualTreeAsset = calendarUXML;
            var root = uiDocument.rootVisualElement;

            ConfigurarNavegacao(root);
            calendarController = new CalendarController(root);
        }
    }

    // ==========================================
    // NAVEGAÇÃO COMPARTILHADA
    // ==========================================
    private void ConfigurarNavegacao(VisualElement root)
    {
        Button navHome = root.Q<Button>("nav-home");
        Button navQuickTasks = root.Q<Button>("nav-quick-tasks");
        Button navCalendar = root.Q<Button>("nav-calendar");

        if (navHome != null) navHome.clicked += CarregarTelaHome;
        if (navQuickTasks != null) navQuickTasks.clicked += CarregarTelaTarefasRapidas;
        if (navCalendar != null) navCalendar.clicked += CarregarTelaCalendario;
    }
}