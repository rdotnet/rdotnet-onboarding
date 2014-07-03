using System.Collections.Generic;
using RDotNet;
using RDotNet.Devices;
using RDotNet.Internals;

namespace WebApplicationRdn.Models
{
    public class CharacterDevice : ICharacterDevice
    {
        private readonly List<string> _output = new List<string>();
        private string _pending = string.Empty;

        public IEnumerable<string> GetOutput()
        {
            return _output;
        }

        public string ReadConsole(string prompt, int capacity, bool history)
        {
            _output.Add(prompt);
            return string.Empty;
        }

        public void WriteConsole(string output, int length, ConsoleOutputType outputType)
        {
            _pending += output;
            if (output.IndexOfAny(new[] { '\r', '\n' }) == 0)
            {
                _output.Add(_pending);
                _pending = string.Empty;
            }
        }

        public void ShowMessage(string message)
        {
            _output.Add(message);
        }

        public void Busy(BusyType which)
        { }

        public void Callback()
        { }

        public YesNoCancel Ask(string question)
        {
            return YesNoCancel.Yes;
        }

        public void Suicide(string message)
        {
            CleanUp(StartupSaveAction.Suicide, 2, false);
        }

        public void ResetConsole()
        {
            _pending = string.Empty;
            _output.Clear();
        }

        public void FlushConsole()
        {
            _pending = string.Empty;
        }

        public void ClearErrorConsole()
        {
            _pending = string.Empty;
            _output.Clear();
        }

        public void CleanUp(StartupSaveAction saveAction, int status, bool runLast)
        { }

        public bool ShowFiles(string[] files, string[] headers, string title, bool delete, string pager)
        {
            return true;
        }

        public string ChooseFile(bool create)
        {
            return string.Empty;
        }

        public void EditFile(string file)
        { }

        public SymbolicExpression LoadHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
        {
            throw new System.NotImplementedException();
        }

        public SymbolicExpression SaveHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
        {
            throw new System.NotImplementedException();
        }

        public SymbolicExpression AddHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
        {
            throw new System.NotImplementedException();
        }
    }
}
