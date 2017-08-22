using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Commands
{
    class CommandSeeker
    {
        public CommandEntity GetCommandsFromAttributeAsync()
        {
            CommandEntity commandHierarchyList = new CommandEntity();
            commandHierarchyList.Id = "root";

            Assembly asm = Assembly.GetEntryAssembly();

            var modules = asm.GetTypes()
                .Where(m => m.GetTypeInfo().GetCustomAttributes<ModuleAttribute>().Any() && !m.IsNested)
                .ToArray();

            int modulesLoaded = 0;
            int subModulesLoaded = 0;
            int commandsLoaded = 0;

            foreach (var m in modules)
            {
                CommandEntity entity = GetChildrenFromModule(m);
                commandHierarchyList.Children.Add(entity);

                modulesLoaded++;

                subModulesLoaded += entity.Children.Count(x => (x as Module) != null);
                commandsLoaded += entity.GetAllEntitiesOf<Command>().Count;
            }
            return commandHierarchyList;
        }

        public CommandEntity GetChildrenFromModule(Type attribute)
        {
            CommandEntity module = new CommandEntity();
            ModuleAttribute moduleAttribute = attribute.GetTypeInfo().GetCustomAttribute<ModuleAttribute>();

            module = new Module(moduleAttribute.Entity as Module);

            object constructedInstance = null;

            List<Type> allChildren = attribute.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)
                .Where((x) => x.GetTypeInfo().GetCustomAttributes<CommandEntityAttribute>().Any())
                .ToList();

            try
            {
                constructedInstance = Activator.CreateInstance(Type.GetType(attribute.AssemblyQualifiedName), module);
            }
            catch
            {
                constructedInstance = Activator.CreateInstance(Type.GetType(attribute.AssemblyQualifiedName));
            }

            foreach (Type t in allChildren)
            {
                CommandEntityAttribute entityAttribute = t.GetTypeInfo().GetCustomAttribute<CommandEntityAttribute>();

                if(entityAttribute is ModuleAttribute mAttribute)
                {
                    CommandEntity entity = GetChildrenFromModule(t);

                    module = new Module(mAttribute.Entity as Module);

                    entity.Parent = module;
                    module.Children.Add(entity);
                }
            }

            List<MethodInfo> methods = attribute.GetMethods()
                .Where((x) => x.GetCustomAttributes<CommandAttribute>().Any())
                .ToList();

            foreach (MethodInfo m in methods)
            {
                CommandAttribute commandAttribute = m.GetCustomAttribute<CommandAttribute>();
                Command newEvent = new Command(commandAttribute.Entity as Command);

                newEvent.ProcessCommand =
                    async (context) => await (Task)m.Invoke(constructedInstance, new object[] { context });
                newEvent.Parent = module;
                module.Children.Add(newEvent);
            }
            return module;
        }
    }
}
