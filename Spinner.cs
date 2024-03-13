namespace GitDeleteObsoleteBranches;

internal class Spinner
{
    private readonly CancellationTokenSource _source = new();
    private readonly int _spinnerPosition = Console.CursorLeft;

    public void Start()
    {
        Task.Run(() => ShowLoading(_source.Token));
    }

    public void Stop()
    {
        _source.Cancel();
        Console.CursorLeft = _spinnerPosition;
    }

    private async Task ShowLoading(CancellationToken token)
    {
        var states = new[] { "|", "/", "-", "\\" };
        var index = 0;

        while (!token.IsCancellationRequested)
        {
            var spinChar = states[index];
            Console.CursorLeft = _spinnerPosition;
            Console.Write(spinChar);
            index = index + 1 == states.Length ? 0 : index + 1;
            await Task.Delay(200);
        }
    }
}
