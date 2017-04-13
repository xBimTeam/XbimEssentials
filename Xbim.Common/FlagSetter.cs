namespace Xbim.Common
{
    public class FlagSetter
    {
        public static void SetActivationFlag(IPersistEntity entity, bool value)
        {
            var p = entity as PersistEntity;
            if (ReferenceEquals(p, null))
                return;
            p._activated = true;
        }

    }
}
