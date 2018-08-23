using Kongverge.Common;
using Kongverge.Common.DTOs;
using Kongverge.Common.Plugins;
using Kongverge.Common.Services;
using Moq;

namespace KongVerge.Tests.Workflow
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
            KongWriter.Verify(k =>
                k.AddService(It.IsAny<KongService>()), Times.Never);
            KongWriter.Verify(k =>
                k.DeleteService(It.IsAny<string>()), Times.Never);
            KongWriter.Verify(k =>
                k.AddRoute(It.IsAny<KongService>(), It.IsAny<KongRoute>()), Times.Never);
            KongWriter.Verify(k =>
                k.DeleteRoute(It.IsAny<string>()), Times.Never);
        }
    }
}
