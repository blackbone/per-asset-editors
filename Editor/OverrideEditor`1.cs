using NUnit.Framework;
using UnityEngine;

namespace B.PerAssetEditors
{
    public abstract class OverrideEditor<TAsset> : OverrideEditor where TAsset: Object
    {
        protected TAsset Target => RawTarget as TAsset;

        protected sealed override void OnEnable()
        {
            base.OnEnable();
            
            Assert.IsNotNull(Target);
            EnableInternal();
        }

        protected sealed override void OnDisable()
        {
            DisableInternal();
            base.OnDisable();
        }

        protected sealed override void OnAssetDirty()
        {
            base.OnAssetDirty();
            ApplyChangesInternal();
        }

        protected virtual void EnableInternal()
        {
            // no op
        }

        protected virtual void DisableInternal()
        {
            // no op
        }

        protected virtual void ApplyChangesInternal()
        {
            // no op
        }
    }
}