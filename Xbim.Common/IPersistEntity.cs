using System;
using Xbim.Common.Metadata;

namespace Xbim.Common
{
	public interface IPersistEntity : IPersist
	{
        /// <summary>
        /// Entity Label is an identifier unique inside one IModel. It can't be changed as other objects rely on it.
        /// </summary>
		int EntityLabel {get; }

        /// <summary>
        /// Model which contains this entity. No entity can exist outside of a IModel
        /// </summary>
		IModel Model { get; }

        /// <summary>
        /// Some implementations of IModel may implement lazy loading when internal data of
        /// the entity is only loaded when needed. This flag is used by entity itself and possibly
        /// by IModel for performance optimization.
        /// </summary>
		bool Activated { get; }

        /// <summary>
        /// This function should only be called by external code if you know very well what you are doing.
        /// Otherwise it is called by the entity internally when you first access the internal data and it has
        /// not been activated yet.
        /// </summary>
        void Activate();

        /// <summary>
        /// Cached reflection information for this type of object
        /// </summary>
		ExpressType ExpressType { get; }

		[Obsolete("This property is deprecated and likely to be removed. Use just 'Model' instead.")]
		IModel ModelOf { get; }
	}
}