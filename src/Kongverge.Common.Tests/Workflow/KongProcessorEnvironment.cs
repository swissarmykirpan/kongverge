using Kongverge.Common.DTOs;
using Kongverge.Common.Services;
using Kongverge.Common.Workflow;
using Moq;

namespace Kongverge.Common.Tests.Workflow
{
    internal class KongProcessorEnvironment
    {
        public readonly Mock<IKongAdminWriter> KongWriter = new Mock<IKongAdminWriter>();
        public KongProcessor Processor { get; }

        public KongProcessorEnvironment()
        {
            Processor = new KongProcessor(KongWriter.Object);
        }

        public void VerifyNoActionTaken()
        {
            VerifyNoAdds();
            VerifyNoDeletes();
        }

        public void VerifyNoDeletes()
        {
            KongWriter.Verify(k => k.DeleteService(It.IsAny<string>()), Times.Never);
            KongWriter.Verify(k => k.DeleteRoute(It.IsAny<string>()), Times.Never);
            KongWriter.Verify(k => k.DeletePlugin(It.IsAny<string>()), Times.Never);
        }

        public void VerifyNoAdds()
        {
            KongWriter.Verify(k => k.AddService(It.IsAny<KongService>()), Times.Never);
            KongWriter.Verify(k => k.AddRoute(It.IsAny<string>(), It.IsAny<KongRoute>()), Times.Never);
            KongWriter.Verify(k => k.UpsertPlugin(It.IsAny<KongPlugin>()), Times.Never);
        }
    }
}
