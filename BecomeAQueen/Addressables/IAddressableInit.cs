namespace _Game.Scripts.Addressables
{
    /// <summary>
    /// Scriptable Object architecture has a limitation when using Addressables:
    /// when instantiating Addressable asset, all SO dependencies will be instantiated for this asset as well.
    /// As a result, non-Addressable assets will call one instance of SO, and instantiated Addressable asset
    /// will call another, which can cause not expected behaviour and bugs.
    /// todo - check this behaviour in case there is single SO in different Addressable bundles
    /// 
    /// It seems, this behaviour is intended, according to Unity devs:
    /// https://forum.unity.com/threads/scriptableobject-asset-and-addressables.652525/
    /// https://forum.unity.com/threads/v0-6-7-scriptable-object-reference-is-a-new-instance.642061/#post-4372054
    ///
    /// As a workaround for this behaviour we will pass all SO we want after Addressable asset instantiation.
    /// This will make sure, Addressable asset uses correct instance of all SO.
    ///
    /// As a result, when using Scriptable Object architecture we should consider following rules:
    /// 1. We can use SO for communicating between different components inside Addressable asset.
    ///    But we can't use these SO for communicating outside this asset. If we want
    ///    Addressable asset communicate to non-Addressable assets, check next point.
    /// 2. We can use SO between communicating non-Addressable parts of the code.
    ///    If we want use such SO inside Addressable asset, we must explicitly pass these SO during
    ///    Addressable asset initialization.
    /// </summary>
    /// <typeparam name="SODependency">SO dependency we want to pass to Addressable Asset during its initialization</typeparam>
    public interface IAddressableInit<SODependency>
    {
        void Init(SODependency dependency);
    }
}