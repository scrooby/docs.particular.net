using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

#region unit-of-work-behavior
class MyUowBehavior :
    Behavior<IIncomingPhysicalMessageContext>
{
    MySessionProvider sessionProvider;

    public MyUowBehavior(MySessionProvider sessionProvider)
    {
        this.sessionProvider = sessionProvider;
    }

    public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
    {
        var tennant = context.MessageHeaders["tennant"];

        using (var session = await sessionProvider.Open(tennant))
        {
            context.Extensions.Set<IMySession>(session);

            try
            {
                await next();

                await session.Commit();
            }
            catch (Exception)
            {
                await session.Rollback();

                throw;
            }
        }
    }
}
#endregion