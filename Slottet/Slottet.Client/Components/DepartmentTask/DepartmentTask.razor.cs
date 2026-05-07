using Microsoft.AspNetCore.Components;

namespace Slottet.Client.Components.DepartmentTask;

public partial class DepartmentTask
{
    [Parameter, EditorRequired] public List<string> Items { get; set; } = default!;
    [Parameter] public EventCallback<List<string>> OnSave { get; set; }

    private List<string> _editItems = new();
    private bool         _saved;

    protected override void OnParametersSet()
    {
        _editItems = new List<string>(Items);
        _saved     = false;
    }

    private void AddItem()         => _editItems.Add(string.Empty);
    private void RemoveItem(int i) => _editItems.RemoveAt(i);

    private async Task Save()
    {
        Items.Clear();
        Items.AddRange(_editItems.Where(x => !string.IsNullOrWhiteSpace(x)));
        _saved = true;
        await OnSave.InvokeAsync(Items);
    }
}
