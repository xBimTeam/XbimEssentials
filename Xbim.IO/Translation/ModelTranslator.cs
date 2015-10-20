using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xbim.Common;

namespace Xbim.IO.Translation
{
    public class ModelTranslator
    {
        private readonly Dictionary<Type, EntityTranslator> _translators = new Dictionary<Type, EntityTranslator>();
        private readonly Dictionary<IPersistEntity, IPersistEntity> _map = new Dictionary<IPersistEntity, IPersistEntity>();

        public IModel Source { get; private set; }
        public IModel Target { get; private set; }

        public ModelTranslator(IModel source, IModel target)
        {
            Source = source;
            Target = target;
        }

        public void Translate()
        {
            foreach (var instance in Source.Instances)
                TranslateEntity(instance);
        }

        public void AddTranslator(EntityTranslator translator)
        {
            _translators.Add(translator.CanTranslateType, translator);
        }

        public IEnumerable<EntityTranslator> Translators { get { return _translators.Values; } } 

        public IPersistEntity TranslateEntity(IPersistEntity original)
        {
            IPersistEntity result;
            if (_map.TryGetValue(original, out result))
                return result;

            var type = original.GetType();
            EntityTranslator translator;
            if (!_translators.TryGetValue(type, out translator))
            {
                translator = new EntityTranslator(this, type);
                _translators.Add(type, translator);
            }

            result = translator.Translate(original);
            _map.Add(original, result);

            return result;
        }
    }
}
