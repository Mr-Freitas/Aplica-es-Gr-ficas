using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;

public class CalendarController
{
    private VisualElement root;
    
    // Elementos da UI
    private Label labelMonthYear;
    private Button btnPrevMonth;
    private Button btnNextMonth;
    private VisualElement daysContainer;

    // Estado do Calendário
    private DateTime currentDate;

    // Dados das Tarefas
    private WorkDeskDatabase myWorkDesks = new WorkDeskDatabase();
    private const string WD_SAVE_KEY = "WorkDeskSaveData";

    // Nomes dos meses em português
    private string[] nomeMeses = { "", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

    public CalendarController(VisualElement rootVisualElement)
    {
        root = rootVisualElement;
        currentDate = DateTime.Now; // Começa no mês atual
        
        ConfigurarTela();
        CarregarTarefasDoDisco();
        RenderizarCalendario();
    }

    private void ConfigurarTela()
    {
        labelMonthYear = root.Q<Label>("label-month-year");
        btnPrevMonth = root.Q<Button>("btn-prev-month");
        btnNextMonth = root.Q<Button>("btn-next-month");
        daysContainer = root.Q<VisualElement>("calendar-days-container");

        // Eventos dos botões de navegação do mês
        if (btnPrevMonth != null)
        {
            btnPrevMonth.clicked += () => 
            {
                currentDate = currentDate.AddMonths(-1);
                RenderizarCalendario();
            };
        }

        if (btnNextMonth != null)
        {
            btnNextMonth.clicked += () => 
            {
                currentDate = currentDate.AddMonths(1);
                RenderizarCalendario();
            };
        }
    }

    private void CarregarTarefasDoDisco()
    {
        if (PlayerPrefs.HasKey(WD_SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(WD_SAVE_KEY);
            myWorkDesks = JsonUtility.FromJson<WorkDeskDatabase>(json);
        }
        if (myWorkDesks == null) myWorkDesks = new WorkDeskDatabase();
    }

    private void RenderizarCalendario()
    {
        // 1. Atualiza o Título (Ex: "Setembro 2026")
        if (labelMonthYear != null)
        {
            labelMonthYear.text = $"{nomeMeses[currentDate.Month]} {currentDate.Year}";
        }

        // 2. Limpa os dias antigos
        if (daysContainer == null) return;
        daysContainer.Clear();

        // 3. Descobre o primeiro dia do mês e quantos dias o mês tem
        DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
        
        // Em C#, Domingo = 0, Segunda = 1, etc.
        int startingDayOfWeek = (int)firstDayOfMonth.DayOfWeek; 

        // 4. Cria os espaços em branco antes do dia 1
        for (int i = 0; i < startingDayOfWeek; i++)
        {
            daysContainer.Add(CriarCelulaVazia());
        }

        // 5. Cria os dias do mês
        for (int day = 1; day <= daysInMonth; day++)
        {
            // Formata o dia para comparar com as tarefas (Ex: "08/09/2026")
            string dateString = $"{day:D2}/{currentDate.Month:D2}/{currentDate.Year}";
            
            // Verifica se existe alguma tarefa com essa data
            bool hasTask = myWorkDesks.items.Any(task => task.dueDate == dateString);

            daysContainer.Add(CriarCelulaDeDia(day, hasTask));
        }
    }

    // Cria um espaço invisível para alinhar a primeira semana
    private VisualElement CriarCelulaVazia()
    {
        VisualElement cell = new VisualElement();
        cell.style.width = Length.Percent(14.28f); // 100% dividido por 7 dias
        cell.style.height = 100;
        return cell;
    }

    // Cria o quadradinho do dia com o número
    private VisualElement CriarCelulaDeDia(int day, bool hasTask)
    {
        VisualElement cell = new VisualElement();
        cell.style.width = Length.Percent(14.28f); 
        cell.style.height = 100;
        cell.style.alignItems = Align.Center;
        cell.style.justifyContent = Justify.Center;

        Label labelDay = new Label(day.ToString());
        labelDay.style.fontSize = 32;
        labelDay.style.color = Color.black;
        labelDay.style.unityFontStyleAndWeight = FontStyle.Bold;

        // Se for o dia de HOJE, pinta o fundo de uma cor diferente
        if (day == DateTime.Now.Day && currentDate.Month == DateTime.Now.Month && currentDate.Year == DateTime.Now.Year)
        {
            cell.style.backgroundColor = new StyleColor(new Color(0.85f, 0.95f, 1f)); // Azul clarinho
            
            cell.style.borderTopLeftRadius = 15;
            cell.style.borderTopRightRadius = 15;
            cell.style.borderBottomLeftRadius = 15;
            cell.style.borderBottomRightRadius = 15;
        }

        cell.Add(labelDay);

        // Se tiver tarefa, adiciona uma bolinha vermelha embaixo do número
        if (hasTask)
        {
            VisualElement taskIndicator = new VisualElement();
            taskIndicator.style.width = 15;
            taskIndicator.style.height = 15;
            taskIndicator.style.backgroundColor = new StyleColor(Color.red);
            taskIndicator.style.marginTop = 5;
            
            // Arredonda pra virar uma bolinha
            taskIndicator.style.borderTopLeftRadius = 8;
            taskIndicator.style.borderTopRightRadius = 8;
            taskIndicator.style.borderBottomLeftRadius = 8;
            taskIndicator.style.borderBottomRightRadius = 8;

            cell.Add(taskIndicator);
        }

        return cell;
    }
}