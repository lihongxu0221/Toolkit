//namespace RoslynPad.UI;

//public abstract class NotificationObject : ObservableValidator
//{
//    private ConcurrentDictionary<string, List<ErrorInfo>>? _propertyErrors;

//    protected void SetError(string propertyName, string id, string message)
//    {
//        if (_propertyErrors == null)
//        {
//            _ = LazyInitializer.EnsureInitialized(
//                ref _propertyErrors,
//                () => new ConcurrentDictionary<string, List<ErrorInfo>>());
//        }

//        List<ErrorInfo> errors = _propertyErrors.GetOrAdd(propertyName, _ => []);
//        _ = errors.RemoveAll(e => e.Id == id);
//        errors.Add(new ErrorInfo(id, message));

//        OnErrorsChanged(propertyName);
//    }

//    protected void ClearError(string propertyName, string id)
//    {
//        if (_propertyErrors == null)
//        {
//            return;
//        }

//        _ = _propertyErrors.TryGetValue(propertyName, out List<ErrorInfo>? errors);
//        if (errors?.RemoveAll(e => e.Id == id) > 0)
//        {
//            OnErrorsChanged(propertyName);
//        }
//    }

//    protected void ClearErrors(string propertyName)
//    {
//        if (_propertyErrors == null)
//        {
//            return;
//        }

//        _ = _propertyErrors.TryGetValue(propertyName, out var errors);
//        if (errors?.Count > 0)
//        {
//            errors.Clear();

//            OnErrorsChanged(propertyName);
//        }
//    }

//    public IEnumerable GetErrors(string? propertyName)
//    {
//        if (propertyName == null)
//        {
//            return Array.Empty<ErrorInfo>();
//        }

//        List<ErrorInfo>? errors = null;
//        _ = _propertyErrors?.TryGetValue(propertyName, out errors);
//        return errors?.AsEnumerable() ?? [];
//    }

//    public bool HasErrors => _propertyErrors?.Any(c => c.Value.Count != 0) == true;

//    protected virtual void OnErrorsChanged(string propertyName)
//    {
//        base.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
//    }

//    protected class ErrorInfo(string id, string message)
//    {
//        public string Id { get; } = id;

//        public string Message { get; } = message;

//        public override string ToString() => Message;
//    }
//}
