using System.Collections.Generic;

[System.Serializable]
public class WorkDeskItem
{
    public string id; // Precisamos de um ID para saber qual estamos editando/deletando
    public string title;
    public string description; // NOVO: Campo de descrição adicionado
    public string dueDate;
    public string priority;
}

[System.Serializable]
public class WorkDeskDatabase
{
    public List<WorkDeskItem> items = new List<WorkDeskItem>();
}