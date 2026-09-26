using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class HomeController
{
    private VisualElement root;

    // Tarefas Rápidas
    private TextField inputHomeQuickTask;
    private Button btnAddHomeQuickTask;

    // CRUD Mesa de Trabalho
    private ScrollView taskList;
    private Button btnCreateTask;

    // Modal
    private VisualElement modalWorkDesk;
    private Label modalTitle;
    private TextField inputWdTitle;
    private TextField inputWdDescription;
    private TextField inputWdDate;
    private DropdownField dropdownWdPriority;
    private Button btnSaveWd;
    private Button btnCancelWd;

    // Dados
    private WorkDeskDatabase myWorkDesks = new WorkDeskDatabase();
    private const string WD_SAVE_KEY = "WorkDeskSaveData";

    private string idEmEdicao = "";

    public HomeController(VisualElement rootVisualElement)
    {
        root = rootVisualElement;
        ConfigurarTela();
        CarregarMesasDoDisco();
    }

    private void ConfigurarTela()
    {
        inputHomeQuickTask = root.Q<TextField>("quick-task-input");
        btnAddHomeQuickTask = root.Q<Button>("btn-add-home-task");
        if (btnAddHomeQuickTask != null) btnAddHomeQuickTask.clicked += InserirTarefaRapida;

        taskList = root.Q<ScrollView>("task-list");
        btnCreateTask = root.Q<Button>("btn-create-task");
        if (btnCreateTask != null) btnCreateTask.clicked += () => AbrirModal(false);

        modalWorkDesk = root.Q<VisualElement>("modal-workdesk");
        modalTitle = root.Q<Label>("modal-title");
        inputWdTitle = root.Q<TextField>("input-wd-title");
        inputWdDescription = root.Q<TextField>("input-wd-description");
        inputWdDate = root.Q<TextField>("input-wd-date");
        dropdownWdPriority = root.Q<DropdownField>("dropdown-wd-priority");

        btnSaveWd = root.Q<Button>("btn-save-wd");
        btnCancelWd = root.Q<Button>("btn-cancel-wd");

        dropdownWdPriority.value = "Média";

        if (btnSaveWd != null) btnSaveWd.clicked += SalvarMesaDeTrabalho;
        if (btnCancelWd != null) btnCancelWd.clicked += FecharModal;
    }

    private void InserirTarefaRapida()
    {
        string texto = inputHomeQuickTask.value;
        QuickTasksController.AdicionarTarefaDiretoNoSave(texto);
        inputHomeQuickTask.value = "";
    }

    private void CarregarMesasDoDisco()
    {
        if (PlayerPrefs.HasKey(WD_SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(WD_SAVE_KEY);
            myWorkDesks = JsonUtility.FromJson<WorkDeskDatabase>(json);
        }

        if (myWorkDesks == null) myWorkDesks = new WorkDeskDatabase();

        AtualizarUI();
    }

    private void AtualizarUI()
    {
        taskList.Clear();

        foreach (var item in myWorkDesks.items)
        {
            VisualElement cardItem = new VisualElement();
            cardItem.style.backgroundColor = new StyleColor(new Color(0.9f, 0.9f, 0.9f));
            cardItem.style.paddingBottom = 15;
            cardItem.style.paddingTop = 15;
            cardItem.style.paddingLeft = 15;
            cardItem.style.paddingRight = 15;
            cardItem.style.marginBottom = 10;

            cardItem.style.borderTopLeftRadius = 10;
            cardItem.style.borderTopRightRadius = 10;
            cardItem.style.borderBottomLeftRadius = 10;
            cardItem.style.borderBottomRightRadius = 10;

            cardItem.style.color = Color.black;

            cardItem.Add(new Label { text = $"Título: {item.title}", style = { unityFontStyleAndWeight = FontStyle.Bold, fontSize = 28, marginBottom = 5 } });

            if (!string.IsNullOrEmpty(item.description))
            {
                cardItem.Add(new Label { text = $"Descrição: {item.description}", style = { fontSize = 24, marginBottom = 5 } });
            }

            cardItem.Add(new Label { text = $"Entrega: {item.dueDate}", style = { fontSize = 24 } });
            cardItem.Add(new Label { text = $"Prioridade: {item.priority}", style = { fontSize = 24, marginBottom = 10 } });

            // Linha de botões organizada
            VisualElement buttonRow = new VisualElement();
            buttonRow.style.flexDirection = FlexDirection.Row;
            buttonRow.style.justifyContent = Justify.SpaceBetween;
            buttonRow.style.marginTop = 10;

            // BOTÃO VERDE (FINALIZAR)
            Button btnFinish = new Button { text = "✔ FINALIZAR" };
            btnFinish.style.backgroundColor = new StyleColor(new Color(0.18f, 0.49f, 0.20f)); // Verde Escuro
            btnFinish.style.color = Color.white;
            btnFinish.style.unityFontStyleAndWeight = FontStyle.Bold;
            btnFinish.style.height = 60;
            btnFinish.style.flexGrow = 1;
            btnFinish.style.marginRight = 10;

            // Correção das bordas do botão Finalizar
            btnFinish.style.borderTopLeftRadius = 8;
            btnFinish.style.borderTopRightRadius = 8;
            btnFinish.style.borderBottomLeftRadius = 8;
            btnFinish.style.borderBottomRightRadius = 8;

            btnFinish.clicked += () => DeletarMesa(item.id);

            // BOTÃO EDITAR
            Button btnEdit = new Button { text = "Editar" };
            btnEdit.style.height = 60;
            btnEdit.style.marginRight = 5;

            // Correção das bordas do botão Editar
            btnEdit.style.borderTopLeftRadius = 8;
            btnEdit.style.borderTopRightRadius = 8;
            btnEdit.style.borderBottomLeftRadius = 8;
            btnEdit.style.borderBottomRightRadius = 8;

            btnEdit.clicked += () => AbrirModal(true, item.id);

            // BOTÃO EXCLUIR
            Button btnDelete = new Button { text = "Excluir" };
            btnDelete.style.backgroundColor = new StyleColor(Color.red);
            btnDelete.style.color = Color.white;
            btnDelete.style.height = 60;

            // Correção das bordas do botão Excluir
            btnDelete.style.borderTopLeftRadius = 8;
            btnDelete.style.borderTopRightRadius = 8;
            btnDelete.style.borderBottomLeftRadius = 8;
            btnDelete.style.borderBottomRightRadius = 8;

            btnDelete.clicked += () => DeletarMesa(item.id);

            buttonRow.Add(btnFinish);
            buttonRow.Add(btnEdit);
            buttonRow.Add(btnDelete);

            cardItem.Add(buttonRow);
            taskList.Add(cardItem);
        }
    }

    private void AbrirModal(bool isEdit, string id = "")
    {
        if (isEdit)
        {
            modalTitle.text = "EDITAR MESA";
            idEmEdicao = id;

            WorkDeskItem itemToEdit = myWorkDesks.items.FirstOrDefault(x => x.id == id);
            if (itemToEdit != null)
            {
                inputWdTitle.value = itemToEdit.title;
                inputWdDescription.value = itemToEdit.description;
                inputWdDate.value = itemToEdit.dueDate;
                dropdownWdPriority.value = itemToEdit.priority;
            }
        }
        else
        {
            modalTitle.text = "NOVA MESA DE TRABALHO";
            idEmEdicao = "";

            inputWdTitle.value = "";
            inputWdDescription.value = "";
            inputWdDate.value = "";
            dropdownWdPriority.value = "Média";
        }

        modalWorkDesk.style.display = DisplayStyle.Flex;
    }

    public void FecharModal()
    {
        modalWorkDesk.style.display = DisplayStyle.None;
    }

    // Permite saber se o painel está aberto na tela
    public bool IsModalAberto()
    {
        return modalWorkDesk != null && modalWorkDesk.style.display == DisplayStyle.Flex;
    }
    private void SalvarMesaDeTrabalho()
    {
        if (string.IsNullOrWhiteSpace(inputWdTitle.value)) return;

        if (string.IsNullOrEmpty(idEmEdicao))
        {
            WorkDeskItem newItem = new WorkDeskItem
            {
                id = Guid.NewGuid().ToString(),
                title = inputWdTitle.value,
                description = inputWdDescription.value,
                dueDate = inputWdDate.value,
                priority = dropdownWdPriority.value
            };
            myWorkDesks.items.Add(newItem);
        }
        else
        {
            WorkDeskItem itemToEdit = myWorkDesks.items.FirstOrDefault(x => x.id == idEmEdicao);
            if (itemToEdit != null)
            {
                itemToEdit.title = inputWdTitle.value;
                itemToEdit.description = inputWdDescription.value;
                itemToEdit.dueDate = inputWdDate.value;
                itemToEdit.priority = dropdownWdPriority.value;
            }
        }

        SalvarNoDisco();
        FecharModal();
        AtualizarUI();
    }

    private void DeletarMesa(string id)
    {
        WorkDeskItem itemToRemove = myWorkDesks.items.FirstOrDefault(x => x.id == id);
        if (itemToRemove != null)
        {
            myWorkDesks.items.Remove(itemToRemove);
            SalvarNoDisco();
            AtualizarUI();
        }
    }

    private void SalvarNoDisco()
    {
        string json = JsonUtility.ToJson(myWorkDesks);
        PlayerPrefs.SetString(WD_SAVE_KEY, json);
        PlayerPrefs.Save();
    }
}