using Kongverge.Common.DTOs;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Kongverge.Common.Workflow;
using Kongverge.KongPlugin;
using Moq;

namespace Kongverge.Common.Tests.Workflow
{
    internal class KongProcessorEnvironment
    {
        public readonly Mock<IKongAdminWriter> KongWriter = new Mock<IKongAdminWriter>();
        public readonly Mock<IKongPluginCollection> KongPluginCollection = new Mock<IKongPluginCollection>();
        public KongProcessor Processor { get; }

        public KongProcessorEnvironment()
        {
            Processor = new KongProcessor(KongWriter.Object, KongPluginCollection.Object);
        }

        public void VerifyNoActionTaken()
        {
            VerifyNoAdds();
            VerifyNoDeletes();
        }

        public void VerifyNoDeletes()
        {
            KongWriter.Verify(k => k.DeleteService(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
            KongWriter.Verify(k => k.DeleteRoute(It.IsAny<string>()), Times.Never);
            KongWriter.Verify(k => k.DeletePlugin(It.IsAny<string>()), Times.Never);
        }

        public void VerifyNoAdds()
        {
            KongWriter.Verify(k => k.AddService(It.IsAny<KongService>()), Times.Never);
            KongWriter.Verify(k => k.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Never);
            KongWriter.Verify(k => k.UpsertPlugin(It.IsAny<PluginBody>()), Times.Never);
        }
    }
}
