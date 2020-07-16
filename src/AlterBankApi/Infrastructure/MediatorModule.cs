namespace AlterBankApi.Infrastructure
{
    using System.Reflection;
    using AlterBankApi.Application.Handlers;
    using Autofac;
    using MediatR;

    /// <summary>
    /// The mediator module configuration
    /// </summary>
    public class MediatorModule : Autofac.Module
    {
        /// <summary>
        /// Loads module configuration
        /// </summary>
        /// <param name="builder">Container builder</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(AccountRequestHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });
        }
    }
}
