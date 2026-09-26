using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

// 1. Classe para armazenar a lista de tarefas de forma que a Unity consiga transformar em JSON
[System.Serializable]
public class QuickTaskList
{
    public List<string> tasks = new List<string>();
}

public class QuickTasksController
{
    private VisualElement root;
    private TextField inputNewQuickTask;
    private Button btnAddQuickTask;
    private VisualElement taskListContainer;

    // 2. Variáveis para gerenciar os dados salvos
    private QuickTaskList myTasks = new QuickTaskList();
    private const string SAVE_KEY = "QuickTasksSaveData";

    public QuickTasksController(VisualElement rootVisualElement)
    {
        root = rootVisualElement;
        ConfigurarTela();
        CarregarTarefas(); // 3. Carrega as tarefas salvas assim que a tela abre
    }

    private void ConfigurarTela()
    {
        inputNewQuickTask = root.Q<TextField>("input-new-quick-task");
        btnAddQuickTask = root.Q<Button>("btn-add-quick-task");
        taskListContainer = root.Q<VisualElement>("task-list-container");

        if (btnAddQuickTask != null)
        {
            btnAddQuickTask.clicked += CriarNovaTarefaRapida;
        }
    }

    private void CriarNovaTarefaRapida()
    {
        string texto = inputNewQuickTask.value;
        if (string.IsNullOrWhiteSpace(texto)) return;

        // Adiciona na UI
        AdicionarTarefaNaUI(texto);

        // Adiciona na lista de dados e salva
        myTasks.tasks.Add(texto);
        SalvarTarefas();

        inputNewQuickTask.value = "";
    }

    // 4. Separei a lógica visual em um método próprio para poder reaproveitá-la ao carregar o save
    private void AdicionarTarefaNaUI(string texto)
    {
        VisualElement taskItem = new VisualElement();
        taskItem.AddToClassList("todo-item");

        Label taskLabel = new Label { text = texto };
        taskLabel.AddToClassList("todo-label");

        Button btnDelete = new Button { text = "X" };
        btnDelete.AddToClassList("btn-delete");

        btnDelete.clicked += () =>
        {
            // Remove da UI
            taskListContainer.Remove(taskItem);

            // Remove da lista de dados e atualiza o save
            myTasks.tasks.Remove(texto);
            SalvarTarefas();
        };

        taskItem.Add(taskLabel);
        taskItem.Add(btnDelete);
        taskListContainer.Add(taskItem);
    }

    // 5. Método que converte a lista para JSON e salva no celular
    private void SalvarTarefas()
    {
        string json = JsonUtility.ToJson(myTasks);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save(); // Força o salvamento imediato no disco do aparelho
    }

    // 6. Método que lê o JSON do celular e recria a lista na tela
    private void CarregarTarefas()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            myTasks = JsonUtility.FromJson<QuickTaskList>(json);

            // Se os dados vierem nulos (por algum erro de leitura), inicializa a lista limpa
            if (myTasks == null) myTasks = new QuickTaskList();

            // Para cada tarefa salva, recria os botões na UI
            foreach (string task in myTasks.tasks)
            {
                AdicionarTarefaNaUI(task);
            }
        }
    }



public static void AdicionarTarefaDiretoNoSave(string texto)
{
    if (string.IsNullOrWhiteSpace(texto)) return;

    QuickTaskList lista = new QuickTaskList();
        
    // 1. Lê as tarefas que já estão salvas no celular
    if (PlayerPrefs.HasKey(SAVE_KEY))
    {
        string json = PlayerPrefs.GetString(SAVE_KEY);
        lista = JsonUtility.FromJson<QuickTaskList>(json);
        if (lista == null) lista = new QuickTaskList();
    }

    // 2. Adiciona a nova tarefa que veio da Home
    lista.tasks.Add(texto);

    // 3. Salva tudo de volta no celular
    PlayerPrefs.SetString(SAVE_KEY, JsonUtility.ToJson(lista));
    PlayerPrefs.Save();
}
}

