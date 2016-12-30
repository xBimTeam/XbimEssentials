using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common.ExpressValidation;

namespace Xbim.Essentials.Tests
{
    public class ValidationReporter : IEnumerable<string>
    {
        private readonly Stack<IEnumerator<ValidationResult>> _queue = new Stack<IEnumerator<ValidationResult>>();
        private IEnumerator<ValidationResult> _currentEnum;
        private int _indent = 0;

        public ValidationReporter(IEnumerable<ValidationResult> results)
        {
            _currentEnum = results.GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            while (true)
            {
                var mn = _currentEnum.MoveNext();
                if (mn)
                {
                    // current exists
                    yield return new string('\t', _indent) + _currentEnum.Current.Message;
                    // steps into a child
                    if (_currentEnum.Current.Details.Any())
                    {
                        _queue.Push(_currentEnum);
                        _currentEnum = _currentEnum.Current.Details.GetEnumerator();
                        _indent++;
                    }
                }
                else
                {
                    // current does not exist
                    // steps out to the parent
                    if (_queue.Any())
                    {
                        _currentEnum = _queue.Pop();
                        _indent--;
                    }
                    else
                        yield break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}